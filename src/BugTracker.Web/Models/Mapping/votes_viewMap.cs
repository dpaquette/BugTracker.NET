using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class votes_viewMap : EntityTypeConfiguration<votes_view>
    {
        public votes_viewMap()
        {
            // Primary Key
            this.HasKey(t => t.vote_bug);

            // Properties
            this.Property(t => t.vote_bug)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("votes_view");
            this.Property(t => t.vote_bug).HasColumnName("vote_bug");
            this.Property(t => t.vote_total).HasColumnName("vote_total");
        }
    }
}
