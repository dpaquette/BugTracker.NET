using System;
using System.Data;
using btnet.Search;

namespace btnet
{
    public partial class search_text : BasePage
    {
        protected DataTable _searchResults;
#pragma warning disable 618

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {
            Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");

            try
            {
                if (string.IsNullOrEmpty(Request["query"]))
                {
                    throw new Exception("You forgot to enter something to search for...");
                }
            }
            catch (Exception e3)
            {
                display_exception(e3);
            }

            var search = BugSearchFactory.CreateBugSearch();
            _searchResults = search.Search(Request["Query"], User.Identity).Tables[0];

            Session["query"] = Request["query"];
        }

        void display_exception(Exception e)
        {
            string s = e.Message;
            if (e.InnerException != null)
            {
                s += "<br>";
                s += e.InnerException.Message;
            }
            Response.Write(@"
<html>
<link rel=StyleSheet href=btnet.css type=text/css>
<p>&nbsp;</p>
<div class=align>
<div class=err>");


            Response.Write(s);

            Response.Write(@"
<p>
<a href='javascript:history.go(-1)'>back</a>            
</div></div>
</html>");

            Response.End();

        }


    }


}
