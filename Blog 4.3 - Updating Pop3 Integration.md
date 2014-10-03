###SharpMimeTools and Pop3
SharpMimeTools is an open-source MIME parser / decoder. BugTracker uses this library to parse information from incoming emails and to format email messages when sending bug notifications.

It is unclear if BugTracker is referencing the latest version of ShareMimeTools because assembly is version 0.0.0.0. SharpMimeTools is not available in NuGet and according the the [SharpMimeTools website](http://anmar.eu.org/projects/sharpmimetools/), the latest news was in 2006. This is probably a good time to start looking for an alternative, more actively supported library.

After a quick search on NuGet, we found that [OpenPop.NET](https://www.nuget.org/packages/OpenPop.NET/) seems to be a popular option. In fact, we can also use OpenPop.NET to replace a large block of [Pop3 code](https://github.com/dpaquette/BugTracker.NET/blob/3c64d84de9af96763713eae862d2b2eeeb1cf665/src/BugTracker.Web/btnet/POP3Client.cs) that was downloaded from CodeProject in 2003.

After further review of the usage of POP3Client and SharpMimeTools has revealed a serious design problem. BugTracker is attempting to run a recurring background task in the web application that polls for email in a pop3 account. This type of polling code would be better suited as a Windows Service, Scheduled Task or Azure Web Job. It is the type of work that should run out-of-process as it is dangerous and difficult to consistently run [background tasks in a web application](http://haacked.com/archive/2011/10/16/the-dangers-of-implementing-recurring-background-tasks-in-asp-net.aspx/).

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
