using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_task : BasePage
    {

        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            if (Request.QueryString["ses"] != (string)Session["session_cookie"])
            {
                Response.Write("session in URL doesn't match session cookie");
                Response.End();
            }

            string string_bugid = Util.sanitize_integer(Request["bugid"]);
            int bugid = Convert.ToInt32(string_bugid);

            int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);

            if (permission_level != PermissionLevel.All)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }

            string string_tsk_id = Util.sanitize_integer(Request["id"]);
            int tsk_id = Convert.ToInt32(string_tsk_id);

            if (IsPostBack)
            {
                // do delete here

                sql = new SQLString(@"delete bug_tasks where tsk_id = @tsk_id and tsk_bug = @bugid");
                sql = sql.AddParameterWithValue("tsk_id", string_tsk_id);
                sql = sql.AddParameterWithValue("bugid", string_bugid);
                DbUtil.execute_nonquery(sql);
                Response.Redirect("tasks.aspx?bugid=" + string_bugid);
            }
            else
            {


                Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete task";

                back_href.HRef = "tasks.aspx?bugid=" + string_bugid;

                sql = new SQLString(@"select tsk_description from bug_tasks where tsk_id = @tsk_id and tsk_bug = @bugid");
                sql = sql.AddParameterWithValue("tsk_id", string_tsk_id);
                sql = sql.AddParameterWithValue("bugid", string_bugid);

                DataRow dr = DbUtil.get_datarow(sql);

                confirm_href.InnerText = "confirm delete of task: " + Convert.ToString(dr["tsk_description"]);

            }


        }

    }
}
