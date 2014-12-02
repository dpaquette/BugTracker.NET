using System;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    [PageAuthorize(BtnetRoles.User)]
    public class BasePage : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            if (!IsUserAuthorized())
            {
                Response.Redirect(string.Format("default.aspx?returnUrl={0}", HttpUtility.UrlEncode(Request.RawUrl)));
            }
            base.OnPreInit(e);
        }
        
        private bool IsUserAuthorized()
        {
            bool hasAnonymous = Attribute.GetCustomAttributes(GetType(), typeof (PageAllowAnonymous)).Any();
            if (hasAnonymous)
            {
                return true;
            }

            var attributes = Attribute.GetCustomAttributes(GetType(), typeof (PageAuthorizeAttribute)).Cast<PageAuthorizeAttribute>();
            return Page.User.Identity.IsAuthenticated && attributes.All(a => a.OnAuthorize(Page.User));
        }
    }
}