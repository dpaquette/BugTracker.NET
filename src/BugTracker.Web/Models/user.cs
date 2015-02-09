using System;
using System.Collections.Generic;

namespace btnet.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }
        public string FristName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }
        public int AdminId { get; set; }
        public int DefaltQueryID { get; set; }
        public int EnableNotifications { get; set; }
        public int AutoSubscribe { get; set; }
        public Nullable<int> AutoSubscribeOwnBugs { get; set; }
        public Nullable<int> AutoSubscribeReportedBugs { get; set; }
        public Nullable<int> SendNotificationsToSelf { get; set; }
        public int Active { get; set; }
        public Nullable<int> BugsPerPage { get; set; }
        public Nullable<int> ForcedProject { get; set; }
        public int ReportedNotifications { get; set; }
        public int AssignedNotifications { get; set; }
        public int SubscribedNotifications { get; set; }
        public string Signature { get; set; }
        public int UseEditor { get; set; }
        public int EnableBugListPopUps { get; set; }
        public int CreatedUser { get; set; }
        public int OrganizationId { get; set; }
        public Nullable<DateTime> LastLoginDate { get; set; }
        public string PasswordResetKey { get; set; }
    }
}
