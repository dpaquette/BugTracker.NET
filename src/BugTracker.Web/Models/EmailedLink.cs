using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class EmailedLink
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> Salt { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
