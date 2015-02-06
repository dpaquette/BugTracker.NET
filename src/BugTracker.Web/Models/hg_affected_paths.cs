using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class hg_affected_paths
    {
        public int hgap_id { get; set; }
        public int hgap_hgrev_id { get; set; }
        public string hgap_action { get; set; }
        public string hgap_path { get; set; }
    }
}
