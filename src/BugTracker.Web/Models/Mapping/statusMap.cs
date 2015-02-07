using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class StatusMap : EntityTypeConfiguration<Status>
    {
        public StatusMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.Style)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("statuses");
            this.Property(t => t.Id).HasColumnName("st_id");
            this.Property(t => t.Name).HasColumnName("st_name");
            this.Property(t => t.SortOrder).HasColumnName("st_sort_seq");
            this.Property(t => t.Style).HasColumnName("st_style");
            this.Property(t => t.Default).HasColumnName("st_default");
        }
    }
}
