using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class bug_tasks
    {
        public int tsk_id { get; set; }
        public int tsk_bug { get; set; }
        public int tsk_created_user { get; set; }
        public System.DateTime tsk_created_date { get; set; }
        public int tsk_last_updated_user { get; set; }
        public System.DateTime tsk_last_updated_date { get; set; }
        public Nullable<int> tsk_assigned_to_user { get; set; }
        public Nullable<System.DateTime> tsk_planned_start_date { get; set; }
        public Nullable<System.DateTime> tsk_actual_start_date { get; set; }
        public Nullable<System.DateTime> tsk_planned_end_date { get; set; }
        public Nullable<System.DateTime> tsk_actual_end_date { get; set; }
        public Nullable<decimal> tsk_planned_duration { get; set; }
        public Nullable<decimal> tsk_actual_duration { get; set; }
        public string tsk_duration_units { get; set; }
        public Nullable<int> tsk_percent_complete { get; set; }
        public Nullable<int> tsk_status { get; set; }
        public Nullable<int> tsk_sort_sequence { get; set; }
        public string tsk_description { get; set; }
    }
}
