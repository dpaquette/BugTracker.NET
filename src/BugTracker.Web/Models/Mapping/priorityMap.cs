using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class PriorityMap : EntityTypeConfiguration<Priority>
    {
        public PriorityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.BackgroundColor)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.Style)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("priorities");
            this.Property(t => t.Id).HasColumnName("pr_id");
            this.Property(t => t.Name).HasColumnName("pr_name");
            this.Property(t => t.SortOrder).HasColumnName("pr_sort_seq");
            this.Property(t => t.BackgroundColor).HasColumnName("pr_background_color");
            this.Property(t => t.Style).HasColumnName("pr_style");
            this.Property(t => t.Default).HasColumnName("pr_default");
        }
    }
}
