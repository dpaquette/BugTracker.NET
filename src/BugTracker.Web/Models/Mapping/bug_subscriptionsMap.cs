using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_subscriptionsMap : EntityTypeConfiguration<bug_subscriptions>
    {
        public bug_subscriptionsMap()
        {
            // Primary Key
            this.HasKey(t => new { t.bs_bug, t.bs_user });

            // Properties
            this.Property(t => t.bs_bug)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bs_user)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("bug_subscriptions");
            this.Property(t => t.bs_bug).HasColumnName("bs_bug");
            this.Property(t => t.bs_user).HasColumnName("bs_user");
        }
    }
}
