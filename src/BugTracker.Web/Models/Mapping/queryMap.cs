using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class queryMap : EntityTypeConfiguration<query>
    {
        public queryMap()
        {
            // Primary Key
            this.HasKey(t => t.qu_id);

            // Properties
            this.Property(t => t.qu_desc)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.qu_sql)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("queries");
            this.Property(t => t.qu_id).HasColumnName("qu_id");
            this.Property(t => t.qu_desc).HasColumnName("qu_desc");
            this.Property(t => t.qu_sql).HasColumnName("qu_sql");
            this.Property(t => t.qu_default).HasColumnName("qu_default");
            this.Property(t => t.qu_user).HasColumnName("qu_user");
            this.Property(t => t.qu_org).HasColumnName("qu_org");
        }
    }
}
