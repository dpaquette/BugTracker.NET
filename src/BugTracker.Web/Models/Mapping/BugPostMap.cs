using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugPostMap : EntityTypeConfiguration<BugPost>
    {
        public BugPostMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.Comment)
                .IsRequired();

            this.Property(t => t.FromEmail)
                .HasMaxLength(800);

            this.Property(t => t.ToEmail)
                .HasMaxLength(800);

            this.Property(t => t.File)
                .HasMaxLength(1000);

            this.Property(t => t.ContentType)
                .HasMaxLength(200);

            this.Property(t => t.CCEmail)
                .HasMaxLength(800);

            // Table & Column Mappings
            this.ToTable("bug_posts");
            this.Property(t => t.Id).HasColumnName("bp_id");
            this.Property(t => t.BugId).HasColumnName("bp_bug");
            this.Property(t => t.Type).HasColumnName("bp_type");
            this.Property(t => t.UserId).HasColumnName("bp_user");
            this.Property(t => t.Date).HasColumnName("bp_date");
            this.Property(t => t.Comment).HasColumnName("bp_comment");
            this.Property(t => t.CommentSearch).HasColumnName("bp_comment_search");
            this.Property(t => t.FromEmail).HasColumnName("bp_email_from");
            this.Property(t => t.ToEmail).HasColumnName("bp_email_to");
            this.Property(t => t.File).HasColumnName("bp_file");
            this.Property(t => t.Size).HasColumnName("bp_size");
            this.Property(t => t.ContentType).HasColumnName("bp_content_type");
            this.Property(t => t.ParentId).HasColumnName("bp_parent");
            this.Property(t => t.OriginalCommentId).HasColumnName("bp_original_comment_id");
            this.Property(t => t.HideFromExternalUsers).HasColumnName("bp_hidden_from_external_users");
            this.Property(t => t.CCEmail).HasColumnName("bp_email_cc");
        }
    }
}
