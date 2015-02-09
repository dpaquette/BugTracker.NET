using System;
using System.Linq;
using btnet.Models;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin, BtnetRoles.ProjectAdmin)]
    public partial class categories : BasePage
    {

        protected IEnumerable<Category> _categories;

        protected void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);


            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "categories";

            using (var context = new Context())
            {
                _categories = context.Categories.OrderBy(x => x.Name).ToList();
            }

        }

    }
}
