using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class hg_revisionsMap : EntityTypeConfiguration<hg_revisions>
    {
        public hg_revisionsMap()
        {
            // Primary Key
            this.HasKey(t => t.hgrev_id);

            // Properties
            this.Property(t => t.hgrev_repository)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.hgrev_author)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.hgrev_hg_date)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.hgrev_msg)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("hg_revisions");
            this.Property(t => t.hgrev_id).HasColumnName("hgrev_id");
            this.Property(t => t.hgrev_revision).HasColumnName("hgrev_revision");
            this.Property(t => t.hgrev_bug).HasColumnName("hgrev_bug");
            this.Property(t => t.hgrev_repository).HasColumnName("hgrev_repository");
            this.Property(t => t.hgrev_author).HasColumnName("hgrev_author");
            this.Property(t => t.hgrev_hg_date).HasColumnName("hgrev_hg_date");
            this.Property(t => t.hgrev_btnet_date).HasColumnName("hgrev_btnet_date");
            this.Property(t => t.hgrev_msg).HasColumnName("hgrev_msg");
        }
    }
}
