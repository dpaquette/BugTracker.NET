using System;
using System.Data;
using System.Linq;
using btnet.Models;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class projects : BasePage
    {
        protected Project[] Projects;

        protected void Page_Load(Object sender, EventArgs e)
        {            
            Util.do_not_cache(Response);
            
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "projects";
            using (Context context = new Context())
            {
                Projects = context.Projects.OrderBy(p => p.Name).ToArray();
            }           
        }



    }
}
