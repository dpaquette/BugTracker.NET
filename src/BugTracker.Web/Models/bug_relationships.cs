using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class bug_relationships
    {
        public int re_id { get; set; }
        public int re_bug1 { get; set; }
        public int re_bug2 { get; set; }
        public string re_type { get; set; }
        public int re_direction { get; set; }
    }
}
