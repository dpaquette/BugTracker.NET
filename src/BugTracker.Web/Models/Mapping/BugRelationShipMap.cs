using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugRelationShipMap : EntityTypeConfiguration<BugRelationShip>
    {
        public BugRelationShipMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Type)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("bug_relationships");
            this.Property(t => t.Id).HasColumnName("re_id");
            this.Property(t => t.Bug1Id).HasColumnName("re_bug1");
            this.Property(t => t.Bug2Id).HasColumnName("re_bug2");
            this.Property(t => t.Type).HasColumnName("re_type");
            this.Property(t => t.Direction).HasColumnName("re_direction");
        }
    }
}
