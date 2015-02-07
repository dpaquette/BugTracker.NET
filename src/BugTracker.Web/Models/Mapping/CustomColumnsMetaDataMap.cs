using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class CustomColumnsMetaDataMap : EntityTypeConfiguration<CustomColumnsMetaData>
    {
        public CustomColumnsMetaDataMap()
        {
            // Primary Key
            this.HasKey(t => t.Order);

            // Properties
            this.Property(t => t.Order)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DropDownValues)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.DropDownType)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("custom_col_metadata");
            this.Property(t => t.Order).HasColumnName("ccm_colorder");
            this.Property(t => t.DropDownValues).HasColumnName("ccm_dropdown_vals");
            this.Property(t => t.SortSequence).HasColumnName("ccm_sort_seq");
            this.Property(t => t.DropDownType).HasColumnName("ccm_dropdown_type");
        }
    }
}
