using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public int NonAdminsCanUse { get; set; }
        public int ExternalUser { get; set; }
        public int CanBeAssignedTo { get; set; }
        public int CanOnlySeeOwnReport { get; set; }
        public int CanEditSQL { get; set; }
        public int CanDeleteBug { get; set; }
        public int CanEditPosts { get; set; }
        public int CanMergeBugs { get; set; }
        public int CanMassEdit { get; set; }
        public int CanUseReports { get; set; }
        public int CanEditReports { get; set; }
        public int CanViewTasks { get; set; }
        public int CanEditTasks { get; set; }
        public int CanSearch { get; set; }
        public int OtherOrgsPermissionLevel { get; set; }
        public int CanAssignToInternalUsers { get; set; }
        public int CategoryFieldPErmissionLevel { get; set; }
        public int PriorityFieldPermissionLevel { get; set; }
        public int AssignedToFieldPermissionLevel { get; set; }
        public int StatusFieldPermissionLevel { get; set; }
        public int ProjectFieldPermissionLevel { get; set; }
        public int OrgFieldPermissionLevel { get; set; }
        public int UserDefinedFieldPermissionLevel { get; set; }
        public int TagsPermissionLevel { get; set; }
        public int Active { get; set; }
    }
}
