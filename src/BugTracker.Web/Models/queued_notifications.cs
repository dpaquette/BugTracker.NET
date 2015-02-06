using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class queued_notifications
    {
        public int qn_id { get; set; }
        public System.DateTime qn_date_created { get; set; }
        public int qn_bug { get; set; }
        public int qn_user { get; set; }
        public string qn_status { get; set; }
        public int qn_retries { get; set; }
        public string qn_last_exception { get; set; }
        public string qn_to { get; set; }
        public string qn_from { get; set; }
        public string qn_subject { get; set; }
        public string qn_body { get; set; }
    }
}
