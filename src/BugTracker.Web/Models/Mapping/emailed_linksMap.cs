using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class emailed_linksMap : EntityTypeConfiguration<emailed_links>
    {
        public emailed_linksMap()
        {
            // Primary Key
            this.HasKey(t => t.el_id);

            // Properties
            this.Property(t => t.el_id)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(37);

            this.Property(t => t.el_email)
                .IsRequired()
                .HasMaxLength(120);

            this.Property(t => t.el_action)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.el_username)
                .HasMaxLength(40);

            this.Property(t => t.el_password)
                .HasMaxLength(64);

            this.Property(t => t.el_firstname)
                .HasMaxLength(60);

            this.Property(t => t.el_lastname)
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("emailed_links");
            this.Property(t => t.el_id).HasColumnName("el_id");
            this.Property(t => t.el_date).HasColumnName("el_date");
            this.Property(t => t.el_email).HasColumnName("el_email");
            this.Property(t => t.el_action).HasColumnName("el_action");
            this.Property(t => t.el_username).HasColumnName("el_username");
            this.Property(t => t.el_user_id).HasColumnName("el_user_id");
            this.Property(t => t.el_salt).HasColumnName("el_salt");
            this.Property(t => t.el_password).HasColumnName("el_password");
            this.Property(t => t.el_firstname).HasColumnName("el_firstname");
            this.Property(t => t.el_lastname).HasColumnName("el_lastname");
        }
    }
}
