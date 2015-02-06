using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class UserDefinedAttributeMap : EntityTypeConfiguration<UserDefinedAttribute>
    {
        public UserDefinedAttributeMap()
        {
            // Primary Key
            this.HasKey(t => t.udf_id);

            // Properties
            this.Property(t => t.udf_name)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("user_defined_attribute");
            this.Property(t => t.udf_id).HasColumnName("udf_id");
            this.Property(t => t.udf_name).HasColumnName("udf_name");
            this.Property(t => t.udf_sort_seq).HasColumnName("udf_sort_seq");
            this.Property(t => t.udf_default).HasColumnName("udf_default");
        }
    }
}
