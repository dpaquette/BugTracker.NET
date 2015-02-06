using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class bug_posts
    {
        public int bp_id { get; set; }
        public int bp_bug { get; set; }
        public string bp_type { get; set; }
        public int bp_user { get; set; }
        public System.DateTime bp_date { get; set; }
        public string bp_comment { get; set; }
        public string bp_comment_search { get; set; }
        public string bp_email_from { get; set; }
        public string bp_email_to { get; set; }
        public string bp_file { get; set; }
        public Nullable<int> bp_size { get; set; }
        public string bp_content_type { get; set; }
        public Nullable<int> bp_parent { get; set; }
        public Nullable<int> bp_original_comment_id { get; set; }
        public int bp_hidden_from_external_users { get; set; }
        public string bp_email_cc { get; set; }
    }
}
