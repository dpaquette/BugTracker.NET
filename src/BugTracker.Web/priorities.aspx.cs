using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using btnet.Models;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class priorities : BasePage
    {

        protected DataSet ds;
        protected IEnumerable<Priority> Priorities;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";
            
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "priorities";
            using (Context context = new Context())
            {
                Priorities = context.Priorities.OrderBy(p => p.Name).ToArray();
            }      
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
