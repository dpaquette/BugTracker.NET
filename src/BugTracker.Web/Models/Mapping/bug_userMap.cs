using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_userMap : EntityTypeConfiguration<bug_user>
    {
        public bug_userMap()
        {
            // Primary Key
            this.HasKey(t => new { t.bu_bug, t.bu_user, t.bu_flag, t.bu_seen, t.bu_vote });

            // Properties
            this.Property(t => t.bu_bug)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bu_user)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bu_flag)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bu_seen)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.bu_vote)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("bug_user");
            this.Property(t => t.bu_bug).HasColumnName("bu_bug");
            this.Property(t => t.bu_user).HasColumnName("bu_user");
            this.Property(t => t.bu_flag).HasColumnName("bu_flag");
            this.Property(t => t.bu_flag_datetime).HasColumnName("bu_flag_datetime");
            this.Property(t => t.bu_seen).HasColumnName("bu_seen");
            this.Property(t => t.bu_seen_datetime).HasColumnName("bu_seen_datetime");
            this.Property(t => t.bu_vote).HasColumnName("bu_vote");
            this.Property(t => t.bu_vote_datetime).HasColumnName("bu_vote_datetime");
        }
    }
}
