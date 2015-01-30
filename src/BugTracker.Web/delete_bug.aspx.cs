using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    public partial class delete_bug : BasePage
    {

        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanDeleteBugs())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            string id = Util.sanitize_integer(Request["id"]);

            int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(id), User.Identity);
            if (permission_level != PermissionLevel.All)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }

            if (IsPostBack)
            {

                Bug.delete_bug(Convert.ToInt32(row_id.Value));
                Server.Transfer("bugs.aspx");

            }
            else
            {

                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete " + Util.get_setting("SingularBugLabel", "bug");

                back_href.HRef = "edit_bug.aspx?id=" + id;

                sql = new SQLString(@"select bg_short_desc from bugs where bg_id = @bugId");
                sql = sql.AddParameterWithValue("bugId", id);

                DataRow dr = DbUtil.get_datarow(sql);

                confirm_href.InnerText = "confirm delete of "
                        + Util.get_setting("SingularBugLabel", "bug")
                        + ": "
                        + Convert.ToString(dr["bg_short_desc"]);

                row_id.Value = id;
            }

        }

    }
}
