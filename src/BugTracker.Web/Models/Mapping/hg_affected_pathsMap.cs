using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class hg_affected_pathsMap : EntityTypeConfiguration<hg_affected_paths>
    {
        public hg_affected_pathsMap()
        {
            // Primary Key
            this.HasKey(t => t.hgap_id);

            // Properties
            this.Property(t => t.hgap_action)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.hgap_path)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("hg_affected_paths");
            this.Property(t => t.hgap_id).HasColumnName("hgap_id");
            this.Property(t => t.hgap_hgrev_id).HasColumnName("hgap_hgrev_id");
            this.Property(t => t.hgap_action).HasColumnName("hgap_action");
            this.Property(t => t.hgap_path).HasColumnName("hgap_path");
        }
    }
}
