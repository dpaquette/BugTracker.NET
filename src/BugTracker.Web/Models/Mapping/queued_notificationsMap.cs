using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class queued_notificationsMap : EntityTypeConfiguration<queued_notifications>
    {
        public queued_notificationsMap()
        {
            // Primary Key
            this.HasKey(t => t.qn_id);

            // Properties
            this.Property(t => t.qn_status)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.qn_last_exception)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.qn_to)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.qn_from)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.qn_subject)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.qn_body)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("queued_notifications");
            this.Property(t => t.qn_id).HasColumnName("qn_id");
            this.Property(t => t.qn_date_created).HasColumnName("qn_date_created");
            this.Property(t => t.qn_bug).HasColumnName("qn_bug");
            this.Property(t => t.qn_user).HasColumnName("qn_user");
            this.Property(t => t.qn_status).HasColumnName("qn_status");
            this.Property(t => t.qn_retries).HasColumnName("qn_retries");
            this.Property(t => t.qn_last_exception).HasColumnName("qn_last_exception");
            this.Property(t => t.qn_to).HasColumnName("qn_to");
            this.Property(t => t.qn_from).HasColumnName("qn_from");
            this.Property(t => t.qn_subject).HasColumnName("qn_subject");
            this.Property(t => t.qn_body).HasColumnName("qn_body");
        }
    }
}
