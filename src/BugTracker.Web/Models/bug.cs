using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Bug
    {
        public int Id { get; set; }
        public string ShortDescription { get; set; }

        public int ReportedUserId { get; set; }
        public virtual User ReportedUser { get; set; }

        public DateTime ReportedDate { get; set; }

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

        public int PriorityId { get; set; }
        public virtual Priority Priority { get; set; }

        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public Nullable<int> AssignedToUserId { get; set; }
        public virtual User AssignedToUser { get; set; }

        public Nullable<int> LastUpdatedUserId { get; set; }
        public virtual User LastUpdatedUser { get; set; }

        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<int> UserDefinedAttributeId { get; set; }
        public string CustomDropDownValue1 { get; set; }
        public string CustomDropDownValue2 { get; set; }
        public string CustomDropDownValue3 { get; set; }
        public string Tags { get; set; }

        public virtual IList<BugTask> Tasks { get; set; }
    }
}
