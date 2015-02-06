using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class custom_col_metadata
    {
        public int ccm_colorder { get; set; }
        public string ccm_dropdown_vals { get; set; }
        public Nullable<int> ccm_sort_seq { get; set; }
        public string ccm_dropdown_type { get; set; }
    }
}
