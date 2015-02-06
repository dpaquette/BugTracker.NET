using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bugMap : EntityTypeConfiguration<bug>
    {
        public bugMap()
        {
            // Primary Key
            this.HasKey(t => t.bg_id);

            // Properties
            this.Property(t => t.bg_short_desc)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.bg_project_custom_dropdown_value1)
                .HasMaxLength(120);

            this.Property(t => t.bg_project_custom_dropdown_value2)
                .HasMaxLength(120);

            this.Property(t => t.bg_project_custom_dropdown_value3)
                .HasMaxLength(120);

            this.Property(t => t.bg_tags)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("bugs");
            this.Property(t => t.bg_id).HasColumnName("bg_id");
            this.Property(t => t.bg_short_desc).HasColumnName("bg_short_desc");
            this.Property(t => t.bg_reported_user).HasColumnName("bg_reported_user");
            this.Property(t => t.bg_reported_date).HasColumnName("bg_reported_date");
            this.Property(t => t.bg_status).HasColumnName("bg_status");
            this.Property(t => t.bg_priority).HasColumnName("bg_priority");
            this.Property(t => t.bg_org).HasColumnName("bg_org");
            this.Property(t => t.bg_category).HasColumnName("bg_category");
            this.Property(t => t.bg_project).HasColumnName("bg_project");
            this.Property(t => t.bg_assigned_to_user).HasColumnName("bg_assigned_to_user");
            this.Property(t => t.bg_last_updated_user).HasColumnName("bg_last_updated_user");
            this.Property(t => t.bg_last_updated_date).HasColumnName("bg_last_updated_date");
            this.Property(t => t.bg_user_defined_attribute).HasColumnName("bg_user_defined_attribute");
            this.Property(t => t.bg_project_custom_dropdown_value1).HasColumnName("bg_project_custom_dropdown_value1");
            this.Property(t => t.bg_project_custom_dropdown_value2).HasColumnName("bg_project_custom_dropdown_value2");
            this.Property(t => t.bg_project_custom_dropdown_value3).HasColumnName("bg_project_custom_dropdown_value3");
            this.Property(t => t.bg_tags).HasColumnName("bg_tags");
        }
    }
}
