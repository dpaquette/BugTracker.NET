using btnet.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class edit_comment : BasePage
    {

        int id;
        SQLString sql;


        protected bool use_fckeditor = false;
        protected int bugid;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {
            Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");
            Util.do_not_cache(Response);


            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanEditAndDeletePosts())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit comment";

            msg.InnerText = "";

            id = Convert.ToInt32(Request["id"]);

            if (!IsPostBack)
            {
                sql = new SQLString(@"select bp_comment, bp_type,
        isnull(bp_comment_search,bp_comment) bp_comment_search,
        isnull(bp_content_type,'') bp_content_type,
        bp_bug, bp_hidden_from_external_users
        from bug_posts where bp_id = @id");
            }
            else
            {
                sql = new SQLString(@"select bp_bug, bp_type,
        isnull(bp_content_type,'') bp_content_type,
        bp_hidden_from_external_users
        from bug_posts where bp_id = @id");
            }

            sql = sql.AddParameterWithValue("id", Convert.ToString(id));
            DataRow dr = DbUtil.get_datarow(sql);

            bugid = (int)dr["bp_bug"];

            int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
            if (permission_level == PermissionLevel.None
            || permission_level == PermissionLevel.ReadOnly
            || (string)dr["bp_type"] != "comment")
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }

            string content_type = (string)dr["bp_content_type"];

            if (User.Identity.GetUseFCKEditor() && content_type == "text/html" && Util.get_setting("DisableFCKEditor", "0") == "0")
            {
                use_fckeditor = true;
            }
            else
            {
                use_fckeditor = false;
            }

            if (User.Identity.GetIsExternalUser() || Util.get_setting("EnableInternalOnlyPosts", "0") == "0")
            {
                internal_only.Visible = false;
                internal_only_label.Visible = false;
            }

            if (!IsPostBack)
            {
                internal_only.Checked = Convert.ToBoolean((int)dr["bp_hidden_from_external_users"]);

                if (use_fckeditor)
                {
                    comment.Value = (string)dr["bp_comment"];
                }
                else
                {
                    comment.Value = (string)dr["bp_comment_search"];
                }
            }
            else
            {
                on_update();
            }

        }


        ///////////////////////////////////////////////////////////////////////
        Boolean validate()
        {

            Boolean good = true;

            if (comment.Value.Length == 0)
            {
                msg.InnerText = "Comment cannot be blank.";
                return false;
            }

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {

                sql = new SQLString(@"update bug_posts set
                    bp_comment = @cm,
                    bp_comment_search = @cs,
                    bp_content_type = @cn,
                    bp_hidden_from_external_users = @internal
                where bp_id = @id

                select bg_short_desc from bugs where bg_id = @bugid");

                if (use_fckeditor)
                {
                    string text = Util.strip_dangerous_tags(comment.Value);
                    sql = sql.AddParameterWithValue("cm", text.Replace("'", "&#39;"));
                    sql = sql.AddParameterWithValue("cs", Util.strip_html(comment.Value).Replace("'", "''"));
                    sql = sql.AddParameterWithValue("cn", "text/html");
                }
                else
                {
                    sql = sql.AddParameterWithValue("cm", HttpUtility.HtmlDecode(comment.Value).Replace("'", "''"));
                    sql = sql.AddParameterWithValue("cs", comment.Value.Replace("'", "''"));
                    sql = sql.AddParameterWithValue("cn", "text/plain");
                }

                sql = sql.AddParameterWithValue("id", Convert.ToString(id));
                sql = sql.AddParameterWithValue("bugid", Convert.ToString(bugid));
                sql = sql.AddParameterWithValue("internal", Util.bool_to_string(internal_only.Checked));
                DataRow dr = DbUtil.get_datarow(sql);

                // Don't send notifications for internal only comments.
                // We aren't putting them the email notifications because it that makes it
                // easier for them to accidently get forwarded to the "wrong" people...
                if (!internal_only.Checked)
                {
                    Bug.send_notifications(Bug.UPDATE, bugid, User.Identity);
                    WhatsNew.add_news(bugid, (string)dr["bg_short_desc"], "updated", User.Identity);
                }


                Response.Redirect("edit_bug.aspx?id=" + Convert.ToString(bugid));

            }

        }

    }
}
