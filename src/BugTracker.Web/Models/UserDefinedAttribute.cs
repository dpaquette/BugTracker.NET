using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class UserDefinedAttribute
    {
        public int udf_id { get; set; }
        public string udf_name { get; set; }
        public int udf_sort_seq { get; set; }
        public int udf_default { get; set; }
    }
}
