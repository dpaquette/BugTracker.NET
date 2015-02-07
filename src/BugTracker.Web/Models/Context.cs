using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using btnet.Models.Mapping;

namespace btnet.Models
{
    public partial class Context : DbContext
    {
        static Context()
        {
            Database.SetInitializer<Context>(null);
        }

        public Context()
            : base("Name=bugtrackerContext")
        {
        }

        public DbSet<BugPostAttachment> BugPostAttachments { get; set; }
        public DbSet<BugPost> BugPosts { get; set; }
        public DbSet<BugRelationShip> BugRelationShip { get; set; }
        public DbSet<BugSubscription> BugSubscription { get; set; }
        public DbSet<BugTask> BugTasks { get; set; }
        public DbSet<BugUser> BugUsers { get; set; }
        public DbSet<Bug> Bugs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CustomColumnsMetaData> CustomColumnsMetaDatas { get; set; }
        public DbSet<DashboardItems> DashboardItems { get; set; }
        public DbSet<EmailedLink> EmailedLinks { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<query> Queries { get; set; }
        public DbSet<QueuedNotification> QueuedNotification { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<session> Sessions { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<UserDefinedAttribute> UserDefinedAttributes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Votes> Votes { get; set; }

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
