using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class hg_revisions
    {
        public int hgrev_id { get; set; }
        public Nullable<int> hgrev_revision { get; set; }
        public int hgrev_bug { get; set; }
        public string hgrev_repository { get; set; }
        public string hgrev_author { get; set; }
        public string hgrev_hg_date { get; set; }
        public System.DateTime hgrev_btnet_date { get; set; }
        public string hgrev_msg { get; set; }
    }
}
