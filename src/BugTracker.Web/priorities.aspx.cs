using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class priorities : BasePage
    {

        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "priorities";

            ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select pr_id [id],
		pr_name [description],
		pr_sort_seq [sort seq],
		'<div style=''background:' + pr_background_color + ';''>' + pr_background_color + '</div>' [background<br>color],
		pr_style [css<br>class],
		case when pr_default = 1 then 'Y' else 'N' end [default],
		pr_id [hidden] from priorities"));

        }
    }
}
