using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class backup_db : BasePage
    {
        public override string[] AuthorizedRoles
        {
            get { return new[] { BtnetRoles.Admin}; }
        }

    }
}
