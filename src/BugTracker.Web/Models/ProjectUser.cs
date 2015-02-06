using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class ProjectUser
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public int AutoSubscribe { get; set; }
        public int PermissionLevel { get; set; }
        public int AdminId { get; set; }
    }
}
