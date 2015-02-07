using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class CustomColumnsMetaData
    {
        public int Order { get; set; }
        public string DropDownValues { get; set; }
        public Nullable<int> SortSequence { get; set; }
        public string DropDownType { get; set; }
    }
}
