using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin, BtnetRoles.ProjectAdmin)]
    public partial class delete_user : BasePage
    {
        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            string id = Util.sanitize_integer(Request["id"]);

            if (!User.IsInRole(BtnetRoles.Admin))
            {
                sql = new SQLString(@"select us_created_user, us_admin from users where us_id = @us");
                sql = sql.AddParameterWithValue("us", id);
                DataRow dr = DbUtil.get_datarow(sql);

                if (User.Identity.GetUserId() != (int)dr["us_created_user"])
                {
                    Response.Write("You not allowed to delete this user, because you didn't create it.");
                    Response.End();
                }
                else if ((int)dr["us_admin"] == 1)
                {
                    Response.Write("You not allowed to delete this user, because it is an admin.");
                    Response.End();
                }
            }

            if (IsPostBack)
            {
                // do delete here
                sql = new SQLString(@"
delete from emailed_links where el_username in (select us_username from users where us_id = @us)
delete users where us_id = @us
delete project_user_xref where pu_user = @us
delete bug_subscriptions where bs_user = @us
delete bug_user where bu_user = @us
delete queries where qu_user = @us
delete queued_notifications where qn_user = @us
delete dashboard_items where ds_user = @us");

                sql = sql.AddParameterWithValue("us", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("users.aspx");
            }
            else
            {
                Page.Header.Title= Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete user";

                sql = new SQLString(@"declare @cnt int
select @cnt = count(1) from bugs where bg_reported_user = @us or bg_assigned_to_user = @us
if @cnt = 0
begin
	select @cnt = count(1) from bug_posts where bp_user = @us
end
select us_username, @cnt [cnt] from users where us_id = @us");


                sql = sql.AddParameterWithValue("us", id);

                DataRow dr = DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete user \""
                        + Convert.ToString(dr["us_username"])
                        + "\" because some bugs or bug posts still reference it.");
                    Response.End();
                }
                else
                {

                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["us_username"])
                        + "\"";

                    row_id.Value = id;

                }
            }

        }
    }
}
