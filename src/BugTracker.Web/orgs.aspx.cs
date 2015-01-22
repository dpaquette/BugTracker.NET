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
    public partial class orgs : BasePage
    {


        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "organizations";

            ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select og_id [id],
		'<a href=edit_org.aspx?id=' + convert(varchar,og_id) + '>edit</a>' [$no_sort_edit],
		'<a href=delete_org.aspx?id=' + convert(varchar,og_id) + '>delete</a>' [$no_sort_delete],
		og_name[desc],
		case when og_active = 1 then 'Y' else 'N' end [active],
		case when og_can_search = 1 then 'Y' else 'N' end [can<br>search],
		case when og_non_admins_can_use = 1 then 'Y' else 'N' end [non-admin<br>can use],
		case when og_can_only_see_own_reported = 1 then 'Y' else 'N' end [can see<br>only own bugs],
		case
			when og_other_orgs_permission_level = 0 then 'None'
			when og_other_orgs_permission_level = 1 then 'Read Only'
			else 'Add/Edit' end [other orgs<br>permission<br>level],
		case when og_external_user = 1 then 'Y' else 'N' end [external],
		case when og_can_be_assigned_to = 1 then 'Y' else 'N' end [can<br>be assigned to],
		case
			when og_status_field_permission_level = 0 then 'None'
			when og_status_field_permission_level = 1 then 'Read Only'
			else 'Add/Edit' end [status<br>permission<br>level],
		case
			when og_assigned_to_field_permission_level = 0 then 'None'
			when og_assigned_to_field_permission_level = 1 then 'Read Only'
			else 'Add/Edit' end [assigned to<br>permission<br>level],
		case
			when og_priority_field_permission_level = 0 then 'None'
			when og_priority_field_permission_level = 1 then 'Read Only'
			else 'Add/Edit' end [priority<br>permission<br>level],
		isnull(og_domain,'')[domain]
		from orgs order by og_name"));

        }


    }
}
