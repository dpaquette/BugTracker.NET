using btnet.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class delete_comment : BasePage
    {


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

            if (IsPostBack)
            {
                // do delete here

                sql = new SQLString(@"delete bug_posts where bp_id = @bpid");
                sql = sql.AddParameterWithValue("bpid", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Response.Redirect("edit_bug.aspx?id=" + Util.sanitize_integer(redirect_bugid.Value));
            }
            else
            {

                string bug_id = Util.sanitize_integer(Request["bug_id"]);
                redirect_bugid.Value = bug_id;

                int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(bug_id), User.Identity);
                if (permission_level != PermissionLevel.All)
                {
                    Response.Write("You are not allowed to edit this item");
                    Response.End();
                }

                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete comment";

                string id = Util.sanitize_integer(Request["id"]);

                back_href.HRef = "edit_bug.aspx?id=" + bug_id;

                sql = new SQLString(@"select bp_comment from bug_posts where bp_id = @bpid");
                sql = sql.AddParameterWithValue("bpid", id);

                DataRow dr = DbUtil.get_datarow(sql);

                // show the first few chars of the comment
                string s = Convert.ToString(dr["bp_comment"]);
                int len = 20;
                if (s.Length < len) { len = s.Length; }

                confirm_href.InnerText = "confirm delete of comment: "
                        + s.Substring(0, len)
                        + "...";

                row_id.Value = id;
            }


        }
    }
}
