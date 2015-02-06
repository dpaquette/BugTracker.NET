using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class project_user_xrefMap : EntityTypeConfiguration<project_user_xref>
    {
        public project_user_xrefMap()
        {
            // Primary Key
            this.HasKey(t => t.pu_id);

            // Properties
            // Table & Column Mappings
            this.ToTable("project_user_xref");
            this.Property(t => t.pu_id).HasColumnName("pu_id");
            this.Property(t => t.pu_project).HasColumnName("pu_project");
            this.Property(t => t.pu_user).HasColumnName("pu_user");
            this.Property(t => t.pu_auto_subscribe).HasColumnName("pu_auto_subscribe");
            this.Property(t => t.pu_permission_level).HasColumnName("pu_permission_level");
            this.Property(t => t.pu_admin).HasColumnName("pu_admin");
        }
    }
}
