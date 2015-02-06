using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using btnet.Models.Mapping;

namespace btnet.Models
{
    public partial class bugtrackerContext : DbContext
    {
        static bugtrackerContext()
        {
            Database.SetInitializer<bugtrackerContext>(null);
        }

        public bugtrackerContext()
            : base("Name=bugtrackerContext")
        {
        }

        public DbSet<BugPostAttachment> bug_post_attachments { get; set; }
        public DbSet<BugPost> bug_posts { get; set; }
        public DbSet<BugRelationShip> bug_relationships { get; set; }
        public DbSet<BugSubscription> bug_subscriptions { get; set; }
        public DbSet<BugTask> bug_tasks { get; set; }
        public DbSet<BugUser> bug_user { get; set; }
        public DbSet<Bug> bugs { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<CustomColumnsMetaData> custom_col_metadata { get; set; }
        public DbSet<DashboardItems> dashboard_items { get; set; }
        public DbSet<EmailedLink> emailed_links { get; set; }
        public DbSet<Organization> orgs { get; set; }
        public DbSet<Priority> priorities { get; set; }
        public DbSet<ProjectUser> project_user_xref { get; set; }
        public DbSet<Project> projects { get; set; }
        public DbSet<query> queries { get; set; }
        public DbSet<QueuedNotification> queued_notifications { get; set; }
        public DbSet<Report> reports { get; set; }
        public DbSet<session> sessions { get; set; }
        public DbSet<Status> statuses { get; set; }
        public DbSet<UserDefinedAttribute> user_defined_attribute { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Votes> votes_view { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BugPostAttachmentMap());
            modelBuilder.Configurations.Add(new BugPostMap());
            modelBuilder.Configurations.Add(new BugRelationShipMap());
            modelBuilder.Configurations.Add(new BugSubscriptionMap());
            modelBuilder.Configurations.Add(new BugTaskMap());
            modelBuilder.Configurations.Add(new BugUserMap());
            modelBuilder.Configurations.Add(new BugMap());
            modelBuilder.Configurations.Add(new CategoryMap());
            modelBuilder.Configurations.Add(new CustomColumnsMetaDataMap());
            modelBuilder.Configurations.Add(new DashboardItemsMap());
            modelBuilder.Configurations.Add(new EmailedLinkMap());
            modelBuilder.Configurations.Add(new OrganizationMap());
            modelBuilder.Configurations.Add(new PriorityMap());
            modelBuilder.Configurations.Add(new ProjectUserMap());
            modelBuilder.Configurations.Add(new ProjectMap());
            modelBuilder.Configurations.Add(new queryMap());
            modelBuilder.Configurations.Add(new QueuedNotificationMap());
            modelBuilder.Configurations.Add(new ReportMap());
            modelBuilder.Configurations.Add(new sessionMap());
            modelBuilder.Configurations.Add(new StatusMap());
            modelBuilder.Configurations.Add(new UserDefinedAttributeMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new VotesMap());
        }
    }
}
