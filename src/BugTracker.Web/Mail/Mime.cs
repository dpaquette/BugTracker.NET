/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using OpenPop.Mime;

namespace btnet.Mail
{
    public class Mime
    {

        ///////////////////////////////////////////////////////////////////////    
        public static Message GetMimeMessage(string message_raw_string)
        {            
            // feed a stream to MIME parser
            byte[] bytes = Encoding.UTF8.GetBytes(message_raw_string);
            return new Message(bytes);
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
                var sql = new SQLString("select count(1) from bugs where bg_id = @bg");
                sql = sql.Replace("bg", Convert.ToString(bugid));
                int bug_count = (int)btnet.DbUtil.execute_scalar(sql);
                if (bug_count != 1)
                {
                    subject = subject.Replace(bugidString, "WAS #:");
                    bugid = 0;
                }
            }

            return bugid;
        }

        public static string get_from_addr(Message message)
        {
            return message.Headers.From.Address;
        }

        public static string get_subject(Message message)
        {
            
            string subject = message.Headers.Subject;

            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = "[No Subject]";
            }

            return subject;
        }

        public static string get_cc(Message message)
        {
            string cc = string.Join("; ", message.Headers.Cc.Select(c => c.Address));
            return cc;
        }

        public static string get_to(Message message)
        {
            return string.Join("; ", message.Headers.To.Select(c => c.Address));
        }

        public static string get_comment(Message message)
        {
            string commentText = null;
            MessagePart comment = message.FindFirstPlainTextVersion();
            if (comment != null)
            {
                commentText = comment.GetBodyAsText();
                if (string.IsNullOrEmpty(commentText))
                {
                    comment = message.FindFirstHtmlVersion();
                    if (comment != null)
                    {
                        commentText = comment.GetBodyAsText();
                    }
                }
            }

            if (string.IsNullOrEmpty(commentText))
            {
                commentText = "NO PLAIN TEXT MESSAGE BODY FOUND";
            }

            return commentText;
        }

        ///////////////////////////////////////////////////////////////////////    
        public static DataRow get_user_datarow_maybe_using_from_addr(Message message, string from_addr, string username)
        {

            DataRow dr = null;

            var sql = new SQLString( @"
select us_id, us_admin, us_username, us_org, og_other_orgs_permission_level, isnull(us_forced_project,0) us_forced_project
from users
inner join orgs on us_org = og_id
where us_username = @us");

            // Create a new user from the "from" email address    
            string btnet_service_username = Util.get_setting("CreateUserFromEmailAddressIfThisUsername", "");
            if (!string.IsNullOrEmpty(from_addr) && username == btnet_service_username)
            {
                from_addr = get_from_addr(message);

                // See if there's already a username that matches this email address
                username = Email.simplify_email_address(from_addr);

                // Does a user with this email already exist?
                sql = sql.Replace("us", username);

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
        public static void add_attachments(Message message, int bugid, int parent_postid, Security security)
        {
            foreach (MessagePart attachment in message.FindAllAttachments())
            {
                add_attachment(attachment.FileName, attachment, bugid, parent_postid, security);                
            }
        }

        ///////////////////////////////////////////////////////////////////////

        public static void add_attachment(string filename, MessagePart part, int bugid, int parent_postid, Security security)
        {

            Util.write_to_log("attachment:" + filename);

            string missing_attachment_msg = "";

            int max_upload_size = Convert.ToInt32(Util.get_setting("MaxUploadSize", "100000"));
            if (part.Body.Length > max_upload_size)
            {
                missing_attachment_msg = "ERROR: email attachment exceeds size limit.";
            }

            string content_type = part.ContentType.MediaType;
            string desc;
            MemoryStream attachmentStream = new MemoryStream(part.Body);

            if (missing_attachment_msg == "")
            {
                desc = "email attachment";
            }
            else
            {
                desc = missing_attachment_msg;
            }

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
        public static string get_headers_for_comment(Message message)
        {
            string headers = "";
            string subject = get_subject(message);
            if (!string.IsNullOrEmpty(subject))
            {
                headers = "Subject: " + subject + "\n";
            }

            string to = get_to(message);
            if (!string.IsNullOrEmpty(to))
            {
                headers += "To: " + to + "\n";
            }

            string cc = get_cc(message);
            if (!string.IsNullOrEmpty(cc))
            {
                headers += "Cc: " + cc + "\n";
            }

            return headers;
        }

        public static Security get_synthesized_security(Message message, string from_addr, string username)
        {
            // Get the btnet user, which might actually be a user that corresonds with the email sender, not the username above
            DataRow dr = Mime.get_user_datarow_maybe_using_from_addr(message, from_addr, username);

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