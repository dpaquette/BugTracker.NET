using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class category
    {
        public int ct_id { get; set; }
        public string ct_name { get; set; }
        public int ct_sort_seq { get; set; }
        public int ct_default { get; set; }
    }
}
