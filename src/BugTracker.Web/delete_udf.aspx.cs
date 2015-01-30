using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_udf : BasePage
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
                sql = new SQLString(@"delete user_defined_attribute where udf_id = @udfid");
                sql = sql.AddParameterWithValue("udfid", Util.sanitize_integer(row_id.Value));
                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("udfs.aspx");
            }
            else
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete user defined attribute value";

                string id = Util.sanitize_integer(Request["id"]);

                sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from bugs where bg_user_defined_attribute = @udfid
			select udf_name, @cnt [cnt] from user_defined_attribute where udf_id = @udfid");
                sql = sql.AddParameterWithValue("udfid", id);

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete value \""
                        + Convert.ToString(dr["udf_name"])
                        + "\" because some bugs still reference it.");
                    Response.End();
                }
                else
                {
                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["udf_name"])
                        + "\"";

                    row_id.Value = id;
                }

            }

        }

    }
}
