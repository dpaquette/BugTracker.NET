using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class git_commitsMap : EntityTypeConfiguration<git_commits>
    {
        public git_commitsMap()
        {
            // Primary Key
            this.HasKey(t => t.gitcom_id);

            // Properties
            this.Property(t => t.gitcom_commit)
                .IsFixedLength()
                .HasMaxLength(40);

            this.Property(t => t.gitcom_repository)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.gitcom_author)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.gitcom_git_date)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.gitcom_msg)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("git_commits");
            this.Property(t => t.gitcom_id).HasColumnName("gitcom_id");
            this.Property(t => t.gitcom_commit).HasColumnName("gitcom_commit");
            this.Property(t => t.gitcom_bug).HasColumnName("gitcom_bug");
            this.Property(t => t.gitcom_repository).HasColumnName("gitcom_repository");
            this.Property(t => t.gitcom_author).HasColumnName("gitcom_author");
            this.Property(t => t.gitcom_git_date).HasColumnName("gitcom_git_date");
            this.Property(t => t.gitcom_btnet_date).HasColumnName("gitcom_btnet_date");
            this.Property(t => t.gitcom_msg).HasColumnName("gitcom_msg");
        }
    }
}
