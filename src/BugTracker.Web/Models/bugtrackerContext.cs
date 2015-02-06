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

        public DbSet<bug_post_attachments> bug_post_attachments { get; set; }
        public DbSet<bug_posts> bug_posts { get; set; }
        public DbSet<bug_relationships> bug_relationships { get; set; }
        public DbSet<bug_subscriptions> bug_subscriptions { get; set; }
        public DbSet<bug_tasks> bug_tasks { get; set; }
        public DbSet<bug_user> bug_user { get; set; }
        public DbSet<bug> bugs { get; set; }
        public DbSet<category> categories { get; set; }
        public DbSet<custom_col_metadata> custom_col_metadata { get; set; }
        public DbSet<dashboard_items> dashboard_items { get; set; }
        public DbSet<emailed_links> emailed_links { get; set; }
        public DbSet<git_affected_paths> git_affected_paths { get; set; }
        public DbSet<git_commits> git_commits { get; set; }
        public DbSet<hg_affected_paths> hg_affected_paths { get; set; }
        public DbSet<hg_revisions> hg_revisions { get; set; }
        public DbSet<org> orgs { get; set; }
        public DbSet<priority> priorities { get; set; }
        public DbSet<project_user_xref> project_user_xref { get; set; }
        public DbSet<project> projects { get; set; }
        public DbSet<query> queries { get; set; }
        public DbSet<queued_notifications> queued_notifications { get; set; }
        public DbSet<report> reports { get; set; }
        public DbSet<session> sessions { get; set; }
        public DbSet<status> statuses { get; set; }
        public DbSet<svn_affected_paths> svn_affected_paths { get; set; }
        public DbSet<svn_revisions> svn_revisions { get; set; }
        public DbSet<user_defined_attribute> user_defined_attribute { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<votes_view> votes_view { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new bug_post_attachmentsMap());
            modelBuilder.Configurations.Add(new bug_postsMap());
            modelBuilder.Configurations.Add(new bug_relationshipsMap());
            modelBuilder.Configurations.Add(new bug_subscriptionsMap());
            modelBuilder.Configurations.Add(new bug_tasksMap());
            modelBuilder.Configurations.Add(new bug_userMap());
            modelBuilder.Configurations.Add(new bugMap());
            modelBuilder.Configurations.Add(new categoryMap());
            modelBuilder.Configurations.Add(new custom_col_metadataMap());
            modelBuilder.Configurations.Add(new dashboard_itemsMap());
            modelBuilder.Configurations.Add(new emailed_linksMap());
            modelBuilder.Configurations.Add(new git_affected_pathsMap());
            modelBuilder.Configurations.Add(new git_commitsMap());
            modelBuilder.Configurations.Add(new hg_affected_pathsMap());
            modelBuilder.Configurations.Add(new hg_revisionsMap());
            modelBuilder.Configurations.Add(new orgMap());
            modelBuilder.Configurations.Add(new priorityMap());
            modelBuilder.Configurations.Add(new project_user_xrefMap());
            modelBuilder.Configurations.Add(new projectMap());
            modelBuilder.Configurations.Add(new queryMap());
            modelBuilder.Configurations.Add(new queued_notificationsMap());
            modelBuilder.Configurations.Add(new reportMap());
            modelBuilder.Configurations.Add(new sessionMap());
            modelBuilder.Configurations.Add(new statusMap());
            modelBuilder.Configurations.Add(new svn_affected_pathsMap());
            modelBuilder.Configurations.Add(new svn_revisionsMap());
            modelBuilder.Configurations.Add(new user_defined_attributeMap());
            modelBuilder.Configurations.Add(new userMap());
            modelBuilder.Configurations.Add(new votes_viewMap());
        }
    }
}
