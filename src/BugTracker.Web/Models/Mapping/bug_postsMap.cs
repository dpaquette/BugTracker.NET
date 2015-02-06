using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_postsMap : EntityTypeConfiguration<bug_posts>
    {
        public bug_postsMap()
        {
            // Primary Key
            this.HasKey(t => t.bp_id);

            // Properties
            this.Property(t => t.bp_type)
                .IsRequired()
                .HasMaxLength(8);

            this.Property(t => t.bp_comment)
                .IsRequired();

            this.Property(t => t.bp_email_from)
                .HasMaxLength(800);

            this.Property(t => t.bp_email_to)
                .HasMaxLength(800);

            this.Property(t => t.bp_file)
                .HasMaxLength(1000);

            this.Property(t => t.bp_content_type)
                .HasMaxLength(200);

            this.Property(t => t.bp_email_cc)
                .HasMaxLength(800);

            // Table & Column Mappings
            this.ToTable("bug_posts");
            this.Property(t => t.bp_id).HasColumnName("bp_id");
            this.Property(t => t.bp_bug).HasColumnName("bp_bug");
            this.Property(t => t.bp_type).HasColumnName("bp_type");
            this.Property(t => t.bp_user).HasColumnName("bp_user");
            this.Property(t => t.bp_date).HasColumnName("bp_date");
            this.Property(t => t.bp_comment).HasColumnName("bp_comment");
            this.Property(t => t.bp_comment_search).HasColumnName("bp_comment_search");
            this.Property(t => t.bp_email_from).HasColumnName("bp_email_from");
            this.Property(t => t.bp_email_to).HasColumnName("bp_email_to");
            this.Property(t => t.bp_file).HasColumnName("bp_file");
            this.Property(t => t.bp_size).HasColumnName("bp_size");
            this.Property(t => t.bp_content_type).HasColumnName("bp_content_type");
            this.Property(t => t.bp_parent).HasColumnName("bp_parent");
            this.Property(t => t.bp_original_comment_id).HasColumnName("bp_original_comment_id");
            this.Property(t => t.bp_hidden_from_external_users).HasColumnName("bp_hidden_from_external_users");
            this.Property(t => t.bp_email_cc).HasColumnName("bp_email_cc");
        }
    }
}
