using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class EmailedLinkMap : EntityTypeConfiguration<EmailedLink>
    {
        public EmailedLinkMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(37);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(120);

            this.Property(t => t.Action)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UserName)
                .HasMaxLength(40);

            this.Property(t => t.PasswordHash)
                .HasMaxLength(64);

            this.Property(t => t.FirstName)
                .HasMaxLength(60);

            this.Property(t => t.LastName)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("emailed_links");
            this.Property(t => t.Id).HasColumnName("el_id");
            this.Property(t => t.Date).HasColumnName("el_date");
            this.Property(t => t.Email).HasColumnName("el_email");
            this.Property(t => t.Action).HasColumnName("el_action");
            this.Property(t => t.UserName).HasColumnName("el_username");
            this.Property(t => t.UserId).HasColumnName("el_user_id");
            this.Property(t => t.Salt).HasColumnName("el_salt");
            this.Property(t => t.PasswordHash).HasColumnName("el_password");
            this.Property(t => t.FirstName).HasColumnName("el_firstname");
            this.Property(t => t.LastName).HasColumnName("el_lastname");
        }
    }
}
