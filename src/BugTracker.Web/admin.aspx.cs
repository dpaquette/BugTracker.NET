using System;
using System.Threading.Tasks;
using System.Web;
using btnet.Search;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class admin : BasePage
    {
        protected bool nag = false;

        ///////////////////////////////////////////////////////////////////////
        public void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "admin";

            if (false) // change this to if(true) to make the donation nag message go away
            {

            }
            else
            {
                int bugs = Convert.ToInt32(btnet.DbUtil.execute_scalar(new SQLString("select count(1) from bugs")));
                if (bugs > 100)
                {
                    nag = true;
                }
            }
        }


        public void ReindexAllBugs(object sender, EventArgs e)
        {
            if (Util.get_setting("EnableSearch", "1") == "1")
            {
                IBugSearch search = BugSearchFactory.CreateBugSearch();
                Task.Run(() => search.IndexAll());
                reindexLink.Enabled = false;
                reindexLink.Text = reindexLink.Text + " (Indexing in process)";
            }
        }
    }
}
