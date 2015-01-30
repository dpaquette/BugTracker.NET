using System;
using System.Linq;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class delete_subscriber : BasePage
    {

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            if (Request.QueryString["ses"] != (string)Session["session_cookie"])
            {
                Response.Write("session in URL doesn't match session cookie");
                Response.End();
            }

            var sql = new SQLString("delete from bug_subscriptions where bs_bug = @bg_id and bs_user = @us_id");
            sql = sql.AddParameterWithValue("$bg_id", Util.sanitize_integer(Request["bg_id"]));
            sql = sql.AddParameterWithValue("$us_id", Util.sanitize_integer(Request["us_id"]));
            DbUtil.execute_nonquery(sql);

            Response.Redirect("view_subscribers.aspx?id=" + Util.sanitize_integer(Request["bg_id"]));

        }

    }
}
