using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugUser
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public int UserId { get; set; }
        public int Flag { get; set; }
        public Nullable<DateTime> FlagDate { get; set; }
        public int Seen { get; set; }
        public Nullable<DateTime> SeenDate { get; set; }
        public int Vote { get; set; }
        public Nullable<DateTime> VoteDate { get; set; }
    }
}
