using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class QueuedNotification
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public int BugId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public int Retries { get; set; }
        public string LastException { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
