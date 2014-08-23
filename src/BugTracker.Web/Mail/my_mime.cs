/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.IO;
using System.Text;
using anmar.SharpMimeTools;

namespace btnet.Mail
{
    public class Mime
    {

        ///////////////////////////////////////////////////////////////////////    
        public static SharpMimeMessage get_sharp_mime_message(string message_raw_string)
        {
            // feed a stream to MIME parser
            byte[] bytes = Encoding.UTF8.GetBytes(message_raw_string);
            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            return new SharpMimeMessage(ms);
        }

        public static int get_bugid_from_subject(ref string subject)
        {
            int bugid = 0;

            // Try to parse out the bugid from the subject line
            string bugidString = Util.get_setting("TrackingIdString", "DO NOT EDIT THIS:");

            int pos = subject.IndexOf(bugidString);

            if (pos >= 0)
            {
                // position of colon
                pos = subject.IndexOf(":", pos);
                pos++;

                // position of close paren
                int pos2 = subject.IndexOf(")", pos);
                if (pos2 > pos)
                {
                    string bugid_string_temp = subject.Substring(pos, pos2 - pos);
                    if (Util.is_int(bugid_string_temp))
                    {
                        bugid = Convert.ToInt32(bugid_string_temp);
                    }
                }
            }

            // maybe a deleted bug?
            if (bugid != 0)
            {
                string sql = "select count(1) from bugs where bg_id = $bg";
                sql = sql.Replace("$bg", Convert.ToString(bugid));
                int bug_count = (int)btnet.DbUtil.execute_scalar(sql);
                if (bug_count != 1)
                {
                    subject = subject.Replace(bugidString, "WAS #:");
                    bugid = 0;
                }
            }

            return bugid;
        }

        public static string get_from_addr(SharpMimeMessage mime_message)
        {
            string from_addr = "";

            if (mime_message.Header.From != null && mime_message.Header.From != "")
            {
                from_addr = SharpMimeTools.parserfc2047Header(mime_message.Header.From);

                // handle multiline subject
                from_addr = from_addr.Replace("\t", " ");
            }
            else
            {
                from_addr = "[No From]";
            }

            return from_addr;
        }

        public static string get_subject(SharpMimeMessage mime_message)
        {
            string subject = "";

            if (mime_message.Header.Subject != null && mime_message.Header.Subject != "")
            {
                subject = SharpMimeTools.parserfc2047Header(mime_message.Header.Subject);

                // handle multiline subject
                subject = subject.Replace("\t", " ");

            }
            else
            {
                subject = "[No Subject]";
            }

            return subject;
        }

        public static string get_cc(SharpMimeMessage mime_message)
        {
            string cc = "";
            if (mime_message.Header.Cc != null && mime_message.Header.Cc != "")
            {
                cc = SharpMimeTools.parserfc2047Header(mime_message.Header.Cc);

                // handle multiline
                cc = cc.Replace("\t", " ");
            }
            return cc;
        }

        public static string get_to(SharpMimeMessage mime_message)
        {
            string to = "";
            if (mime_message.Header.To != null && mime_message.Header.To != "")
            {
                to = SharpMimeTools.parserfc2047Header(mime_message.Header.To);

                // handle multiline
                to = to.Replace("\t", " ");
            }
            return to;
        }

        public static string get_comment(SharpMimeMessage mime_message)
        {
            string comment = Mime.extract_comment_text_from_email(mime_message, "text/plain");
            if (comment == null)
            {
                comment = Mime.extract_comment_text_from_email(mime_message, "text/html");
            }

            if (comment == null)
            {
                comment = "NO PLAIN TEXT MESSAGE BODY FOUND";
            }

            return comment;
        }

        ///////////////////////////////////////////////////////////////////////    
        public static DataRow get_user_datarow_maybe_using_from_addr(SharpMimeMessage mime_message, string from_addr, string username)
        {

            DataRow dr = null;

            string sql = @"
select us_id, us_admin, us_username, us_org, og_other_orgs_permission_level, isnull(us_forced_project,0) us_forced_project
from users
inner join orgs on us_org = og_id
where us_username = N'$us'";

            // Create a new user from the "from" email address    
            string btnet_service_username = Util.get_setting("CreateUserFromEmailAddressIfThisUsername", "");
            if (!string.IsNullOrEmpty(from_addr) && username == btnet_service_username)
            {

                // We can do a better job of parsing the from_addr here than we did in btnet_service.exe    
                if (mime_message != null)
                {
                    if (mime_message.Header.From != null && mime_message.Header.From != "")
                    {
                        from_addr = SharpMimeTools.parserfc2047Header(mime_message.Header.From);

                        // handle multiline from
                        from_addr = from_addr.Replace("\t", " ");
                    }
                }

                // See if there's already a username that matches this email address
                username = Email.simplify_email_address(from_addr);

                // Does a user with this email already exist?
                sql = sql.Replace("$us", username.Replace("'", "''"));

                // We maybe found user@example.com, so let's use him as the user instead of the btnet_service.exe user
                dr = btnet.DbUtil.get_datarow(sql);

                // We didn't find the user, so let's create him, using the email address as the username.	
                if (dr == null)
                {

                    bool use_domain_as_org_name = Util.get_setting("UseEmailDomainAsNewOrgNameWhenCreatingNewUser", "0") == "1";

                    btnet.User.copy_user(
                        username,
                        username,
                        "", "", "",  // first, last, signature
                        0,  // salt
                        Guid.NewGuid().ToString(), // random value for password,
                        Util.get_setting("CreateUsersFromEmailTemplate", "[error - missing user template]"),
                        use_domain_as_org_name);

                    // now that we have created a user, try again
                    dr = btnet.DbUtil.get_datarow(sql);
                }
            }
            else
            {
                // Use the btnet_service.exe user as the username
                sql = sql.Replace("$us", username.Replace("'", "''"));
                dr = btnet.DbUtil.get_datarow(sql);
            }

            return dr;
        }


        ///////////////////////////////////////////////////////////////////////
        // This should be rewritten with recursion...
        public static string extract_comment_text_from_email(SharpMimeMessage mime_message, string mimetype)
        {

            string comment = null;

            // use the first plain text message body
            foreach (SharpMimeMessage part in mime_message)
            {
                if (part.IsMultipart)
                {
                    foreach (SharpMimeMessage subpart in part)
                    {

                        if (subpart.IsMultipart)
                        {
                            foreach (SharpMimeMessage sub2 in subpart)
                            {
                                if (sub2.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                                && !is_attachment(sub2))
                                {
                                    comment = sub2.BodyDecoded;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (subpart.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                            && !is_attachment(subpart))
                            {
                                comment = subpart.BodyDecoded;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (part.Header.ContentType.ToLower().IndexOf(mimetype) > -1
                    && !is_attachment(part))
                    {
                        comment = part.BodyDecoded;
                        break;
                    }
                }
            }


            if (comment == null)
            { 
                if (mime_message.Header.ContentType.ToLower().IndexOf(mimetype) > -1)
                {
                    comment = mime_message.BodyDecoded;
                }
            }

            return comment;
        }


        ///////////////////////////////////////////////////////////////////////
        public static bool is_attachment(SharpMimeMessage part)
        {
            string filename = part.Header.ContentDispositionParameters["filename"];
            if (string.IsNullOrEmpty(filename))
            {
                filename = part.Header.ContentTypeParameters["name"];
            }

            if (filename != null && filename != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static string determine_part_filename(SharpMimeMessage part)
        {


            string filename = "";

            filename = part.Header.ContentDispositionParameters["filename"];


            // try again
            if (string.IsNullOrEmpty(filename))
            {
                filename = part.Header.ContentTypeParameters["name"];
            }


            // Maybe it's still some sort of non-text part but without a filename.
            // Like an inline image, or the html alternative of a plain text body.
            if (string.IsNullOrEmpty(filename))
            {

                if (part.Header.ContentType.ToLower().IndexOf("text/plain") > -1)
                {
                    // The plain text body.  We don't want to
                    // add this as an attachment.

                }
                else
                {

                    // Some other mime part we don't understand.
                    // Let's make it an attachment with a synthesized filename, just so we don't loose it.

                    // Change text/html to text.html, etc
                    // so that downstream logic that reacts
                    // to the file extensions works.
                    filename = part.Header.ContentType;
                    filename = filename.Replace("/", ".");
                    int pos = filename.IndexOf(";");
                    if (pos > 0)
                    {
                        filename = filename.Substring(0, pos);
                    }
                }
            }

            if (filename == null)
            {
                filename = "";
            }

            return filename;

        }

        ///////////////////////////////////////////////////////////////////////
        public static void add_attachments(SharpMimeMessage mime_message, int bugid, int parent_postid, Security security)
        {
            if (mime_message.IsMultipart)
            {
                foreach (SharpMimeMessage part in mime_message)
                {
                    if (part.IsMultipart)
                    {
                        // recursive call to this function
                        add_attachments(part, bugid, parent_postid, security);
                    }
                    else
                    {

                        string filename = determine_part_filename(part);

                        if (filename != "")
                        {
                            add_attachment(filename, part, bugid, parent_postid, security);
                        }
                    }
                }
            }

            else
            {
                string filename = determine_part_filename(mime_message);

                if (filename != "")
                {
                    add_attachment(filename, mime_message, bugid, parent_postid, security);
                }
            }

        }

        ///////////////////////////////////////////////////////////////////////

        public static void add_attachment(string filename, SharpMimeMessage part, int bugid, int parent_postid, Security security)
        {

            Util.write_to_log("attachment:" + filename);

            string missing_attachment_msg = "";

            int max_upload_size = Convert.ToInt32(Util.get_setting("MaxUploadSize", "100000"));
            if (part.Size > max_upload_size)
            {
                missing_attachment_msg = "ERROR: email attachment exceeds size limit.";
            }

            string content_type = part.Header.TopLevelMediaType + "/" + part.Header.SubType;
            string desc;
            MemoryStream attachmentStream = new MemoryStream();

            if (missing_attachment_msg == "")
            {
                desc = "email attachment";
            }
            else
            {
                desc = missing_attachment_msg;
            }

            part.DumpBody(attachmentStream);
            attachmentStream.Position = 0;
            Bug.insert_post_attachment(
                security,
                bugid,
                attachmentStream,
                (int)attachmentStream.Length,
                filename,
                desc,
                content_type,
                parent_postid,
                false,  // not hidden
                false); // don't send notifications

        }


        ///////////////////////////////////////////////////////////////////////
        public static string get_headers_for_comment(SharpMimeMessage mime_message)
        {
            string headers = "";

            if (mime_message.Header.Subject != null && mime_message.Header.Subject != "")
            {
                headers = "Subject: " + Mime.get_subject(mime_message) + "\n";
            }

            if (mime_message.Header.To != null && mime_message.Header.To != "")
            {
                headers += "To: " + Mime.get_to(mime_message) + "\n";
            }

            if (mime_message.Header.Cc != null && mime_message.Header.Cc != "")
            {
                headers += "Cc: " + Mime.get_cc(mime_message) + "\n";
            }

            return headers;
        }

        public static Security get_synthesized_security(SharpMimeMessage mime_message, string from_addr, string username)
        {
            // Get the btnet user, which might actually be a user that corresonds with the email sender, not the username above
            DataRow dr = Mime.get_user_datarow_maybe_using_from_addr(mime_message, from_addr, username);

            // simulate a user having logged in, for downstream code
            Security security = new Security();
            security.context = System.Web.HttpContext.Current;
            security.user.username = username;
            security.user.usid = (int)dr["us_id"];
            security.user.is_admin = Convert.ToBoolean(dr["us_admin"]);
            security.user.org = (int)dr["us_org"];
            security.user.other_orgs_permission_level = (int)dr["og_other_orgs_permission_level"];
            security.user.forced_project = (int)dr["us_forced_project"];

            return security;
        }

    }
}