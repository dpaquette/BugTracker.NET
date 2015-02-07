using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class BugTaskMap : EntityTypeConfiguration<BugTask>
    {
        public BugTaskMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DurationUnits)
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("bug_tasks");
            this.Property(t => t.Id).HasColumnName("tsk_id");
            this.Property(t => t.BugId).HasColumnName("tsk_bug");
            this.Property(t => t.CreatedUserId).HasColumnName("tsk_created_user");
            this.Property(t => t.CreatedDate).HasColumnName("tsk_created_date");
            this.Property(t => t.LastUpdatedUserId).HasColumnName("tsk_last_updated_user");
            this.Property(t => t.LastUpdatedDate).HasColumnName("tsk_last_updated_date");
            this.Property(t => t.AssignedToUserId).HasColumnName("tsk_assigned_to_user");
            this.Property(t => t.PlannedStartDate).HasColumnName("tsk_planned_start_date");
            this.Property(t => t.ActualStartDate).HasColumnName("tsk_actual_start_date");
            this.Property(t => t.PlannedEndDate).HasColumnName("tsk_planned_end_date");
            this.Property(t => t.ActualEndDate).HasColumnName("tsk_actual_end_date");
            this.Property(t => t.PlannedDuration).HasColumnName("tsk_planned_duration");
            this.Property(t => t.ActualDuration).HasColumnName("tsk_actual_duration");
            this.Property(t => t.DurationUnits).HasColumnName("tsk_duration_units");
            this.Property(t => t.PercentComplete).HasColumnName("tsk_percent_complete");
            this.Property(t => t.StatusId).HasColumnName("tsk_status");
            this.Property(t => t.SortSequence).HasColumnName("tsk_sort_sequence");
            this.Property(t => t.Description).HasColumnName("tsk_description");
        }
    }
}
