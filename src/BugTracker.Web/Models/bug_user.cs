using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class bug_user
    {
        public int bu_bug { get; set; }
        public int bu_user { get; set; }
        public int bu_flag { get; set; }
        public Nullable<System.DateTime> bu_flag_datetime { get; set; }
        public int bu_seen { get; set; }
        public Nullable<System.DateTime> bu_seen_datetime { get; set; }
        public int bu_vote { get; set; }
        public Nullable<System.DateTime> bu_vote_datetime { get; set; }
    }
}
