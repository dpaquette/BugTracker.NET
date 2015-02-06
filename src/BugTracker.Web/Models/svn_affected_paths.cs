using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class svn_affected_paths
    {
        public int svnap_id { get; set; }
        public int svnap_svnrev_id { get; set; }
        public string svnap_action { get; set; }
        public string svnap_path { get; set; }
    }
}
