using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class report
    {
        public int rp_id { get; set; }
        public string rp_desc { get; set; }
        public string rp_sql { get; set; }
        public string rp_chart_type { get; set; }
    }
}
