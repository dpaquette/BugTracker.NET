using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class VotesMap : EntityTypeConfiguration<Votes>
    {
        public VotesMap()
        {
            // Primary Key
            this.HasKey(t => t.BugId);

            // Properties
            this.Property(t => t.BugId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("votes_view");
            this.Property(t => t.BugId).HasColumnName("vote_bug");
            this.Property(t => t.Total).HasColumnName("vote_total");
        }
    }
}
