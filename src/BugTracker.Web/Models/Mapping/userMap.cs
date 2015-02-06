using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.Salt)
                .HasMaxLength(200);

            this.Property(t => t.Password)
                .HasMaxLength(200);

            this.Property(t => t.FristName)
                .HasMaxLength(60);

            this.Property(t => t.LastName)
                .HasMaxLength(60);

            this.Property(t => t.EMail)
                .HasMaxLength(120);

            this.Property(t => t.Signature)
                .HasMaxLength(1000);

            this.Property(t => t.PasswordResetKey)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("users");
            this.Property(t => t.Id).HasColumnName("us_id");
            this.Property(t => t.UserName).HasColumnName("us_username");
            this.Property(t => t.Salt).HasColumnName("us_salt");
            this.Property(t => t.Password).HasColumnName("us_password");
            this.Property(t => t.FristName).HasColumnName("us_firstname");
            this.Property(t => t.LastName).HasColumnName("us_lastname");
            this.Property(t => t.EMail).HasColumnName("us_email");
            this.Property(t => t.AdminId).HasColumnName("us_admin");
            this.Property(t => t.DefaltQueryID).HasColumnName("us_default_query");
            this.Property(t => t.EnableNotifications).HasColumnName("us_enable_notifications");
            this.Property(t => t.AutoSubscribe).HasColumnName("us_auto_subscribe");
            this.Property(t => t.AutoSubscribeOwnBugs).HasColumnName("us_auto_subscribe_own_bugs");
            this.Property(t => t.AutoSubscribeReportedBugs).HasColumnName("us_auto_subscribe_reported_bugs");
            this.Property(t => t.SendNotificationsToSelf).HasColumnName("us_send_notifications_to_self");
            this.Property(t => t.Active).HasColumnName("us_active");
            this.Property(t => t.BugsPerPage).HasColumnName("us_bugs_per_page");
            this.Property(t => t.ForcedProject).HasColumnName("us_forced_project");
            this.Property(t => t.ReportedNotifications).HasColumnName("us_reported_notifications");
            this.Property(t => t.AssignedNotifications).HasColumnName("us_assigned_notifications");
            this.Property(t => t.SubscribedNotifications).HasColumnName("us_subscribed_notifications");
            this.Property(t => t.Signature).HasColumnName("us_signature");
            this.Property(t => t.UseEditor).HasColumnName("us_use_fckeditor");
            this.Property(t => t.EnableBugListPopUps).HasColumnName("us_enable_bug_list_popups");
            this.Property(t => t.CreatedUser).HasColumnName("us_created_user");
            this.Property(t => t.OrganizationId).HasColumnName("us_org");
            this.Property(t => t.LastLoginDate).HasColumnName("us_most_recent_login_datetime");
            this.Property(t => t.PasswordResetKey).HasColumnName("password_reset_key");
        }
    }
}
