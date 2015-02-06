using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace btnet.Models.Mapping
{
    public class userMap : EntityTypeConfiguration<user>
    {
        public userMap()
        {
            // Primary Key
            this.HasKey(t => t.us_id);

            // Properties
            this.Property(t => t.us_username)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.us_salt)
                .HasMaxLength(200);

            this.Property(t => t.us_password)
                .HasMaxLength(200);

            this.Property(t => t.us_firstname)
                .HasMaxLength(60);

            this.Property(t => t.us_lastname)
                .HasMaxLength(60);

            this.Property(t => t.us_email)
                .HasMaxLength(120);

            this.Property(t => t.us_signature)
                .HasMaxLength(1000);

            this.Property(t => t.password_reset_key)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("users");
            this.Property(t => t.us_id).HasColumnName("us_id");
            this.Property(t => t.us_username).HasColumnName("us_username");
            this.Property(t => t.us_salt).HasColumnName("us_salt");
            this.Property(t => t.us_password).HasColumnName("us_password");
            this.Property(t => t.us_firstname).HasColumnName("us_firstname");
            this.Property(t => t.us_lastname).HasColumnName("us_lastname");
            this.Property(t => t.us_email).HasColumnName("us_email");
            this.Property(t => t.us_admin).HasColumnName("us_admin");
            this.Property(t => t.us_default_query).HasColumnName("us_default_query");
            this.Property(t => t.us_enable_notifications).HasColumnName("us_enable_notifications");
            this.Property(t => t.us_auto_subscribe).HasColumnName("us_auto_subscribe");
            this.Property(t => t.us_auto_subscribe_own_bugs).HasColumnName("us_auto_subscribe_own_bugs");
            this.Property(t => t.us_auto_subscribe_reported_bugs).HasColumnName("us_auto_subscribe_reported_bugs");
            this.Property(t => t.us_send_notifications_to_self).HasColumnName("us_send_notifications_to_self");
            this.Property(t => t.us_active).HasColumnName("us_active");
            this.Property(t => t.us_bugs_per_page).HasColumnName("us_bugs_per_page");
            this.Property(t => t.us_forced_project).HasColumnName("us_forced_project");
            this.Property(t => t.us_reported_notifications).HasColumnName("us_reported_notifications");
            this.Property(t => t.us_assigned_notifications).HasColumnName("us_assigned_notifications");
            this.Property(t => t.us_subscribed_notifications).HasColumnName("us_subscribed_notifications");
            this.Property(t => t.us_signature).HasColumnName("us_signature");
            this.Property(t => t.us_use_fckeditor).HasColumnName("us_use_fckeditor");
            this.Property(t => t.us_enable_bug_list_popups).HasColumnName("us_enable_bug_list_popups");
            this.Property(t => t.us_created_user).HasColumnName("us_created_user");
            this.Property(t => t.us_org).HasColumnName("us_org");
            this.Property(t => t.us_most_recent_login_datetime).HasColumnName("us_most_recent_login_datetime");
            this.Property(t => t.password_reset_key).HasColumnName("password_reset_key");
        }
    }
}
