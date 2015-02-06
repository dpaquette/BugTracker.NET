using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class categoryMap : EntityTypeConfiguration<category>
    {
        public categoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ct_id);

            // Properties
            this.Property(t => t.ct_name)
                .IsRequired()
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("categories");
            this.Property(t => t.ct_id).HasColumnName("ct_id");
            this.Property(t => t.ct_name).HasColumnName("ct_name");
            this.Property(t => t.ct_sort_seq).HasColumnName("ct_sort_seq");
            this.Property(t => t.ct_default).HasColumnName("ct_default");
        }
    }
}
