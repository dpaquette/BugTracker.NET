using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class logoff : BasePage
    {
        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);
            Security.Security.SignOut(Request);
            Response.Redirect("default.aspx?msg=logged+off");
        }
    }
}
