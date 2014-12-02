using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_queued_notifications : BasePage
    {
    }
}
