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

###Choosing a Framework

The 2 most common logging frameworks in .NET are [log4net](http://logging.apache.org/log4net/) and [NLog](http://nlog-project.org/). Both are acceptable choices and we won't get into the details of [comparing the 2 frameworks](http://stackoverflow.com/questions/710863/log4net-vs-nlog). We are going to move forward with NLog as it is a little newer and seems to have a more active community.

First, we need to remove the reference to the old log4net assembly and delete the file from the references folder. Now we can add a reference to the latest version of NLog using the Nuget Pacakge Manager Console (Tools -> Nuget Package Manager -> Package Manager Console).

    Install-Package NLog

###Configuring NLog

Next, we need to configure NLog. This can be done using [xml configuration files](https://github.com/nlog/nlog/wiki/Configuration-file), which is very flexible but also a little messy. Since we are only looking for very basic file logging to replace the existing logging functionality, we would prefer to [configure logging programatically](https://github.com/nlog/nlog/wiki/Configuration-API) in C#.

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

###Replacing Custom Logging Code

Now that NLog is configured, we can start replacing the old custom logging code.

Starting with the code in Globa.asax.cs, replace the logging code with the new NLog logging code:

    Logger logger = LogManager.GetCurrentClassLogger();
    logger.Fatal(exc);

###Resolving Relative Paths
While testing this change, we appear to have encountered our first [yak that requires some shaving](http://www.hanselman.com/blog/YakShavingDefinedIllGetThatDoneAsSoonAsIShaveThisYak.aspx).

The call to Util.get_log_folder() in LoggingConfig.Configure fails because of the weird way the application is attempting to resolve relative paths using some value that is cached in the HttpRuntime. Let's simplify this approach by using a more standard and fail-safe method of resolving relative paths.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/bb3a03586eec4e325edae5a42c1cce960d1d506e)

Now that the error logging is working as expected, let's replace the code in Util.write_to_log with the following simplified logging code:

    Logger log = LogManager.GetCurrentClassLogger();
    log.Debug(s);

Finally, we can delete the method Util.get_log_file_path since it is no longer used.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/dd2ea87538c3c48f6a3a44220a14d51e38124fe6)
