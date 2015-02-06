using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class custom_col_metadataMap : EntityTypeConfiguration<custom_col_metadata>
    {
        public custom_col_metadataMap()
        {
            // Primary Key
            this.HasKey(t => t.ccm_colorder);

            // Properties
            this.Property(t => t.ccm_colorder)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ccm_dropdown_vals)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.ccm_dropdown_type)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("custom_col_metadata");
            this.Property(t => t.ccm_colorder).HasColumnName("ccm_colorder");
            this.Property(t => t.ccm_dropdown_vals).HasColumnName("ccm_dropdown_vals");
            this.Property(t => t.ccm_sort_seq).HasColumnName("ccm_sort_seq");
            this.Property(t => t.ccm_dropdown_type).HasColumnName("ccm_dropdown_type");
        }
    }
}
