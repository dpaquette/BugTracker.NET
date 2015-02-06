using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class dashboard_items
    {
        public int ds_id { get; set; }
        public int ds_user { get; set; }
        public int ds_report { get; set; }
        public string ds_chart_type { get; set; }
        public int ds_col { get; set; }
        public int ds_row { get; set; }
    }
}
