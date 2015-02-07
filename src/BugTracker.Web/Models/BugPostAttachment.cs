using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugPostAttachment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public byte[] Content { get; set; }
    }
}
