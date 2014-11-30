using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Permissions;
using System.Text;
using System.Web;
using btnet.Search;
using Nest;

namespace btnet
{
    public partial class search_text : BasePage
    {

#pragma warning disable 618

        Security security;

        ///////////////////////////////////////////////////////////////////////
        void Page_Load(Object sender, EventArgs e)
        {


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
            var results = search.Search(Request["Query"], User.Identity);

            Session["bugs_unfiltered"] = results.Tables[0];
            Session["bugs"] = new DataView(results.Tables[0]);

            Session["just_did_text_search"] = "yes"; // switch for bugs.aspx
            Session["query"] = Request["query"]; // for util.cs, to persist the text in the search <input>
            Response.Redirect("bugs.aspx");
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
