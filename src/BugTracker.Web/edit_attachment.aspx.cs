using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    public partial class edit_attachment : BasePage
    {

        int id;
        protected int bugid;
        SQLString sql;

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


            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit attachment";

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            id = Convert.ToInt32(var);

            var = Request.QueryString["bug_id"];
            bugid = Convert.ToInt32(var);

            int permission_level = btnet.Bug.get_bug_permission_level(bugid, User.Identity);
            if (permission_level != PermissionLevel.All)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }


            if (User.Identity.GetIsExternalUser() || Util.get_setting("EnableInternalOnlyPosts", "0") == "0")
            {
                internal_only.Visible = false;
                internal_only_label.Visible = false;
            }

            if (!IsPostBack)
            {

                // Get this entry's data from the db and fill in the form

                sql = new SQLString(@"select bp_comment, bp_file, bp_hidden_from_external_users from bug_posts where bp_id = @bugPostId");
                sql = sql.AddParameterWithValue("bugPostId", Convert.ToString(id));
                DataRow dr = btnet.DbUtil.get_datarow(sql);

                // Fill in this form
                desc.Value = (string)dr["bp_comment"];
                filename.InnerText = (string)dr["bp_file"];
                internal_only.Checked = Convert.ToBoolean((int)dr["bp_hidden_from_external_users"]);

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

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {

                sql = new SQLString(@"update bug_posts set
			bp_comment = @comment,
			bp_hidden_from_external_users = @internal
			where bp_id = @bugPostId");

                sql = sql.AddParameterWithValue("bugPostId", Convert.ToString(id));
                sql = sql.AddParameterWithValue("comment", desc.Value.Replace("'", "''"));
                sql = sql.AddParameterWithValue("internal", btnet.Util.bool_to_string(internal_only.Checked));

                btnet.DbUtil.execute_nonquery(sql);

                if (!internal_only.Checked)
                {
                    btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, User.Identity);
                }

                Response.Redirect("edit_bug.aspx?id=" + Convert.ToString(bugid));

            }
            else
            {
                msg.InnerText = "Attachment was not updated.";
            }

        }


    }
}
