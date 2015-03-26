using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Models;

namespace btnet
{
    public partial class print_bugs2 : BasePage
    {
        protected SQLString sql;

        protected DataSet ds = null;
        protected DataView dv = null;
        protected bool images_inline;
        protected bool history_inline;

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "print " + Util.get_setting("PluralBugLabel", "bugs");


            // are we doing the query to get the bugs or are we using the cached dataview?
            int queryId = Convert.ToInt32(Request["queryId"]);
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string sortBy = Request["sortBy"];
            string sortOrder = Request["sortOrder"];
            BugQueryFilter[] filters = print_bugs.BuildFilter(Request.Params);
            Query query;
            using (Context context = new Context())
            {
                query = context.Queries.Find(queryId);
            }

            BugQueryExecutor executor = new BugQueryExecutor(query);

            BugQueryResult result = executor.ExecuteQuery(User.Identity, start, length, sortBy, sortOrder, filters);


            dv = new DataView(result.Data);
       

            HttpCookie cookie = Request.Cookies["images_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                images_inline = false;
            }
            else
            {
                images_inline = true;
            }

            cookie = Request.Cookies["history_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                history_inline = false;
            }
            else
            {
                history_inline = true;
            }

        }
    }
}
