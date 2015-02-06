using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class git_affected_paths
    {
        public int gitap_id { get; set; }
        public int gitap_gitcom_id { get; set; }
        public string gitap_action { get; set; }
        public string gitap_path { get; set; }
    }
}
