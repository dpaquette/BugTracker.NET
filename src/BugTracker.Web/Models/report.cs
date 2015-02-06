using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string SQL { get; set; }
        public string ChartType { get; set; }
    }
}
