using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class bug_post_attachments
    {
        public int bpa_id { get; set; }
        public int bpa_post { get; set; }
        public byte[] bpa_content { get; set; }
    }
}
