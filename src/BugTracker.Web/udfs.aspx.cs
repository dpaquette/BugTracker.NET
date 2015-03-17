using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Models;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class udfs : BasePage
    {
        protected DataSet ds;
        protected IEnumerable<UserDefinedAttribute> Attributes;

        protected void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "user defined attribute values";

            using (Context context = new Context())
            {
                Attributes = context.UserDefinedAttributes.OrderBy(u => u.udf_name).ToList();
            }
            ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select udf_id [id],
		udf_name [user defined attribute value],
		udf_sort_seq [sort seq],
		case when udf_default = 1 then 'Y' else 'N' end [default],
		udf_id [hidden]
		from user_defined_attribute order by udf_name"));

        }
    }
}
