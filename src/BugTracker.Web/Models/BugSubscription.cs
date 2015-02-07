using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugSubscription
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public int UserId { get; set; }
    }
}
