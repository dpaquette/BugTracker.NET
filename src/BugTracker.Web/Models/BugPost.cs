using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class BugPost
    {
        public int Id { get; set; }
        public int BugId { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public string CommentSearch { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string File { get; set; }
        public Nullable<int> Size { get; set; }
        public string ContentType { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> OriginalCommentId { get; set; }
        public int HideFromExternalUsers { get; set; }
        public string CCEmail { get; set; }
    }
}
