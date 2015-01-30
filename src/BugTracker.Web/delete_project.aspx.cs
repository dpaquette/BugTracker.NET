using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;
using System.Data;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_project : BasePage
    {
        SQLString sql;
        protected internal void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected internal void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (IsPostBack)
            {
                // do delete here
                sql = new SQLString(@"delete projects where pj_id = @projectId");
                sql = sql.AddParameterWithValue("projectId", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("projects.aspx");
            }
            else
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete project";

                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from bugs where bg_project = @projectId
			select pj_name, @cnt [cnt] from projects where pj_id = @projectId");
                sql = sql.AddParameterWithValue("projectId", id);

                DataRow dr = DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete project \""
                        + Convert.ToString(dr["pj_name"])
                        + "\" because some bugs still reference it.");
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["pj_name"])
                        + "\"";

                    row_id.Value = id;

                }

            }

        }
    }
}
