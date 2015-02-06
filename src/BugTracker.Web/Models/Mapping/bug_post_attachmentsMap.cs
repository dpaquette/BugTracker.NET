using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_post_attachmentsMap : EntityTypeConfiguration<bug_post_attachments>
    {
        public bug_post_attachmentsMap()
        {
            // Primary Key
            this.HasKey(t => t.bpa_id);

            // Properties
            this.Property(t => t.bpa_content)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("bug_post_attachments");
            this.Property(t => t.bpa_id).HasColumnName("bpa_id");
            this.Property(t => t.bpa_post).HasColumnName("bpa_post");
            this.Property(t => t.bpa_content).HasColumnName("bpa_content");
        }
    }
}
