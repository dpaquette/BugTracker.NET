using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;
using System.Data;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin, BtnetRoles.ProjectAdmin)]
    public partial class categories : BasePage
    {

        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);


            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "categories";

            ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select
		ct_id [id],
		ct_name [category],
		ct_sort_seq [sort seq],
		case when ct_default = 1 then 'Y' else 'N' end [default],
		ct_id [hidden]
		from categories order by ct_name"));

        }

    }
}
