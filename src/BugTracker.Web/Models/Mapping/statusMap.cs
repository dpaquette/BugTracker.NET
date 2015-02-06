using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class statusMap : EntityTypeConfiguration<status>
    {
        public statusMap()
        {
            // Primary Key
            this.HasKey(t => t.st_id);

            // Properties
            this.Property(t => t.st_name)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.st_style)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("statuses");
            this.Property(t => t.st_id).HasColumnName("st_id");
            this.Property(t => t.st_name).HasColumnName("st_name");
            this.Property(t => t.st_sort_seq).HasColumnName("st_sort_seq");
            this.Property(t => t.st_style).HasColumnName("st_style");
            this.Property(t => t.st_default).HasColumnName("st_default");
        }
    }
}
