using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugSubscriptionMap : EntityTypeConfiguration<BugSubscription>
    {
        public BugSubscriptionMap()
        {
            // Primary Key
            this.HasKey(t => new { bs_bug = t.BugId, bs_user = t.UserId });

            // Properties
            this.Property(t => t.BugId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("bug_subscriptions");
            this.Property(t => t.BugId).HasColumnName("bs_bug");
            this.Property(t => t.UserId).HasColumnName("bs_user");
        }
    }
}
