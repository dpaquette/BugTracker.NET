using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class orgMap : EntityTypeConfiguration<org>
    {
        public orgMap()
        {
            // Primary Key
            this.HasKey(t => t.og_id);

            // Properties
            this.Property(t => t.og_name)
                .IsRequired()
                .HasMaxLength(80);

            this.Property(t => t.og_domain)
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("orgs");
            this.Property(t => t.og_id).HasColumnName("og_id");
            this.Property(t => t.og_name).HasColumnName("og_name");
            this.Property(t => t.og_domain).HasColumnName("og_domain");
            this.Property(t => t.og_non_admins_can_use).HasColumnName("og_non_admins_can_use");
            this.Property(t => t.og_external_user).HasColumnName("og_external_user");
            this.Property(t => t.og_can_be_assigned_to).HasColumnName("og_can_be_assigned_to");
            this.Property(t => t.og_can_only_see_own_reported).HasColumnName("og_can_only_see_own_reported");
            this.Property(t => t.og_can_edit_sql).HasColumnName("og_can_edit_sql");
            this.Property(t => t.og_can_delete_bug).HasColumnName("og_can_delete_bug");
            this.Property(t => t.og_can_edit_and_delete_posts).HasColumnName("og_can_edit_and_delete_posts");
            this.Property(t => t.og_can_merge_bugs).HasColumnName("og_can_merge_bugs");
            this.Property(t => t.og_can_mass_edit_bugs).HasColumnName("og_can_mass_edit_bugs");
            this.Property(t => t.og_can_use_reports).HasColumnName("og_can_use_reports");
            this.Property(t => t.og_can_edit_reports).HasColumnName("og_can_edit_reports");
            this.Property(t => t.og_can_view_tasks).HasColumnName("og_can_view_tasks");
            this.Property(t => t.og_can_edit_tasks).HasColumnName("og_can_edit_tasks");
            this.Property(t => t.og_can_search).HasColumnName("og_can_search");
            this.Property(t => t.og_other_orgs_permission_level).HasColumnName("og_other_orgs_permission_level");
            this.Property(t => t.og_can_assign_to_internal_users).HasColumnName("og_can_assign_to_internal_users");
            this.Property(t => t.og_category_field_permission_level).HasColumnName("og_category_field_permission_level");
            this.Property(t => t.og_priority_field_permission_level).HasColumnName("og_priority_field_permission_level");
            this.Property(t => t.og_assigned_to_field_permission_level).HasColumnName("og_assigned_to_field_permission_level");
            this.Property(t => t.og_status_field_permission_level).HasColumnName("og_status_field_permission_level");
            this.Property(t => t.og_project_field_permission_level).HasColumnName("og_project_field_permission_level");
            this.Property(t => t.og_org_field_permission_level).HasColumnName("og_org_field_permission_level");
            this.Property(t => t.og_udf_field_permission_level).HasColumnName("og_udf_field_permission_level");
            this.Property(t => t.og_tags_field_permission_level).HasColumnName("og_tags_field_permission_level");
            this.Property(t => t.og_active).HasColumnName("og_active");
        }
    }
}
