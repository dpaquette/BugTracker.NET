using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class projects : BasePage
    {
        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {            
            Util.do_not_cache(Response);
            
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "projects";

            ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select
		pj_id [id],
		pj_name [project],
		case when pj_active = 1 then 'Y' else 'N' end [active],
		us_username [default user],
		case when isnull(pj_auto_assign_default_user,0) = 1 then 'Y' else 'N' end [auto assign default user],
		case when isnull(pj_auto_subscribe_default_user,0) = 1 then 'Y' else 'N' end [auto subscribe default user],
		case when isnull(pj_enable_pop3,0) = 1 then 'Y' else 'N' end [receive items via pop3],
		pj_pop3_username [pop3 username],
		pj_pop3_email_from [from email address],
		case when pj_default = 1 then 'Y' else 'N' end [default]
		from projects
		left outer join users on us_id = pj_default_user
		order by pj_name"));

        }



    }
}
