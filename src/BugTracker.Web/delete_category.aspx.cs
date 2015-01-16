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
    public partial class delete_category : BasePage
    {
protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            SQLString sql;
            if (IsPostBack)
            {
                sql = new SQLString(@"delete categories where ct_id = @catid");
                sql = sql.AddParameterWithValue("catid", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("categories.aspx");
            }
            else
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                     + "delete category";

                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from bugs where bg_category = @ctid
			select ct_name, @cnt [cnt] from categories where ct_id = @ctid");
                sql = sql.AddParameterWithValue("ctid", id);

                DataRow dr = DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete category \""
                        + Convert.ToString(dr["ct_name"])
                        + "\" because some bugs still reference it.");
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["ct_name"])
                        + "\"";

                    row_id.Value = id;
                }
            }

        }
    }
}
