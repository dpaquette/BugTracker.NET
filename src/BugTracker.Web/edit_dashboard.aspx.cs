using System;
using System.Data;
using System.Linq;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    public partial class edit_dashboard : BasePage
    {

        DataSet ds = null;
        protected string ses = "";

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit dashboard";

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanUseReports())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }


            ses = (string)Session.SessionID;

            var sql = new SQLString(@"
select ds_id, ds_col, ds_row, ds_chart_type, rp_desc
from dashboard_items ds
inner join reports on rp_id = ds_report
where ds_user = @user
order by ds_col, ds_row");

            sql = sql.AddParameterWithValue("user", Convert.ToString(User.Identity.GetUserId()));

            ds = DbUtil.get_dataset(sql);

        }

        protected void write_link(int id, string action, string text)
        {

            Response.Write("<a href=update_dashboard.aspx?actn=");
            Response.Write(action);
            Response.Write("&ds_id=");
            Response.Write(Convert.ToString(id));
            Response.Write("&ses=");
            Response.Write(ses);
            Response.Write(">[");
            Response.Write(text);
            Response.Write("]</a>&nbsp;&nbsp;&nbsp;");

        }

        protected void write_column(int col)
        {

            bool first_row = true;
            int last_row = -1;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                if ((int)dr["ds_col"] == col)
                {
                    last_row = (int)dr["ds_row"];
                }
            }


            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                if ((int)dr["ds_col"] == col)
                {
                    Response.Write("<div class=panel>");

                    write_link((int)dr["ds_id"], "delete", "delete");

                    if (first_row)
                    {
                        first_row = false;
                    }
                    else
                    {
                        write_link((int)dr["ds_id"], "moveup", "move up");
                    }

                    if ((int)dr["ds_row"] == last_row)
                    {
                        // skip
                    }
                    else
                    {
                        write_link((int)dr["ds_id"], "movedown", "move down");
                    }

                    //write_link((int) dr["ds_id"], "switchcols", "switch columns");

                    Response.Write("<p><div style='text-align: center; font-weight: bold;'>");
                    Response.Write((string)dr["rp_desc"] + "&nbsp;-&nbsp; " + (string)dr["ds_chart_type"]);
                    Response.Write("</div>");

                    Response.Write("</div>");
                }
            }

        }
    }
}
