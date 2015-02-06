Database access is one of those things that is continually changing in the .net world. When I started doing .net development datatables and ADO.net were all the rage. Typically these were accessed over web services just to throw some unnecessary network latency into the mix. It was not at all uncommon to see a web application that called a web service, often on the same machine, that would access a database, extract a datatable and then serialize that and send it back over the network. In hindsight it was a terrible architecture but we didn't think that at the time. In a few years everything we believe is best practice now will also be considered wrong.

A> In the world of accounting the normal way to maintain a list of debits and credits is through a system known as "double entry bookkeeping". This system was developed in Italy some time [before the year 1400](https://en.wikipedia.org/wiki/Double-entry_bookkeeping_system#History). It remains more or less unchanged to this day. Whenever you're upset at the pace that things change in computer technology imagine how boring it must be to work in a field where all the major chalenges were solved 600 years ago.

I've used numerous other data access tools over the years

 - LINQ2SQL
 - SubSonic
 - Dapper
 - Entity Framework
 - NHibernate

to name just those I remember off the top of my head. All of these tools attempt to bridge the gap between the relational world of SQL databases and the object world in which we program.

If we take a look at BugTracker.net we can see that it predates any of these nifty object relational mapping technologies. It is still heavily dependant on using loosely typed collections such as DataSet to hold information from the database. There is a lot of overhead to these collections and we lose out on all the advantage of using a strongly typed language. Type conversion errors are not caught until run time.

Dynamic typing is fine if you have a good collection of unit tests to cover the implicit tests that the compiler performs. Unfortunatly BugTracker.NET is devoid of tests. This is a deficiency we're going to address at some point but first things first we should look at leveraging an object relational mapper to help out.

A> Many programmers are moving away from using object relational mappers(ORMs). The reasoning is that there is a discord between objects and relationships. This is, in effect, the law of leaky abstractions: as you add layers to cover complex actions with simple ones there are some nuances about the lower layers that are lost.

A> For instance the n+1 problem is not one that occurs outside of an object relational mapper. In order to address the shortcomings with object relational mappers some suggest completely abandoning them as being more trouble that they are worth. I think that ORMs are a great 80% solution. You cannot do everything with an ORM but the vast majority of tasks more easily and more reliably. For the places where some specialized functionality is needed (bulk operations, high performance,...) then you can drop to using lower level tools or tools that are specifically built for that scenario.

A> A very fair, if dated, treatment of the whole Object/Relationship mismatch is available on Ted Neward's blog entitled [The Vietnam of Computer Science]( http://blogs.tedneward.com/2006/06/26/The+Vietnam+Of+Computer+Science.aspx).

For future development on BugTracker.NET we would like to find an ORM based solution for accessing the database in net new code. Before we get to that point, however, we would like to address one of the pain points for BugTracker.NET: when we make changes to the database.

In the security chapter we updated the table structure of the database to account for the longer passwords and salts the new password hashing algorithms needed. To do this we opened up a rather sizeable file called upgrade.sql and appended

```
alter table users add password_reset_key nvarchar(200);
alter table users alter column us_salt nvarchar(200);
alter table users alter column us_password nvarchar(200);
```

We also updated the creation script to have these same changes. Manually updating the database structure like this is error prone and, as you can tell from the length of the file, time consuming. Because there is both an update.sql file and a full creation script the changes need to be made in two places, introducing another vector for errors. If you don't know the current version of your database then you don't know which part of the file to start at to bring your database up to date. It would be nice to have some tooling take care of this for us.

There are two very good options for versioning database structure:

1. SQL Server Project
2. Entity Framework Migrations

SQL Server Projects are special Visual Studio projects that break the current database structure into text files for tables, views, stored procedures and any other database object. During deployment the deployment tool will diff the structure of the database as described in the text files against the structure of an existing database and generate an upgrade script.

I'm a fan of SQL projects as they provide the ability to generate custom upgrade scripts while maintaining full control over the database. If developers are used to working with SQL this is a fantastic path to upgrade the way in which database updates are deployed. There is no need to abandon existing knowledge and move to another method of update databases.

However SQL projects require that all your developers use Visual Studio; a scenario that is becoming less common.

There are quite a few options available at the moment in the .net space for ORMs. With the release of newer versions of Microsoft's Entity Framework has really picked up its game to the point where it is as good as or better than any other ORM.

One of the features of EF is migrations. When building a model in EF migrations between one model state and another can be written in pure C#. These migrations are very useful when using a code first approach to Entity Framework. The next release of Entity Framework drops support for model and db first and only supports code first. So it is likely that migrations will become more common.

Each of these approaches has its advantages and disadvantages. For existing projects that may have made use of stored procedures or other artifacts of SQL Server an SQL Project is the better option.

This gives us our strategy for update the data access for the application: first we will extract an SQL project from an existing database instance built from the creation script. Second we'll generate a series of model classes and a datacontext allowing our database to be used from Entity Framework. Finally we'll pick a page in the application to update to use Entity Framework just as a demonstration.

##Generating an SQL Project

SQL projects are a fantastic tool for cleaning up existing databases as well as working with new databases. They are an evolution of the older database projects that were all the rage some years ago. We'll start by adding a new SQL project to the solution.

![New project](Images/new_project.jpg)

Here we've called the project BugTracker.Database.  Once we have the database right clicking on it we are presented with the option to import an existing database.

![Import database](Images/import_database.jpg)

Within that we can select our existing database that was built from the original SQL script. In my case this is the bugtracker database on localhost\sql2012.

![Select database](Images/import_select.jpg)

With that familiar dialog behind us we can alter some setting on the import database window. It is likely that you'll want to disable the import of referenced logins as they will be different from test to production.

![Import source](Images/import_source.jpg)

The tool will run for a while and generate a bunch of new files.

![Import running](Images/import_running.jpg)

The result is a couple of directories containing a file for each database object: table, stored proc, view, etc.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/b9088e46cd9e7951f80198e75454bb98df00b9db)

The SQL project brings with it the ability to deploy directly and to generate .dacpac files as part of a build. These .dacpac files are basically portable versions of the schema which can be compared with existing databases to generate diffs. It can even generate a full database if targeted at a blank database. This eliminates the need to know what version an existing database is to upgrade it.

##Generating EF Classes

Now that we have a reliable way to manage database scripts and to upgrade databases we can turn our attention to accessing the database.

There are a bunch of ways to use Entity Framework

- Model first
- Database first
- Code first

The model and database first approaches are being deprecated in future versions so let's take the code first approach. In the past I've built model classes by hand. This process is a bit slow and somewhat error prone, especially around the precision of decimals. David suggested that I try using the model generator tool to build models by looking at the database.

In Visual Studio install the Entity Framework Power Tools.

![Extensions](Images/extensions.jpg)

This brings us a new menu option: to reverse engineer a data model. Unfortunatly this sounds far cooler than it is and, unlike the movie Paycheck, Aaron Eckhart is unlikley to try to kill you for doing it.

![Reverse engineer code first](Images/reverse_engineer.jpg)

Running the tool on our web project will install the Entity Framework package from nuget and then build three different object types:

- A data context
- Model classes for each table
- Mapping companion classes for each model class

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/ddd3dfec3e049a1a387878905b1aacfea2d02158)

With these in place we're now able to make use of EF to access the database. However the classes that are generated are named as they are in the database which is not ideal for use in code. For instance the ```bug_tasks``` table has been mapped to a class called ```bug_tasks```. This is not, typically, how we would name classes in C#. Instead we would call this class ```BugTasks```. There are numerous places in BugTracker.net where the names don't look like C# names but, perhaps, more like PHP names. Updating all of these would be a monumental task so we'll take the approach of just fixing them when we happen to be in that section of code. For new code, however, we want to make sure it looks as clean as possible.

We're going to have to get into some code clean up to rename all of these something sensible.

##Cleaning up EF Classes

Let's start with the first model class that was generated: ```bug.cs```.

```
using System;
using System.Collections.Generic;

namespace btnet.Models
{
  public partial class bug
  {
    public int bg_id { get; set; }
    public string bg_short_desc { get; set; }
    public int bg_reported_user { get; set; }
    public System.DateTime bg_reported_date { get; set; }
    public int bg_status { get; set; }
    public int bg_priority { get; set; }
    public int bg_org { get; set; }
    public int bg_category { get; set; }
    public int bg_project { get; set; }
    public Nullable<int> bg_assigned_to_user { get; set; }
    public Nullable<int> bg_last_updated_user { get; set; }
    public Nullable<System.DateTime> bg_last_updated_date { get; set; }
    public Nullable<int> bg_user_defined_attribute { get; set; }
    public string bg_project_custom_dropdown_value1 { get; set; }
    public string bg_project_custom_dropdown_value2 { get; set; }
    public string bg_project_custom_dropdown_value3 { get; set; }
    public string bg_tags { get; set; }
  }
}
```
Immediately we can see a number of things that could be cleaned up. The ```System``` prefix on bg_reported_date is not necessary as we're importing the ```System``` namespace. The fields are also prefixed with ```bg_```. We don't need to prefix our variables in this fashion. The prefix doesn't provide any value, unlike those that one might see in C/C++ code for [Hungarian Notation](https://en.wikipedia.org/wiki/Hungarian_notation).

In EF Code First the names of the fields in the model objects must match the column names in the database unless explicitly overridden. The overriding can be done using an annotation on a filed like so

```
[ColumnName("bug_id")]
public int Id { get; set; }
```

Alternatively a mapping class can be provided that maps the model names to database names. This is one of the three objects that is provided by the reverse engineering tool. For ```bug.cs``` the mapping looks like

```
public class bugMap : EntityTypeConfiguration<bug>
{
  public bugMap()
  {
    // Primary Key
    this.HasKey(t => t.bg_id);

    // Properties
    this.Property(t => t.bg_short_desc)
    .IsRequired()
    .HasMaxLength(200);

    this.Property(t => t.bg_project_custom_dropdown_value1)
    .HasMaxLength(120);

    this.Property(t => t.bg_project_custom_dropdown_value2)
    .HasMaxLength(120);

    this.Property(t => t.bg_project_custom_dropdown_value3)
    .HasMaxLength(120);

    this.Property(t => t.bg_tags)
    .HasMaxLength(200);

    // Table & Column Mappings
    this.ToTable("bugs");
    this.Property(t => t.bg_id).HasColumnName("bg_id");
    this.Property(t => t.bg_short_desc).HasColumnName("bg_short_desc");
    this.Property(t => t.bg_reported_user).HasColumnName("bg_reported_user");
    this.Property(t => t.bg_reported_date).HasColumnName("bg_reported_date");
    this.Property(t => t.bg_status).HasColumnName("bg_status");
    this.Property(t => t.bg_priority).HasColumnName("bg_priority");
    this.Property(t => t.bg_org).HasColumnName("bg_org");
    this.Property(t => t.bg_category).HasColumnName("bg_category");
    this.Property(t => t.bg_project).HasColumnName("bg_project");
    this.Property(t => t.bg_assigned_to_user).HasColumnName("bg_assigned_to_user");
    this.Property(t => t.bg_last_updated_user).HasColumnName("bg_last_updated_user");
    this.Property(t => t.bg_last_updated_date).HasColumnName("bg_last_updated_date");
    this.Property(t => t.bg_user_defined_attribute).HasColumnName("bg_user_defined_attribute");
    this.Property(t => t.bg_project_custom_dropdown_value1).HasColumnName("bg_project_custom_dropdown_value1");
    this.Property(t => t.bg_project_custom_dropdown_value2).HasColumnName("bg_project_custom_dropdown_value2");
    this.Property(t => t.bg_project_custom_dropdown_value3).HasColumnName("bg_project_custom_dropdown_value3");
    this.Property(t => t.bg_tags).HasColumnName("bg_tags");
  }
}
```

Using a refactoring tool should allow us to change bug.cs and have the change reflected in the equivalent map file. This is a huge time saver so let's push on.

Some of the field names in ```bug.cs``` can simply have the ```bg_``` dropped and their underscore notation replace with Pascal Case, the standard for C#. However many columns could benifit from a bit more information being added. For instance ```bg_reported_user``` is not actually a user object so much as it is the Id of a user object. To make that clear we'll add the Id postfix to it.

After applying all our fixes the class looks like

```
public partial class bug
{
  public int Id { get; set; }
  public string ShortDescription { get; set; }
  public int ReportedUserId { get; set; }
  public DateTime ReportedDate { get; set; }
  public int StatusId { get; set; }
  public int PriorityId { get; set; }
  public int OrganizationId { get; set; }
  public int CategoryId { get; set; }
  public int ProjectId { get; set; }
  public Nullable<int> AssignedToUserId { get; set; }
  public Nullable<int> LastUpdatedUserId { get; set; }
  public Nullable<DateTime> LastUpdatedDate { get; set; }
  public Nullable<int> UserDefinedAttributeId { get; set; }
  public string CustomDropDownValue1 { get; set; }
  public string CustomDropDownValue2 { get; set; }
  public string CustomDropDownValue3 { get; set; }
  public string Tags { get; set; }
}
```

This is looking really good! The one final fix is to rename the class as a whole to ```Bug.cs```. For good measure we'll also rename the mapping file to ```BugMap.cs```.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/0a35f7a54be6d6e569c5482abd95dbc42e01b9b1)

Now we just need to work our way through the remaining classes and apply the same fixes.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/038bfd5b7d2c85f785973343eae336559babc587)

With the model and mapping classes taken care of we can now turn our attention to the final class that was added: the context. This is the class that holds all of the other added classes together. Our refactoring has already updated it significantly but the names of the collections should be updated to be well named.

```
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

```

I also renamed the class to Context from bugtrackerContext, it seemed reduandant to name the context after the project we're in. 

##Adding Migration Properties

##Migrating to EF
