using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Priority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string BackgroundColor { get; set; }
        public string Style { get; set; }
        public int Default { get; set; }
    }
}
