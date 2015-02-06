using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class svn_revisions
    {
        public int svnrev_id { get; set; }
        public int svnrev_revision { get; set; }
        public int svnrev_bug { get; set; }
        public string svnrev_repository { get; set; }
        public string svnrev_author { get; set; }
        public string svnrev_svn_date { get; set; }
        public System.DateTime svnrev_btnet_date { get; set; }
        public string svnrev_msg { get; set; }
    }
}
