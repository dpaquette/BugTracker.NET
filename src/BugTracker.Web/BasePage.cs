using System;
using System.Linq;
using System.Web.UI;

namespace btnet
{
    public class BasePage : Page
    {
        protected override void OnLoad(EventArgs e)
        {            
            if (!IsUserAuthorized())
            {
                Response.Redirect("default.aspx");
            }
            base.OnLoad(e);
        }

        public bool IsUserAuthorized()
        {
            return AllowAnonymous || 
                (User.Identity.IsAuthenticated && AuthorizedRoles.Any(role => User.IsInRole(role)));
        }

        public virtual string[] AuthorizedRoles { get { return new[] {BtnetRoles.User}; } }

        public virtual bool AllowAnonymous { get { return false; } }
    }
}