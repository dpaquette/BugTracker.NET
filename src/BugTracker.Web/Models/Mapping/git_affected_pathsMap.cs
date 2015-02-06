using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class git_affected_pathsMap : EntityTypeConfiguration<git_affected_paths>
    {
        public git_affected_pathsMap()
        {
            // Primary Key
            this.HasKey(t => t.gitap_id);

            // Properties
            this.Property(t => t.gitap_action)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.gitap_path)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("git_affected_paths");
            this.Property(t => t.gitap_id).HasColumnName("gitap_id");
            this.Property(t => t.gitap_gitcom_id).HasColumnName("gitap_gitcom_id");
            this.Property(t => t.gitap_action).HasColumnName("gitap_action");
            this.Property(t => t.gitap_path).HasColumnName("gitap_path");
        }
    }
}
