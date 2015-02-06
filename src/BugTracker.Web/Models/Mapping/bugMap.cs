using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugMap : EntityTypeConfiguration<Bug>
    {
        public BugMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.ShortDescription)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.CustomDropDownValue1)
                .HasMaxLength(120);

            Property(t => t.CustomDropDownValue2)
                .HasMaxLength(120);

            Property(t => t.CustomDropDownValue3)
                .HasMaxLength(120);

            Property(t => t.Tags)
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("bugs");
            Property(t => t.Id).HasColumnName("bg_id");
            Property(t => t.ShortDescription).HasColumnName("bg_short_desc");
            Property(t => t.ReportedUserId).HasColumnName("bg_reported_user");
            Property(t => t.ReportedDate).HasColumnName("bg_reported_date");
            Property(t => t.StatusId).HasColumnName("bg_status");
            Property(t => t.PriorityId).HasColumnName("bg_priority");
            Property(t => t.OrganizationId).HasColumnName("bg_org");
            Property(t => t.CategoryId).HasColumnName("bg_category");
            Property(t => t.ProjectId).HasColumnName("bg_project");
            Property(t => t.AssignedToUserId).HasColumnName("bg_assigned_to_user");
            Property(t => t.LastUpdatedUserId).HasColumnName("bg_last_updated_user");
            Property(t => t.LastUpdatedDate).HasColumnName("bg_last_updated_date");
            Property(t => t.UserDefinedAttributeId).HasColumnName("bg_user_defined_attribute");
            Property(t => t.CustomDropDownValue1).HasColumnName("bg_project_custom_dropdown_value1");
            Property(t => t.CustomDropDownValue2).HasColumnName("bg_project_custom_dropdown_value2");
            Property(t => t.CustomDropDownValue3).HasColumnName("bg_project_custom_dropdown_value3");
            Property(t => t.Tags).HasColumnName("bg_tags");
        }
    }
}
