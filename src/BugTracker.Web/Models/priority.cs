using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class priority
    {
        public int pr_id { get; set; }
        public string pr_name { get; set; }
        public int pr_sort_seq { get; set; }
        public string pr_background_color { get; set; }
        public string pr_style { get; set; }
        public int pr_default { get; set; }
    }
}
