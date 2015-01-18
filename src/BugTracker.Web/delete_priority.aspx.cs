using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_priority : BasePage
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
                sql = new SQLString(@"delete priorities where pr_id = @prid");
                sql = sql.AddParameterWithValue("prid", Util.sanitize_integer(row_id.Value));
                DbUtil.execute_nonquery(sql);
                Server.Transfer("priorities.aspx");
            }
            else
            {

                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete priority";

                string id = Util.sanitize_integer(Request["id"]);


                sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from bugs where bg_priority = @id
			select pr_name, @cnt [cnt] from priorities where pr_id = @id");
                sql = sql.AddParameterWithValue("id", id);

                DataRow dr = DbUtil.get_datarow(sql);

                if ((int)dr["cnt"] > 0)
                {
                    Response.Write("You can't delete priority \""
                        + Convert.ToString(dr["pr_name"])
                        + "\" because some bugs still reference it.");
                    Response.End();
                }
                else
                {

                    confirm_href.InnerText = "confirm delete of \""
                        + Convert.ToString(dr["pr_name"])
                        + "\"";

                    row_id.Value = id;

                }

            }

        }

    }
}
