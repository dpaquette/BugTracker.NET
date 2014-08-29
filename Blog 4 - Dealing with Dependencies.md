In this post, we will explore how to deal with references to older 3rd party packages.

When dealing with older .NET applications, there is a good chance the application will be referencing a number of 3rd party packages. It is worth reviewing each of these packages to check for the following:

- Is there a newer version of the package avaiable? Older versions may no longer be supported, or in the case of some web frameworks they may not work in new browsers.
- Is the package available on Nuget?
- Are there better options for the packages

#.NET Packages
In BugTracker.NET, we have identified 3 different packages that were being referenced: log4net, Lucene.NET and SharpMimeTools. All appear to be very out-of-date so let's take a look at what we can do to upgrade these.

###Logging
BugTracker is referencing v1.1.4322 of log4net. The current supported version of log4net is v1.2.13 and it is available on [Nuget](https://www.nuget.org/packages/log4net/). Upon reviewing the [release notes](http://logging.apache.org/log4net/release/release-notes.html), we can see that there are a large number of bug fixes between v1.1.4322 and v1.2.13. Unfortunately, there are also a number of breaking changes in the v1.2 release. We will need to review how BugTracker is using log4net to assess the difficulty of upgrading to the latest.

To our surprise, BugTracker is referencing the log4net package but never actually uses the log4net. After doing a little digging, we see some custom logging code in Global.asax:

        string path = Util.get_log_file_path();
        // open file
        StreamWriter w = File.AppendText(path);

        w.WriteLine("\nTIME: " + DateTime.Now.ToLongTimeString());
        w.WriteLine("MSG: " + exc.Message);
        w.WriteLine("URL: " + Request.Url);
        w.WriteLine("EXCEPTION: " + exc);
        w.WriteLine(server_vars_string.ToString());
        w.Close();

There are definitely problems with this code. There is one other method in util.cs that does some logging. This method is overly complex and definitely needs some fixing. Rather than try to fix these methods, we are going to replace the custom logging code with a standard .NET logging package.

The 2 most common logging frameworks in .NET are [log4net](http://logging.apache.org/log4net/) and [NLog](http://nlog-project.org/). Both are acceptable choices and we won't get into the details of [comparing the 2 frameworks](http://stackoverflow.com/questions/710863/log4net-vs-nlog). We are going to move forward with NLog as it is a little newer and seems to have a more active community.

First, we need to remove the reference to the old log4net assembly and delete the file from the references folder. Now we can add a reference to the latest version of NLog using the Nuget Pacakge Manager Console (Tools -> Nuget Package Manager -> Package Manager Console).

    Install-Package NLog

Next, we need to configure NLog. This can be done using [xml configuration files](https://github.com/nlog/nlog/wiki/Configuration-file), which is very flexible but also a little messy. Since we are only looking for very basic file logging to replace the existing logging functionality, we would prefer to [configure logging programatically](https://github.com/nlog/nlog/wiki/Configuration-API) in C#. This approach is becoming more common today.

Let's start by creating a App_Start folder that will contain classes that are used to configure components of the application when it starts up. This is a pattern that is used in newer ASP.NET applications and can be seen when creating a new ASP.NET application in Visual Studio 2013.

In the startup folder, we will create a static LogConfig class that will contain a static Configure method with all our log4net configuration.

    public static class LoggingConfig
    {
        public static void Configure()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget();

            fileTarget.FileName = Path.Combine(Util.get_log_folder(), "btnet_log.txt");
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
            fileTarget.ArchiveEvery = FileArchivePeriod.Day;
            config.AddTarget("File", fileTarget);


            //Turn logging on/off based on the LogEnabled setting
            var logLevel = Util.get_setting("LogEnabled", "1") == "1" ? LogLevel.Trace: LogLevel.Off;
            config.LoggingRules.Add(new LoggingRule("*", logLevel, fileTarget));

            LogManager.Configuration = config;
        }
    }

In Global.asax.cs, call the logging configuration method in the Application_OnStart method:

    LoggingConfig.Configure();

Now that NLog is configured, we can start replacing the old custom logging code.

Starting with the code in Globa.asax.cs, replace the logging code with the new NLog logging code:

    Logger logger = LogManager.GetCurrentClassLogger();
    logger.Fatal(exc);

While testing this change, we appear to have encountered our first [yak that requires some shaving](http://www.hanselman.com/blog/YakShavingDefinedIllGetThatDoneAsSoonAsIShaveThisYak.aspx).

The call to Util.get_log_folder() in LoggingConfig.Configure fails because of the weird way the application is attempting to resolve relative paths using some value that is cached in the HttpRuntime. Let's simplify this approach by using a more standard and fail-safe method of resolving relative paths.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/bb3a03586eec4e325edae5a42c1cce960d1d506e)

Now that the error logging is working as expected, let's replace the code in Util.write_to_log with the following simplified logging code:

    Logger log = LogManager.GetCurrentClassLogger();
    log.Debug(s);

Finally, we can delete the method Util.get_log_file_path since it is no longer used.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/dd2ea87538c3c48f6a3a44220a14d51e38124fe6)

###Lucene.NET
Lucene.NET is a search engine library that BugTracker is using to implement text-based search.

![BugTracker Search Box](./Images/SearchBox.png)

BugTracker is referencing v2.4 of Lucene.NET but the current stable release is v3.0.3. Reviewing the [release notes](https://svn.apache.org/repos/asf/lucene.net/tags/Lucene.Net_3_0_3_RC2_final/CHANGES.txt), we see that there are a number of breaking changes in v3.0 as some deprecated methods were removed.

At this point, we need to make a decision whether or not we should stick with Lucene.NET or consider an alternative. It appears that Lucene.NET is not very active, the last release being 2 years ago.

A more modern and active project appears to be [ElasticSearch / NEST](https://github.com/elasticsearch/elasticsearch-net). ElasticSearch differs from Lucene.NET in that Lucene.NET lives entirely within the application process while ElasticSearch runs in a separate process, potentially on a difference server. This has huge implications relating to scalability. Let's consider a load-balancing scenario where we have 2 web server nodes running the BugTracker website. In this case, each web server instance would need to re-create and store the entire search index. Keeping in mind that the indexing process can be a resource intensive step, we can start to see how the Lucene.NET approach does not scale well.

By decoupling the web application from the search server, we are able to independently scale the web server and the search server. We could add web server nodes without effecting the search. Likewise, we could add search nodes without effecting our web server nodes.

Also, using Lucene.NET and storing the search index in the App_Data folder does not made the application very 'cloud-ready'.
In modern application deployment scenarios, such as deploying to the cloud, we want to maintain a separation between application and the infrastructure serving this data out. This allows us to scale them independently and reduces the chance of a catastrophic failure: if the web server gees down we can quickly start up another to replace it. In fact on Azure it is highly inadvisable to store anything more than transient cache data on the web tier. The Azure infrastructure will, from time to time, remove your web server node and replace it with an equivalent one in order to apply patches. While your code will be automatically redeployed any data it may have written to the local machine will be lost.

Giving the scalability concerns and some bad patterns identified in the BugTracker search code, it will be best to move the implementation to use ElasticSearch / NEST instead.

We will start by moving any inlined Lucene related code to code-behind files. This will make it easier to change the code.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/9577e2af0b34b0533297668f79c93c9760b13107)

Next, let's move the search implementation to a folder called Search and introduce an interface to abstract the specific implementation details. When the application uses search, it needs to do one of 2 things: Index a bug and Search for bugs.

    /// <summary>
    /// Provides full text based search of bugs
    /// </summary>
    public interface IBugSearch
    {
        /// <summary>
        /// Re-index all bugs.
        /// Warning: This is a CPU, Database and network intensive operation
        /// </summary>
        void IndexAll();

        /// <summary>
        /// Index of re-index the bug matching the specified id
        /// </summary>
        /// <param name="bugId">The id of the bug to index</param>
        void IndexBug(int bugId);

        /// <summary>
        /// Search for bugs based on the specified input text and security settings
        /// </summary>
        /// <param name="searchText">The user entered search text</param>
        /// <param name="security">The security settings for the current user</param>
        /// <returns>A dataset containing the search results</returns>
        DataSet Search(string searchText, Security security);
    }



Next, let's refactor the code in search_text.aspx.cs to use an instance of IBugSearch. Previously, search_text.aspx had to use several class from Lucene.NET and execute SQL queries. By moving some of these implementation details to BugSearch, we are able to greatly simplify the code in search_text.aspx by replacing over 150 lines of code with 2 simple lines:

     var search = BugSearchFactory.CreateBugSearch();
     var results = search.Search(Request["Query"], security);

Next, we need to add a reference to the NEST and ElasticSearch.NET packages:

    Install Package NEST -PreRelease

Now, we can create an implementation of the IBugSearch interface. Our implementation still has some messy details in it, but at least those details are hidden from the rest of the application. We can clean this up further once we refactor our data access implementation. Overall, the code in BugSearch.cs is much cleaner than the code in the original my_lucene.cs file.

[View BugSearch.cs](https://github.com/dpaquette/BugTracker.NET/blob/fdc471e0ce4900573d61e8873e2c662f94a62dce/src/BugTracker.Web/Search/BugSearch.cs)

Next, we will rename the 2 existing application settings related to search. EnableLucene will become EnableSearch and LuceneIndex will become SearchServerURI. These settings will be used by the BugSearchFactory when creating an instance of IBugSearch.

    <add key="EnableSearch" value="1"/>
    <add key="SearchServerURI" value="http://localhost:9200"/>

Finally, we need to move review some code in the application startup. Currently, BugTracker re-indexes all bugs every time the application starts. This is a very costly operation that will be executed every single time the application pool restarts. This could be dozens of times per day depending on the IIS settings. Really, the index should only need to be completely re-created once when an ElasticSearch server is initially configured. Let's move the re-indexing code to make it an explicit action that is triggered by clicking a button on the admin.aspx page:

        public void ReindexAllBugs(object sender, EventArgs e)
        {
            if (Util.get_setting("EnableSearch", "1") == "1")
            {
                IBugSearch search = BugSearchFactory.CreateBugSearch();
                Task.Run(() => search.IndexAll());
                reindexLink.Enabled = false;
                reindexLink.Text = reindexLink.Text + " (Indexing in process)";
            }

        }

Our new search implementation is now complete. We can delete my_lucene.cs and remove the refernces to Lucene.NET and Highligther.NET.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/fdc471e0ce4900573d61e8873e2c662f94a62dce)

###SharpMimeTools and Pop3
SharpMimeTools is an open-source MIME parser / decoder. BugTracker usies this library to parse information from incoming emails and to format email messages when sending bug notifications.

It is unclear if BugTracker is referencing the latest version of ShareMimeTools because assembly is version 0.0.0.0. SharpMimeTools is not available in NuGet and according the the [SharpMimeTools website](http://anmar.eu.org/projects/sharpmimetools/), the latest news was in 2006. This is probably a good time to start looking for an alternative, more actively supported library.

After a quick search on NuGet, we found that [OpenPop.NET](https://www.nuget.org/packages/OpenPop.NET/) seems to be a popular option. In fact, we can also use OpenPop.NET to replace a large block of [Pop3 code](https://github.com/dpaquette/BugTracker.NET/blob/3c64d84de9af96763713eae862d2b2eeeb1cf665/src/BugTracker.Web/btnet/POP3Client.cs) that was downloaded from CodeProject in 2003.

After further review of the usage of POP3Client and SharpMimeTools has revealed a serious design problem. BugTracker is attempting to run a recurring background task in the web application that pools for email in a pop3 account. This type of polling code would be better suited as a Windows Service, Scheduled Task or Azure Web Job. It is the type of work that should run out-of-process as it is dangerous and difficult to consistently run [background tasks in a web application](http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx/).

Luckily, there is already a Windows Service and Console Application that also implements this email polling logic. Let's include these existing projects in the BugTracker.NET solution.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/ee6a479ffed16bae588e945a831ab117e075fea3)

Next, let's delete the experimental code in BugTracker.Web and move the inlined C# code in insert_bug.aspx code to a code-behind file.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/b73a0a3d61db538b138d3c8d6bdaca08a5924ddb)

The code in the console application polls for email using Pop3Client, then from any new email found it creates new bugs by posting the raw email message to insert_bug.aspx. Insert_bug.aspx then uses SharpMimeTools to parse the email message and create a new bug. Let's refactor the code in the console application to use OpenPop.NET.

We will need to install the OpenPop.NET package in the btnet_service and btnet_console projects:

    Install-Package OpenPop.NET

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/3438da9cf977dfaea2d9b8c23a109fe932a596de)

Now we can refactor the polling code in POPMain.cs to use OpenPop.NET and delete Pop3Client.cs.

Testing the pop3 functionality is a little tricky as we need a pop3 server to talk to. I followed the instructions on Peter Kellner's blog to [setup a local test instance of hmailserver](http://peterkellner.net/2012/03/11/how-to-setup-your-own-pop3imap-email-server-for-local-development-testing/).

The code is still a little messy, but we have managed it clean it up a lot by getting rid of the custom Pop3 implementation. We deleted about 600 lines of code in this commit and have moved to a Pop3 implementation that is likely more standard and is definitely more trustworthy.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/337646cbdcdfb566a1edb9e994c190a899caa202)

Now, the last step is to replace any use of SharpMimeTools from BugTracker.Web. Let's add a reference to OpenPop.NET to the BugTracker.Web project, then remove reference to SharpMimeTools. Since this was the last reference to SharpMimeTools, we can delete the file from the references folder.

Again, the code is not as clean as it could be but we have made a major improvement. We deleted over 200 lines of rather obscure mime parsing code from Mime.cs.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/0ed3d6e0ceff535fa9e57fe0cda534b2f689edc7)

While we are in the mood to delete some code, it looks like we had the source code from SharpMimeTools hanging around. We won't be needing that anymore so let's delete it.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/e3d6b4144d6e1d893e247dbc90a870b4ff860258)

#JavaScript Libraries
