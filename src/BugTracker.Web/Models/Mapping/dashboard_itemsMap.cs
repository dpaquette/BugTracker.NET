using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class dashboard_itemsMap : EntityTypeConfiguration<dashboard_items>
    {
        public dashboard_itemsMap()
        {
            // Primary Key
            this.HasKey(t => t.ds_id);

            // Properties
            this.Property(t => t.ds_chart_type)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("dashboard_items");
            this.Property(t => t.ds_id).HasColumnName("ds_id");
            this.Property(t => t.ds_user).HasColumnName("ds_user");
            this.Property(t => t.ds_report).HasColumnName("ds_report");
            this.Property(t => t.ds_chart_type).HasColumnName("ds_chart_type");
            this.Property(t => t.ds_col).HasColumnName("ds_col");
            this.Property(t => t.ds_row).HasColumnName("ds_row");
        }
    }
}
