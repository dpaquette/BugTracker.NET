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
    public partial class delete_org : BasePage
    {


        SQLString sql;


        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (IsPostBack)
            {
                // do delete here
                sql = new SQLString(@"delete orgs where og_id = @orgid");
                sql = sql.AddParameterWithValue("orgid", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("orgs.aspx");
            }
            else
            {

                Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete organization";

                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from users where us_org = @orgid;
			select @cnt = @cnt + count(1) from queries where qu_org = @orgid;
			select @cnt = @cnt + count(1) from bugs where bg_org = @orgid;
			select og_name, @cnt [cnt] from orgs where og_id = @orgid");
                sql = sql.AddParameterWithValue("orgid", id);

                DataRow dr = DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete organization \""
                        + Convert.ToString(dr["og_name"])
                        + "\" because some bugs, users, queries still reference it.");
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["og_name"])
                        + "\"";

                    row_id.Value = id;

                }

            }

        }

    }
}
