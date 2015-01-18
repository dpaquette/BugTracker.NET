using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    public partial class delete_report : BasePage
    {



        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanEditReports())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }
            SQLString sql;
            if (IsPostBack)
            {
                // do delete here
                sql = new SQLString(@"
delete reports where rp_id = @reportId;
delete dashboard_items where ds_report = @reportId");
                sql = sql.AddParameterWithValue("reportId", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("reports.aspx");
            }
            else
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete report";

                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"select rp_desc from reports where rp_id = @id");
                sql = sql.AddParameterWithValue("id", id);

                DataRow dr = DbUtil.get_datarow(sql);

                confirm_href.InnerText = "confirm delete of report: "
                        + Convert.ToString(dr["rp_desc"]);

                row_id.Value = id;

            }

        }

    }
}
