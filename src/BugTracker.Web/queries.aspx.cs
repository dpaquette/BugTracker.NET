using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using btnet.Models;
using btnet.Security;

namespace btnet
{
    public partial class queries : BasePage
    {

        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "queries";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - queries";

            SQLString sql;

            if (User.IsInRole(BtnetRoles.Admin))
            {
                // allow admin to view all queries

                sql = new SQLString(@"select
            qu_id [id],
			qu_desc [query],
			case
				when isnull(qu_user,0) = 0 and isnull(qu_org,0) is null then 'everybody'
				when isnull(qu_user,0) <> 0 then 'user:' + us_username
				when isnull(qu_org,0) <> 0 then 'org:' + og_name
				else ' '
				end [visibility]
			from queries
			left outer join users on qu_user = us_id
			left outer join orgs on qu_org = og_id
			or isnull(qu_user,0) = @us
			or isnull(qu_user,0) = 0
			order by qu_desc");

            }
            else
            {
                // allow editing for users' own queries

                sql = new SQLString(@"select
            qu_id [id],
			qu_desc [query],
            '' [visibility]
			from queries
			inner join users on qu_user = us_id
			where isnull(qu_user,0) = @us
			order by qu_desc");
            }

            sql = sql.AddParameterWithValue("us", User.Identity.GetUserId());
            ds = btnet.DbUtil.get_dataset(sql);

        }

    }
}
