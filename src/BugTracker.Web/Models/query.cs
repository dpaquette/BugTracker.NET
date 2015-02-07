using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class query
    {
        public int qu_id { get; set; }
        public string qu_desc { get; set; }
        public string qu_sql { get; set; }
        public Nullable<int> qu_default { get; set; }
        public Nullable<int> qu_user { get; set; }
        public Nullable<int> qu_org { get; set; }
    }
}
