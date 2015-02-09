using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class OrganizationMap : EntityTypeConfiguration<Organization>
    {
        public OrganizationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(80);

            this.Property(t => t.Domain)
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("orgs");
            this.Property(t => t.Id).HasColumnName("og_id");
            this.Property(t => t.Name).HasColumnName("og_name");
            this.Property(t => t.Domain).HasColumnName("og_domain");
            this.Property(t => t.NonAdminsCanUse).HasColumnName("og_non_admins_can_use");
            this.Property(t => t.ExternalUser).HasColumnName("og_external_user");
            this.Property(t => t.CanBeAssignedTo).HasColumnName("og_can_be_assigned_to");
            this.Property(t => t.CanOnlySeeOwnReport).HasColumnName("og_can_only_see_own_reported");
            this.Property(t => t.CanEditSQL).HasColumnName("og_can_edit_sql");
            this.Property(t => t.CanDeleteBug).HasColumnName("og_can_delete_bug");
            this.Property(t => t.CanEditPosts).HasColumnName("og_can_edit_and_delete_posts");
            this.Property(t => t.CanMergeBugs).HasColumnName("og_can_merge_bugs");
            this.Property(t => t.CanMassEdit).HasColumnName("og_can_mass_edit_bugs");
            this.Property(t => t.CanUseReports).HasColumnName("og_can_use_reports");
            this.Property(t => t.CanEditReports).HasColumnName("og_can_edit_reports");
            this.Property(t => t.CanViewTasks).HasColumnName("og_can_view_tasks");
            this.Property(t => t.CanEditTasks).HasColumnName("og_can_edit_tasks");
            this.Property(t => t.CanSearch).HasColumnName("og_can_search");
            this.Property(t => t.OtherOrgsPermissionLevel).HasColumnName("og_other_orgs_permission_level");
            this.Property(t => t.CanAssignToInternalUsers).HasColumnName("og_can_assign_to_internal_users");
            this.Property(t => t.CategoryFieldPErmissionLevel).HasColumnName("og_category_field_permission_level");
            this.Property(t => t.PriorityFieldPermissionLevel).HasColumnName("og_priority_field_permission_level");
            this.Property(t => t.AssignedToFieldPermissionLevel).HasColumnName("og_assigned_to_field_permission_level");
            this.Property(t => t.StatusFieldPermissionLevel).HasColumnName("og_status_field_permission_level");
            this.Property(t => t.ProjectFieldPermissionLevel).HasColumnName("og_project_field_permission_level");
            this.Property(t => t.OrgFieldPermissionLevel).HasColumnName("og_org_field_permission_level");
            this.Property(t => t.UserDefinedFieldPermissionLevel).HasColumnName("og_udf_field_permission_level");
            this.Property(t => t.TagsPermissionLevel).HasColumnName("og_tags_field_permission_level");
            this.Property(t => t.Active).HasColumnName("og_active");
        }
    }
}
