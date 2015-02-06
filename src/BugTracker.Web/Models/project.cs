using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class project
    {
        public int pj_id { get; set; }
        public string pj_name { get; set; }
        public int pj_active { get; set; }
        public Nullable<int> pj_default_user { get; set; }
        public Nullable<int> pj_auto_assign_default_user { get; set; }
        public Nullable<int> pj_auto_subscribe_default_user { get; set; }
        public Nullable<int> pj_enable_pop3 { get; set; }
        public string pj_pop3_username { get; set; }
        public string pj_pop3_password { get; set; }
        public string pj_pop3_email_from { get; set; }
        public int pj_enable_custom_dropdown1 { get; set; }
        public int pj_enable_custom_dropdown2 { get; set; }
        public int pj_enable_custom_dropdown3 { get; set; }
        public string pj_custom_dropdown_label1 { get; set; }
        public string pj_custom_dropdown_label2 { get; set; }
        public string pj_custom_dropdown_label3 { get; set; }
        public string pj_custom_dropdown_values1 { get; set; }
        public string pj_custom_dropdown_values2 { get; set; }
        public string pj_custom_dropdown_values3 { get; set; }
        public int pj_default { get; set; }
        public string pj_description { get; set; }
    }
}
