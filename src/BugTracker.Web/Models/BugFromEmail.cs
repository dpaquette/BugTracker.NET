using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace btnet.Models
{
    public class BugFromEmail
    {
        public int? ProjectId { get; set; }
        public string Comment { get; set; }
        
        [Required]
        public string FromAddress { get; set; }
        public string CcAddress { get; set; }
        public string Message { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachmentContentType { get; set; }
        public string AttachmentFileName { get; set; }
        public string AttachmentDescription { get; set; }
        public int BugId { get; set; }
        public string ShortDescription { get; set; }

        public int? OrganizationId { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? AssignedTo { get; set; }
        public int? UdfId { get; set;}                    
    }
}