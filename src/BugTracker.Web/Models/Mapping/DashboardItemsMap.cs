using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class DashboardItemsMap : EntityTypeConfiguration<DashboardItems>
    {
        public DashboardItemsMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ChartType)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("dashboard_items");
            this.Property(t => t.Id).HasColumnName("ds_id");
            this.Property(t => t.UserId).HasColumnName("ds_user");
            this.Property(t => t.ReportId).HasColumnName("ds_report");
            this.Property(t => t.ChartType).HasColumnName("ds_chart_type");
            this.Property(t => t.Column).HasColumnName("ds_col");
            this.Property(t => t.Row).HasColumnName("ds_row");
        }
    }
}
