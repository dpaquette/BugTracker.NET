using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Security;
using btnet.Security;

namespace btnet
{
    public partial class edit_report : BasePage
    {

        int id;
        SQLString sql;

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

            Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit report";

            msg.InnerText = "";

            string var = Request.QueryString["id"];
            if (var == null)
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(var);
            }

            if (!IsPostBack)
            {
                // add or edit?
                if (id == 0)
                {
                    sub.Value = "Create";
                    sql_text.Value = Request.Form["sql_text"]; // if coming from search.aspx
                    table.Checked = true;
                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form
                    sql = new SQLString(@"select
				rp_desc, rp_sql, rp_chart_type
				from reports where rp_id = @rpid");
                    sql = sql.AddParameterWithValue("rpid", Convert.ToString(id));
                    DataRow dr = DbUtil.get_datarow(sql);

                    // Fill in this form
                    desc.Value = (string)dr["rp_desc"];
                    sql_text.Value = (string)dr["rp_sql"];

                    switch ((string)dr["rp_chart_type"])
                    {
                        case "pie":
                            pie.Checked = true;
                            break;
                        case "bar":
                            bar.Checked = true;
                            break;
                        case "line":
                            line.Checked = true;
                            break;
                        default:
                            table.Checked = true;
                            break;
                    }
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

            if (desc.Value == "")
            {
                good = false;
                desc_err.InnerText = "Description is required.";
            }
            else
            {
                desc_err.InnerText = "";
            }

            if (sql_text.Value == "")
            {
                good = false;
                msg.InnerText = "The SQL statement is required.  ";
            }
            else
            {
                msg.InnerText = "";
            }

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {
            Boolean good = validate();
            string ct;

            if (good)
            {
                if (id == 0)
                {  // insert new
                    sql = new SQLString(@"insert into reports
				(rp_desc, rp_sql, rp_chart_type)
				values (@de, @sq, @ct)");
                }
                else
                {	// edit existing
                    sql = new SQLString(@"update reports set
				rp_desc = @de,
				rp_sql = @sq,
				rp_chart_type = @ct
				where rp_id = @id");
                    sql = sql.AddParameterWithValue("@id", Convert.ToString(id));
                }

                sql = sql.AddParameterWithValue("@de", desc.Value);
                sql = sql.AddParameterWithValue("@sq", Server.HtmlDecode(sql_text.Value));

                if (pie.Checked)
                {
                    ct = "pie";
                }
                else if (bar.Checked)
                {
                    ct = "bar";
                }
                else if (line.Checked)
                {
                    ct = "line";
                }
                else
                {
                    ct = "table";
                }

                sql = sql.AddParameterWithValue("@ct", ct);

                DbUtil.execute_nonquery(sql);
                Server.Transfer("reports.aspx");
            }
            else
            {
                if (id == 0)
                {  // insert new
                    msg.InnerText += "Query was not created.";
                }
                else
                {	// edit existing
                    msg.InnerText += "Query was not updated.";
                }
            }
        }
    }
}
