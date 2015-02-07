using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class QueuedNotificationMap : EntityTypeConfiguration<QueuedNotification>
    {
        public QueuedNotificationMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.LastException)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.To)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.From)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.Body)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("queued_notifications");
            this.Property(t => t.Id).HasColumnName("qn_id");
            this.Property(t => t.DateCreated).HasColumnName("qn_date_created");
            this.Property(t => t.BugId).HasColumnName("qn_bug");
            this.Property(t => t.UserId).HasColumnName("qn_user");
            this.Property(t => t.Status).HasColumnName("qn_status");
            this.Property(t => t.Retries).HasColumnName("qn_retries");
            this.Property(t => t.LastException).HasColumnName("qn_last_exception");
            this.Property(t => t.To).HasColumnName("qn_to");
            this.Property(t => t.From).HasColumnName("qn_from");
            this.Property(t => t.Subject).HasColumnName("qn_subject");
            this.Property(t => t.Body).HasColumnName("qn_body");
        }
    }
}
