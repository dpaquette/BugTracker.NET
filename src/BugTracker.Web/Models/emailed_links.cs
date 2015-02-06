using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class emailed_links
    {
        public string el_id { get; set; }
        public System.DateTime el_date { get; set; }
        public string el_email { get; set; }
        public string el_action { get; set; }
        public string el_username { get; set; }
        public Nullable<int> el_user_id { get; set; }
        public Nullable<int> el_salt { get; set; }
        public string el_password { get; set; }
        public string el_firstname { get; set; }
        public string el_lastname { get; set; }
    }
}
