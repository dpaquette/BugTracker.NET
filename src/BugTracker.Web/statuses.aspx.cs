using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using btnet.Models;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class statuses : BasePage
    {
        protected IEnumerable<Status> Statuses;

        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "statuses";

            using (Context context = new Context())
            {
                Statuses = context.Statuses.OrderBy(s => s.SortOrder).ToList();
            }

        }

    }
}
