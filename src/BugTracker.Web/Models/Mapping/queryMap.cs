using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class QueryMap : EntityTypeConfiguration<Query>
    {
        public QueryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.SQL)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("queries");
            this.Property(t => t.Id).HasColumnName("qu_id");
            this.Property(t => t.Description).HasColumnName("qu_desc");
            this.Property(t => t.SQL).HasColumnName("qu_sql");
            this.Property(t => t.Default).HasColumnName("qu_default");
            this.Property(t => t.User).HasColumnName("qu_user");
            this.Property(t => t.Org).HasColumnName("qu_org");
            this.Property(t => t.Columns).HasColumnName("qu_columns");
        }
    }
}
