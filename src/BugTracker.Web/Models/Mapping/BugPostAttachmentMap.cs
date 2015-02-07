using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugPostAttachmentMap : EntityTypeConfiguration<BugPostAttachment>
    {
        public BugPostAttachmentMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Content)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("bug_post_attachments");
            this.Property(t => t.Id).HasColumnName("bpa_id");
            this.Property(t => t.PostId).HasColumnName("bpa_post");
            this.Property(t => t.Content).HasColumnName("bpa_content");
        }
    }
}
