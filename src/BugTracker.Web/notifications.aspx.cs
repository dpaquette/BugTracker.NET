using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class notifications : BasePage
    {

        protected DataSet ds;

        protected string ses;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "queued notifications";

            ds = btnet.DbUtil.get_dataset(
                new SQLString(@"select
		qn_id [id],
		qn_date_created [date created],
		qn_to [to],
		qn_bug [bug],
		qn_status [status],
		qn_retries [retries],
		qn_last_exception [last error]
		from queued_notifications
		order by id;"));

            ses = (string)Session["session_cookie"];
        }


    }
}
