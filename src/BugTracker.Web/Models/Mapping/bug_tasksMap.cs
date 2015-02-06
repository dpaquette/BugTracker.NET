using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class bug_tasksMap : EntityTypeConfiguration<bug_tasks>
    {
        public bug_tasksMap()
        {
            // Primary Key
            this.HasKey(t => t.tsk_id);

            // Properties
            this.Property(t => t.tsk_duration_units)
                .HasMaxLength(20);

            this.Property(t => t.tsk_description)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("bug_tasks");
            this.Property(t => t.tsk_id).HasColumnName("tsk_id");
            this.Property(t => t.tsk_bug).HasColumnName("tsk_bug");
            this.Property(t => t.tsk_created_user).HasColumnName("tsk_created_user");
            this.Property(t => t.tsk_created_date).HasColumnName("tsk_created_date");
            this.Property(t => t.tsk_last_updated_user).HasColumnName("tsk_last_updated_user");
            this.Property(t => t.tsk_last_updated_date).HasColumnName("tsk_last_updated_date");
            this.Property(t => t.tsk_assigned_to_user).HasColumnName("tsk_assigned_to_user");
            this.Property(t => t.tsk_planned_start_date).HasColumnName("tsk_planned_start_date");
            this.Property(t => t.tsk_actual_start_date).HasColumnName("tsk_actual_start_date");
            this.Property(t => t.tsk_planned_end_date).HasColumnName("tsk_planned_end_date");
            this.Property(t => t.tsk_actual_end_date).HasColumnName("tsk_actual_end_date");
            this.Property(t => t.tsk_planned_duration).HasColumnName("tsk_planned_duration");
            this.Property(t => t.tsk_actual_duration).HasColumnName("tsk_actual_duration");
            this.Property(t => t.tsk_duration_units).HasColumnName("tsk_duration_units");
            this.Property(t => t.tsk_percent_complete).HasColumnName("tsk_percent_complete");
            this.Property(t => t.tsk_status).HasColumnName("tsk_status");
            this.Property(t => t.tsk_sort_sequence).HasColumnName("tsk_sort_sequence");
            this.Property(t => t.tsk_description).HasColumnName("tsk_description");
        }
    }
}
