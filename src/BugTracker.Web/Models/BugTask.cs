using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugTask
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public int CreatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int LastUpdatedUserId { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Nullable<int> AssignedToUserId { get; set; }
        public Nullable<DateTime> PlannedStartDate { get; set; }
        public Nullable<DateTime> ActualStartDate { get; set; }
        public Nullable<DateTime> PlannedEndDate { get; set; }
        public Nullable<DateTime> ActualEndDate { get; set; }
        public Nullable<decimal> PlannedDuration { get; set; }
        public Nullable<decimal> ActualDuration { get; set; }
        public string DurationUnits { get; set; }
        public Nullable<int> PercentComplete { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> SortSequence { get; set; }
        public string Description { get; set; }
    }
}
