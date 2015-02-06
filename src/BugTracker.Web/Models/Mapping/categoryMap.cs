using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(80);

            // Table & Column Mappings
            this.ToTable("categories");
            this.Property(t => t.Id).HasColumnName("ct_id");
            this.Property(t => t.Name).HasColumnName("ct_name");
            this.Property(t => t.SortOrder).HasColumnName("ct_sort_seq");
            this.Property(t => t.Default).HasColumnName("ct_default");
        }
    }
}
