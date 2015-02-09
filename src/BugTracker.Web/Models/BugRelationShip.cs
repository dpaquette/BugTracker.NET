using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugRelationShip
    {
        public int Id { get; set; }
        public int Bug1Id { get; set; }
        public int Bug2Id { get; set; }
        public string Type { get; set; }
        public int Direction { get; set; }
    }
}
