using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class org
    {
        public int og_id { get; set; }
        public string og_name { get; set; }
        public string og_domain { get; set; }
        public int og_non_admins_can_use { get; set; }
        public int og_external_user { get; set; }
        public int og_can_be_assigned_to { get; set; }
        public int og_can_only_see_own_reported { get; set; }
        public int og_can_edit_sql { get; set; }
        public int og_can_delete_bug { get; set; }
        public int og_can_edit_and_delete_posts { get; set; }
        public int og_can_merge_bugs { get; set; }
        public int og_can_mass_edit_bugs { get; set; }
        public int og_can_use_reports { get; set; }
        public int og_can_edit_reports { get; set; }
        public int og_can_view_tasks { get; set; }
        public int og_can_edit_tasks { get; set; }
        public int og_can_search { get; set; }
        public int og_other_orgs_permission_level { get; set; }
        public int og_can_assign_to_internal_users { get; set; }
        public int og_category_field_permission_level { get; set; }
        public int og_priority_field_permission_level { get; set; }
        public int og_assigned_to_field_permission_level { get; set; }
        public int og_status_field_permission_level { get; set; }
        public int og_project_field_permission_level { get; set; }
        public int og_org_field_permission_level { get; set; }
        public int og_udf_field_permission_level { get; set; }
        public int og_tags_field_permission_level { get; set; }
        public int og_active { get; set; }
    }
}
