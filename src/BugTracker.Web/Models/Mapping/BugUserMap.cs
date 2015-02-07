using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugUserMap : EntityTypeConfiguration<BugUser>
    {
        public BugUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.BugId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Flag)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Seen)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Vote)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("bug_user");
            this.Property(t => t.BugId).HasColumnName("bu_bug");
            this.Property(t => t.UserId).HasColumnName("bu_user");
            this.Property(t => t.Flag).HasColumnName("bu_flag");
            this.Property(t => t.FlagDate).HasColumnName("bu_flag_datetime");
            this.Property(t => t.Seen).HasColumnName("bu_seen");
            this.Property(t => t.SeenDate).HasColumnName("bu_seen_datetime");
            this.Property(t => t.Vote).HasColumnName("bu_vote");
            this.Property(t => t.VoteDate).HasColumnName("bu_vote_datetime");
        }
    }
}
