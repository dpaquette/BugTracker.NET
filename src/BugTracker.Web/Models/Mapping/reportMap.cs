using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class ReportMap : EntityTypeConfiguration<Report>
    {
        public ReportMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.SQL)
                .IsRequired();

            this.Property(t => t.ChartType)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("reports");
            this.Property(t => t.Id).HasColumnName("rp_id");
            this.Property(t => t.Description).HasColumnName("rp_desc");
            this.Property(t => t.SQL).HasColumnName("rp_sql");
            this.Property(t => t.ChartType).HasColumnName("rp_chart_type");
        }
    }
}
