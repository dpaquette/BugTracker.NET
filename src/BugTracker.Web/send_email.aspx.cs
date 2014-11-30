using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using btnet.Mail;

namespace btnet
{
    public partial class send_email : BasePage
    {
        protected SQLString sql;
        protected Security security;
        
        protected int project = -1;
        protected bool enable_internal_posts = false;

        ///////////////////////////////////////////////////////////////////////
        public void Page_Load(Object sender, EventArgs e)
        {
            btnet.Util.do_not_cache(Response);
            MainMenu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");
            titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "send email";

            msg.InnerText = "";

            string string_bp_id = Request["bp_id"];
            string string_bg_id = Request["bg_id"];
            string request_to = Request["to"];
            string reply = Request["reply"];

            enable_internal_posts = (Util.get_setting("EnableInternalOnlyPosts", "0") == "1");

            if (!enable_internal_posts)
            {
                include_internal_posts.Visible = false;
                include_internal_posts_label.Visible = false;
            }

            if (!IsPostBack)
            {

                Session["email_addresses"] = null;

                DataRow dr = null;

                if (string_bp_id != null)
                {

                    string_bp_id = btnet.Util.sanitize_integer(string_bp_id);

                    sql = new SQLString(@"select
				bp_parent,
                bp_file,
                bp_id,
				bg_id,
				bg_short_desc,
				bp_email_from,
				bp_comment,
				bp_email_from,
				bp_date,
				bp_type,
                bp_content_type,
				bg_project,
                bp_hidden_from_external_users,
				isnull(us_signature,'') [us_signature],
				isnull(pj_pop3_email_from,'') [pj_pop3_email_from],
				isnull(us_email,'') [us_email],
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname]				
				from bug_posts
				inner join bugs on bp_bug = bg_id
				inner join users on us_id = @us
				left outer join projects on bg_project = pj_id
				where bp_id = @id
				or (bp_parent = @id and bp_type='file')");

                    sql = sql.AddParameterWithValue("id", string_bp_id);
                    sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));

                    DataView dv = btnet.DbUtil.get_dataview(sql);
                    dr = null;
                    if (dv.Count > 0)
                    {
                        dv.RowFilter = "bp_id = " + string_bp_id;
                        if (dv.Count > 0)
                        {
                            dr = dv[0].Row;
                        }
                    }

                    int int_bg_id = (int)dr["bg_id"];
                    int permission_level = btnet.Bug.get_bug_permission_level(int_bg_id, User.Identity);
                    if (permission_level == PermissionLevel.None)
                    {
                        Response.Write("You are not allowed to view this item");
                        Response.End();
                    }

                    if ((int)dr["bp_hidden_from_external_users"] == 1)
                    {
                        if (security.user.external_user)
                        {
                            Response.Write("You are not allowed to view this post");
                            Response.End();
                        }
                    }

                    string_bg_id = Convert.ToString(dr["bg_id"]);
                    back_href.HRef = "edit_bug.aspx?id=" + string_bg_id;
                    bg_id.Value = string_bg_id;


                    to.Value = dr["bp_email_from"].ToString();


                    // Work around for a mysterious bug:
                    // http://sourceforge.net/tracker/?func=detail&aid=2815733&group_id=66812&atid=515837
                    if (btnet.Util.get_setting("StripDisplayNameFromEmailAddress", "0") == "1")
                    {
                        to.Value = Email.simplify_email_address(to.Value);
                    }

                    load_from_dropdown(dr, true); // list the project's email address first

                    if (reply != null && reply == "all")
                    {
                        Regex regex = new Regex("\n");
                        string[] lines = regex.Split((string)dr["bp_comment"]);
                        string cc_addrs = "";

                        int max = lines.Length < 5 ? lines.Length : 5;

                        // gather cc addresses, which might include the current user
                        for (int i = 0; i < max; i++)
                        {
                            if (lines[i].StartsWith("To:") || lines[i].StartsWith("Cc:"))
                            {
                                string cc_addr = lines[i].Substring(3, lines[i].Length - 3).Trim();

                                // don't cc yourself

                                if (cc_addr.IndexOf(from.SelectedItem.Value) == -1)
                                {
                                    if (cc_addrs != "")
                                    {
                                        cc_addrs += ",";
                                    }

                                    cc_addrs += cc_addr;
                                }
                            }
                        }

                        cc.Value = cc_addrs;
                    }

                    if (dr["us_signature"].ToString() != "")
                    {
                        if (User.Identity.GetUseFCKEditor())
                        {
                            body.Value += "<br><br><br>";
                            body.Value += dr["us_signature"].ToString().Replace("\r\n", "<br>");
                            body.Value += "<br><br><br>";
                        }
                        else
                        {
                            body.Value += "\n\n\n";
                            body.Value += dr["us_signature"].ToString();
                            body.Value += "\n\n\n";
                        }
                    }


                    if (Request["quote"] != null)
                    {
                        Regex regex = new Regex("\n");
                        string[] lines = regex.Split((string)dr["bp_comment"]);

                        if (dr["bp_type"].ToString() == "received")
                        {
                            if (User.Identity.GetUseFCKEditor())
                            {
                                body.Value += "<br><br><br>";
                                body.Value += "&#62;From: " + dr["bp_email_from"].ToString().Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                            }
                            else
                            {
                                body.Value += "\n\n\n";
                                body.Value += ">From: " + dr["bp_email_from"] + "\n";
                            }
                        }

                        bool next_line_is_date = false;
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (i < 4 && (lines[i].IndexOf("To:") == 0 || lines[i].IndexOf("Cc:") == 0))
                            {
                                next_line_is_date = true;
                                if (User.Identity.GetUseFCKEditor())
                                {
                                    body.Value += "&#62;" + lines[i].Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                                }
                                else
                                {
                                    body.Value += ">" + lines[i] + "\n";
                                }
                            }
                            else if (next_line_is_date)
                            {
                                next_line_is_date = false;
                                if (User.Identity.GetUseFCKEditor())
                                {
                                    body.Value += "&#62;Date: " + Convert.ToString(dr["bp_date"]) + "<br>&#62;<br>";
                                }
                                else
                                {
                                    body.Value += ">Date: " + Convert.ToString(dr["bp_date"]) + "\n>\n";
                                }
                            }
                            else
                            {
                                if (User.Identity.GetUseFCKEditor())
                                {
                                    if (Convert.ToString(dr["bp_content_type"]) != "text/html")
                                    {
                                        body.Value += "&#62;" + lines[i].Replace("<", "&#60;").Replace(">", "&#62;") + "<br>";
                                    }
                                    else
                                    {
                                        if (i == 0)
                                        {
                                            body.Value += "<hr>";
                                        }

                                        body.Value += lines[i];
                                    }
                                }
                                else
                                {
                                    body.Value += ">" + lines[i] + "\n";
                                }
                            }
                        }
                    }

                    if (reply == "forward")
                    {
                        to.Value = "";
                        //original attachments
                        //dv.RowFilter = "bp_parent = " + string_bp_id;
                        dv.RowFilter = "bp_type = 'file'";
                        foreach (DataRowView drv in dv)
                        {
                            attachments_label.InnerText = "Select attachments to forward:";
                            lstAttachments.Items.Add(new ListItem(drv["bp_file"].ToString(), drv["bp_id"].ToString()));
                        }

                    }

                }
                else if (string_bg_id != null)
                {

                    string_bg_id = btnet.Util.sanitize_integer(string_bg_id);

                    int permission_level = btnet.Bug.get_bug_permission_level(Convert.ToInt32(string_bg_id), User.Identity);
                    if (permission_level == PermissionLevel.None
                    || permission_level == PermissionLevel.ReadOnly)
                    {
                        Response.Write("You are not allowed to edit this item");
                        Response.End();
                    }

                    sql = new SQLString(@"select
				bg_short_desc,
				bg_project,
				isnull(us_signature,'') [us_signature],
				isnull(us_email,'') [us_email],
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname],
				isnull(pj_pop3_email_from,'') [pj_pop3_email_from]
				from bugs
				inner join users on us_id = @us
				left outer join projects on bg_project = pj_id
				where bg_id = @bg");

                    sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
                    sql = sql.AddParameterWithValue("bg", string_bg_id);

                    dr = btnet.DbUtil.get_datarow(sql);

                    load_from_dropdown(dr, false); // list the user's email first, then the project

                    back_href.HRef = "edit_bug.aspx?id=" + string_bg_id;
                    bg_id.Value = string_bg_id;

                    if (request_to != null)
                    {
                        to.Value = request_to;
                    }

                    // Work around for a mysterious bug:
                    // http://sourceforge.net/tracker/?func=detail&aid=2815733&group_id=66812&atid=515837
                    if (btnet.Util.get_setting("StripDisplayNameFromEmailAddress", "0") == "1")
                    {
                        to.Value = Email.simplify_email_address(to.Value);
                    }

                    if (dr["us_signature"].ToString() != "")
                    {
                        if (User.Identity.GetUseFCKEditor())
                        {
                            body.Value += "<br><br><br>";
                            body.Value += dr["us_signature"].ToString().Replace("\r\n", "<br>");
                        }
                        else
                        {
                            body.Value += "\n\n\n";
                            body.Value += dr["us_signature"].ToString();
                        }
                    }


                }

                short_desc.Value = (string)dr["bg_short_desc"];

                if (string_bp_id != null || string_bg_id != null)
                {

                    subject.Value = (string)dr["bg_short_desc"]
                        + "  (" + btnet.Util.get_setting("TrackingIdString", "DO NOT EDIT THIS:")
                        + bg_id.Value
                        + ")";

                    // for determining which users to show in "address book"
                    project = (int)dr["bg_project"];

                }
            }
            else
            {
                on_update();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void load_from_dropdown(DataRow dr, bool project_first)
        {
            // format from dropdown
            string project_email = dr["pj_pop3_email_from"].ToString();
            string us_email = dr["us_email"].ToString();
            string us_firstname = dr["us_firstname"].ToString();
            string us_lastname = dr["us_lastname"].ToString();

            if (project_first)
            {
                if (project_email != "")
                {
                    from.Items.Add(new ListItem(project_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + project_email + ">"));
                    }
                }

                if (us_email != "")
                {
                    from.Items.Add(new ListItem(us_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + us_email + ">"));
                    }
                }
            }
            else
            {
                if (us_email != "")
                {
                    from.Items.Add(new ListItem(us_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + us_email + ">"));
                    }
                }

                if (project_email != "")
                {
                    from.Items.Add(new ListItem(project_email));
                    if (us_firstname != "" && us_lastname != "")
                    {
                        from.Items.Add(new ListItem("\"" + us_firstname + " " + us_lastname + "\" <" + project_email + ">"));
                    }
                }
            }

            if (from.Items.Count == 0)
            {
                from.Items.Add(new ListItem("[none]"));
            }

        }

        ///////////////////////////////////////////////////////////////////////
        bool validate()
        {

            Boolean good = true;


            if (to.Value == "")
            {
                good = false;
                to_err.InnerText = "\"To\" is required.";
            }
            else
            {
                try
                {
                    System.Net.Mail.MailMessage dummy_msg = new System.Net.Mail.MailMessage();
                    Email.add_addresses_to_email(dummy_msg, to.Value, Email.AddrType.to);
                    to_err.InnerText = "";
                }
                catch
                {
                    good = false;
                    to_err.InnerText = "\"To\" is not in a valid format. Separate multiple addresses with commas.";
                }

            }

            if (cc.Value != "")
            {
                try
                {
                    System.Net.Mail.MailMessage dummy_msg = new System.Net.Mail.MailMessage();
                    Email.add_addresses_to_email(dummy_msg, cc.Value, Email.AddrType.cc);
                    cc_err.InnerText = "";
                }
                catch
                {
                    good = false;
                    cc_err.InnerText = "\"CC\" is not in a valid format. Separate multiple addresses with commas.";
                }
            }

            if (from.SelectedItem.Value == "[none]")
            {
                good = false;
                from_err.InnerText = "\"From\" is required.  Use \"settings\" to fix.";
            }
            else
            {
                from_err.InnerText = "";
            }

            if (subject.Value == "")
            {
                good = false;
                subject_err.InnerText = "\"Subject\" is required.";
            }
            else
            {
                subject_err.InnerText = "";
            }

            msg.InnerText = "Email was not sent.";

            return good;

        }

        ///////////////////////////////////////////////////////////////////////
        string get_bug_text(int bugid)
        {
            // Get bug html

            DataRow bug_dr = btnet.Bug.get_bug_datarow(bugid, User.Identity);

            // Create a fake response and let the code
            // write the html to that response
            System.IO.StringWriter writer = new System.IO.StringWriter();
            HttpResponse my_response = new HttpResponse(writer);
            PrintBug.print_bug(my_response,
                bug_dr,
                security,
                true,  // include style
                false, // images_inline
                true,  // history_inline
                include_internal_posts.Checked); // internal_posts

            return writer.ToString();
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            if (!validate()) return;

            sql = new SQLString(@"
insert into bug_posts
	(bp_bug, bp_user, bp_date, bp_comment, bp_comment_search, bp_email_from, bp_email_to, bp_type, bp_content_type, bp_email_cc)
	values(@id, @us, getdate(), @cm, @cs, @fr,  @to, 'sent', @ct, @cc);
select scope_identity()
update bugs set
	bg_last_updated_user = @us,
	bg_last_updated_date = getdate()
	where bg_id = @id");

            sql = sql.AddParameterWithValue("id", bg_id.Value);
            sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
            if (User.Identity.GetUseFCKEditor())
            {
                string adjusted_body = "Subject: " + subject.Value + "<br><br>";
                adjusted_body += btnet.Util.strip_dangerous_tags(body.Value);

                sql = sql.AddParameterWithValue("cm", adjusted_body);
                sql = sql.AddParameterWithValue("cs", adjusted_body);
                sql = sql.AddParameterWithValue("ct", "text/html");
            }
            else
            {
                string adjusted_body = "Subject: " + subject.Value + "\n\n";
                adjusted_body += HttpUtility.HtmlDecode(body.Value);

                sql = sql.AddParameterWithValue("cm", adjusted_body);
                sql = sql.AddParameterWithValue("cs", adjusted_body);
                sql = sql.AddParameterWithValue("ct", "text/plain");
            }
            sql = sql.AddParameterWithValue("fr", from.SelectedItem.Value);
            sql = sql.AddParameterWithValue("to", to.Value);
            sql = sql.AddParameterWithValue("cc", cc.Value);

            int comment_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

            int[] attachments = handle_attachments(comment_id);

            string body_text;
            MailFormat format;
            MailPriority priority;

            switch (prior.SelectedItem.Value)
            {
                case "High":
                    priority = MailPriority.High;
                    break;
                case "Low":
                    priority = MailPriority.Low;
                    break;
                default:
                    priority = MailPriority.Normal;
                    break;
            }

            if (include_bug.Checked)
            {

                // white space isn't handled well, I guess.
                if (User.Identity.GetUseFCKEditor())
                {
                    body_text = body.Value;
                    body_text += "<br><br>";
                }
                else
                {
                    body_text = body.Value.Replace("\n", "<br>");
                    body_text = body_text.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                    body_text = body_text.Replace("  ", "&nbsp; ");
                }
                body_text += "<hr>" + get_bug_text(Convert.ToInt32(bg_id.Value));

                format = MailFormat.Html;
            }
            else
            {
                if (User.Identity.GetUseFCKEditor())
                {
                    body_text = body.Value;
                    format = MailFormat.Html;
                }
                else
                {
                    body_text = HttpUtility.HtmlDecode(body.Value);
                    //body_text = body_text.Replace("\n","\r\n");
                    format = MailFormat.Text;
                }
            }

            string result = Email.send_email( // 9 args
                to.Value,
                from.SelectedItem.Value,
                cc.Value,
                subject.Value,
                body_text,
                format,
                priority,
                attachments,
                return_receipt.Checked);

            btnet.Bug.send_notifications(btnet.Bug.UPDATE, Convert.ToInt32(bg_id.Value), User.Identity);
            btnet.WhatsNew.add_news(Convert.ToInt32(bg_id.Value), short_desc.Value, "email sent", User.Identity);

            if (result == "")
            {
                Response.Redirect("edit_bug.aspx?id=" + bg_id.Value);
            }
            else
            {
                msg.InnerText = result;
            }

        }


        ///////////////////////////////////////////////////////////////////////
        int[] handle_attachments(int comment_id)
        {
            ArrayList attachments = new ArrayList();

            string filename = System.IO.Path.GetFileName(attached_file.PostedFile.FileName);
            if (filename != "")
            {
                //add attachment
                int max_upload_size = Convert.ToInt32(btnet.Util.get_setting("MaxUploadSize", "100000"));
                int content_length = attached_file.PostedFile.ContentLength;
                if (content_length > max_upload_size)
                {
                    msg.InnerText = "File exceeds maximum allowed length of "
                    + Convert.ToString(max_upload_size)
                    + ".";
                    return null;
                }

                if (content_length == 0)
                {
                    msg.InnerText = "No data was uploaded.";
                    return null;
                }

                int bp_id = Bug.insert_post_attachment(
                    User.Identity,
                    Convert.ToInt32(bg_id.Value),
                    attached_file.PostedFile.InputStream,
                    content_length,
                    filename,
                    "email attachment",
                    attached_file.PostedFile.ContentType,
                    comment_id,
                    false, false);

                attachments.Add(bp_id);
            }

            //attachments to forward

            foreach (ListItem item_attachment in lstAttachments.Items)
            {
                if (item_attachment.Selected)
                {
                    int bp_id = Convert.ToInt32(item_attachment.Value);

                    Bug.insert_post_attachment_copy(User.Identity, Convert.ToInt32(bg_id.Value), bp_id, "email attachment", comment_id, false, false);
                    attachments.Add(bp_id);
                }
            }

            return (int[])attachments.ToArray(typeof(int));
        }
    }
}
