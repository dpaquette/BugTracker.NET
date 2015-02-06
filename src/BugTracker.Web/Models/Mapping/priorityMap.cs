using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class priorityMap : EntityTypeConfiguration<priority>
    {
        public priorityMap()
        {
            // Primary Key
            this.HasKey(t => t.pr_id);

            // Properties
            this.Property(t => t.pr_name)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(t => t.pr_background_color)
                .IsRequired()
                .HasMaxLength(14);

            this.Property(t => t.pr_style)
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("priorities");
            this.Property(t => t.pr_id).HasColumnName("pr_id");
            this.Property(t => t.pr_name).HasColumnName("pr_name");
            this.Property(t => t.pr_sort_seq).HasColumnName("pr_sort_seq");
            this.Property(t => t.pr_background_color).HasColumnName("pr_background_color");
            this.Property(t => t.pr_style).HasColumnName("pr_style");
            this.Property(t => t.pr_default).HasColumnName("pr_default");
        }
    }
}
