using System;
using System.Data;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class statuses : BasePage
    {
        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "statuses";

            ds = btnet.DbUtil.get_dataset(
                new SQLString(@"select st_id [id],
		        st_name [status],
		        st_sort_seq [sort seq],
		        st_style [css<br>class],
		        case when st_default = 1 then 'Y' else 'N' end [default],
		        st_id [hidden]
		        from statuses order by st_sort_seq"));

        }



    }
}
