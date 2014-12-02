using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin, BtnetRoles.ProjectAdmin)]
    public partial class delete_user : BasePage
    {
    }
}
