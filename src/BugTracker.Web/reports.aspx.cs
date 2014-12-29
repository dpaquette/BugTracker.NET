using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    public partial class reports : BasePage
    {
       protected DataSet ds;
        
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanUseReports() || User.Identity.GetCanEditReports())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "reports";

            SQLString sql;
            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanEditReports())
            {
                sql = new SQLString(@"
select
rp_desc [report],
case
	when rp_chart_type = 'pie' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>pie</a>'
	when rp_chart_type = 'line' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>line</a>'
	when rp_chart_type = 'bar' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>bar</a>'
	else
		'&nbsp;' end [view<br>chart],
'<a target=''_blank'' href=''view_report.aspx?view=data&id=' + convert(varchar, rp_id) + '''>data</a>' [view<br>data],
'<a href=''edit_report.aspx?id=' + convert(varchar, rp_id) + '''>edit</a>' [edit], 
'<a href=''delete_report.aspx?id=' + convert(varchar, rp_id) + '''>delete</a>' [delete]
from reports order by rp_desc");

            }
            else
            {
                sql = new SQLString(@"
select
rp_desc [report],
case
	when rp_chart_type = 'pie' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>pie</a>'
	when rp_chart_type = 'line' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>line</a>'
	when rp_chart_type = 'bar' then
		'<a target=''_blank'' href=''view_report.aspx?view=chart&id=' + convert(varchar, rp_id) + '''>bar</a>'
	else
		'&nbsp;' end [view<br>chart],
'<a target=''_blank'' href=''view_report.aspx?view=data&id=' + convert(varchar, rp_id) + '''>data</a>' [view<br>data]
from reports order by rp_desc");
            }

            ds = btnet.DbUtil.get_dataset(sql);

        }


    }
}
