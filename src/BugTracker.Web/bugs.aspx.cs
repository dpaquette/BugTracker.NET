using System;
using System.Linq;
using btnet.Models;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize]
    public partial class bugs : BasePage
    {
        protected Query _selectedQuery;

        protected void Page_Load(Object sender, EventArgs e)
        {
            Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");
            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + Util.get_setting("PluralBugLabel", "bugs");

            if (!IsPostBack)
            {
                LoadQueryDropdown();

                if (Session["just_did_text_search"] == null)
                {
                    
                }
                else
                {
                    Session["just_did_text_search"] = null;
                    //dv = (DataView)Session["bugs"];
                    //What the heck is this condition?
                }
            }

            LoadQuery();
            BugList.SelectedQuery = _selectedQuery;
        }



        ///////////////////////////////////////////////////////////////////////
        void LoadQuery()
        {
            using (Context context = new Context())
            {
                _selectedQuery = context.Queries.Find(Convert.ToInt32(query.SelectedValue));
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void LoadQueryDropdown()
        {
            IList<Query> queries;
            using (Context context = new Context())
            {
                queries = context.GetQueriesForUser(User.Identity);
            }
            var defaultQuery = queries.FirstOrDefault(q => q.Default == 1);

            query.DataSource = queries;

            query.DataTextField = "Description";
            query.DataValueField = "Id";
            query.SelectedValue = defaultQuery != null ? defaultQuery.Id.ToString() : string.Empty;
            query.DataBind();
        }
    }
}