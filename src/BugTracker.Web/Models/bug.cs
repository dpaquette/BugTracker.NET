using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Bug
    {
        public int Id { get; set; }
        public string ShortDescription { get; set; }
        public int ReportedUserId { get; set; }
        public DateTime ReportedDate { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public int OrganizationId { get; set; }
        public int CategoryId { get; set; }
        public int ProjectId { get; set; }
        public Nullable<int> AssignedToUserId { get; set; }
        public Nullable<int> LastUpdatedUserId { get; set; }
        public Nullable<DateTime> LastUpdatedDate { get; set; }
        public Nullable<int> UserDefinedAttributeId { get; set; }
        public string CustomDropDownValue1 { get; set; }
        public string CustomDropDownValue2 { get; set; }
        public string CustomDropDownValue3 { get; set; }
        public string Tags { get; set; }
    }
}
