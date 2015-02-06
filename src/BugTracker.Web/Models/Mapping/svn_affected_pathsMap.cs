using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class svn_affected_pathsMap : EntityTypeConfiguration<svn_affected_paths>
    {
        public svn_affected_pathsMap()
        {
            // Primary Key
            this.HasKey(t => t.svnap_id);

            // Properties
            this.Property(t => t.svnap_action)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.svnap_path)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("svn_affected_paths");
            this.Property(t => t.svnap_id).HasColumnName("svnap_id");
            this.Property(t => t.svnap_svnrev_id).HasColumnName("svnap_svnrev_id");
            this.Property(t => t.svnap_action).HasColumnName("svnap_action");
            this.Property(t => t.svnap_path).HasColumnName("svnap_path");
        }
    }
}
