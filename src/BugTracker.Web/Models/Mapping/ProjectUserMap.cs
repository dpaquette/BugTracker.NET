using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class ProjectUserMap : EntityTypeConfiguration<ProjectUser>
    {
        public ProjectUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("project_user_xref");
            this.Property(t => t.Id).HasColumnName("pu_id");
            this.Property(t => t.ProjectId).HasColumnName("pu_project");
            this.Property(t => t.UserId).HasColumnName("pu_user");
            this.Property(t => t.AutoSubscribe).HasColumnName("pu_auto_subscribe");
            this.Property(t => t.PermissionLevel).HasColumnName("pu_permission_level");
            this.Property(t => t.AdminId).HasColumnName("pu_admin");
        }
    }
}
