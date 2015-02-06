using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_relationshipsMap : EntityTypeConfiguration<bug_relationships>
    {
        public bug_relationshipsMap()
        {
            // Primary Key
            this.HasKey(t => t.re_id);

            // Properties
            this.Property(t => t.re_type)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("bug_relationships");
            this.Property(t => t.re_id).HasColumnName("re_id");
            this.Property(t => t.re_bug1).HasColumnName("re_bug1");
            this.Property(t => t.re_bug2).HasColumnName("re_bug2");
            this.Property(t => t.re_type).HasColumnName("re_type");
            this.Property(t => t.re_direction).HasColumnName("re_direction");
        }
    }
}
