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
    public partial class orgs : BasePage
    {

        protected IEnumerable<Organization> Organizations;
        protected DataSet ds;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "organizations";

            using (Context context = new Context())
            {
                Organizations = context.Organizations.OrderBy(o => o.Name).ToList();
            }
            
        }


    }
}
