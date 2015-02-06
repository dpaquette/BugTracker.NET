using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class reportMap : EntityTypeConfiguration<report>
    {
        public reportMap()
        {
            // Primary Key
            this.HasKey(t => t.rp_id);

            // Properties
            this.Property(t => t.rp_desc)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.rp_sql)
                .IsRequired();

            this.Property(t => t.rp_chart_type)
                .IsRequired()
                .HasMaxLength(8);

            // Table & Column Mappings
            this.ToTable("reports");
            this.Property(t => t.rp_id).HasColumnName("rp_id");
            this.Property(t => t.rp_desc).HasColumnName("rp_desc");
            this.Property(t => t.rp_sql).HasColumnName("rp_sql");
            this.Property(t => t.rp_chart_type).HasColumnName("rp_chart_type");
        }
    }
}
