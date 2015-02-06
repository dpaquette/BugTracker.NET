using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class user
    {
        public int us_id { get; set; }
        public string us_username { get; set; }
        public string us_salt { get; set; }
        public string us_password { get; set; }
        public string us_firstname { get; set; }
        public string us_lastname { get; set; }
        public string us_email { get; set; }
        public int us_admin { get; set; }
        public int us_default_query { get; set; }
        public int us_enable_notifications { get; set; }
        public int us_auto_subscribe { get; set; }
        public Nullable<int> us_auto_subscribe_own_bugs { get; set; }
        public Nullable<int> us_auto_subscribe_reported_bugs { get; set; }
        public Nullable<int> us_send_notifications_to_self { get; set; }
        public int us_active { get; set; }
        public Nullable<int> us_bugs_per_page { get; set; }
        public Nullable<int> us_forced_project { get; set; }
        public int us_reported_notifications { get; set; }
        public int us_assigned_notifications { get; set; }
        public int us_subscribed_notifications { get; set; }
        public string us_signature { get; set; }
        public int us_use_fckeditor { get; set; }
        public int us_enable_bug_list_popups { get; set; }
        public int us_created_user { get; set; }
        public int us_org { get; set; }
        public Nullable<System.DateTime> us_most_recent_login_datetime { get; set; }
        public string password_reset_key { get; set; }
    }
}
