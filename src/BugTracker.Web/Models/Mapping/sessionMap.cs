using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class sessionMap : EntityTypeConfiguration<session>
    {
        public sessionMap()
        {
            // Primary Key
            this.HasKey(t => t.se_id);

            // Properties
            this.Property(t => t.se_id)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(37);

            // Table & Column Mappings
            this.ToTable("sessions");
            this.Property(t => t.se_id).HasColumnName("se_id");
            this.Property(t => t.se_date).HasColumnName("se_date");
            this.Property(t => t.se_user).HasColumnName("se_user");
        }
    }
}
