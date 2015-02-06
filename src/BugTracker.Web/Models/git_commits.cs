using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class git_commits
    {
        public int gitcom_id { get; set; }
        public string gitcom_commit { get; set; }
        public int gitcom_bug { get; set; }
        public string gitcom_repository { get; set; }
        public string gitcom_author { get; set; }
        public string gitcom_git_date { get; set; }
        public System.DateTime gitcom_btnet_date { get; set; }
        public string gitcom_msg { get; set; }
    }
}
