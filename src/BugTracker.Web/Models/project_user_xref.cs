using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class project_user_xref
    {
        public int pu_id { get; set; }
        public int pu_project { get; set; }
        public int pu_user { get; set; }
        public int pu_auto_subscribe { get; set; }
        public int pu_permission_level { get; set; }
        public int pu_admin { get; set; }
    }
}
