using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class svn_revisionsMap : EntityTypeConfiguration<svn_revisions>
    {
        public svn_revisionsMap()
        {
            // Primary Key
            this.HasKey(t => t.svnrev_id);

            // Properties
            this.Property(t => t.svnrev_repository)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.svnrev_author)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.svnrev_svn_date)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.svnrev_msg)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("svn_revisions");
            this.Property(t => t.svnrev_id).HasColumnName("svnrev_id");
            this.Property(t => t.svnrev_revision).HasColumnName("svnrev_revision");
            this.Property(t => t.svnrev_bug).HasColumnName("svnrev_bug");
            this.Property(t => t.svnrev_repository).HasColumnName("svnrev_repository");
            this.Property(t => t.svnrev_author).HasColumnName("svnrev_author");
            this.Property(t => t.svnrev_svn_date).HasColumnName("svnrev_svn_date");
            this.Property(t => t.svnrev_btnet_date).HasColumnName("svnrev_btnet_date");
            this.Property(t => t.svnrev_msg).HasColumnName("svnrev_msg");
        }
    }
}
