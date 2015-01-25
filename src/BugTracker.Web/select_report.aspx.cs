using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;
using System.Data;

namespace btnet
{
    public partial class select_report : BasePage
    {

        protected DataSet ds;


        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanUseReports())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "reports";

            var sql = new SQLString(@"
select
rp_desc [report],
case
	when rp_chart_type = 'pie' then
		'<a href=''javascript:select_report(""pie"",' + convert(varchar, rp_id) + ')''>select pie</a>'
	when rp_chart_type = 'line' then
		'<a href=''javascript:select_report(""line"",' + convert(varchar, rp_id) + ')''>select line</a>'
	when rp_chart_type = 'bar' then
		'<a href=''javascript:select_report(""bar"",' + convert(varchar, rp_id) + ')''>select bar</a>'
	else
		'&nbsp;' end [chart],
'<a href=''javascript:select_report(""data"",' + convert(varchar, rp_id) + ')''>select data</a>' [data]
from reports order by rp_desc");

            ds = btnet.DbUtil.get_dataset(sql);

        }

    }
}
