In the second post in this series we took a close look at the source code for BugTracker.NET. We found a number of issues ranging from security problems to out of date third party libraries to the lack of proper directory structure.

In this post and the few that follow we will explore how to deal with references to older 3rd party packages. It is worth reviewing each of these packages to check for the following:

- Is there a newer version of the package avaiable? Older versions may no longer be supported, or in the case of some web frameworks they may not work in new browsers.
- Is the package available on Nuget? Pulling from Nuget will save a lot of time for future updates.
- Are there better options for the packages? It is possible that the package has been superseded by some newer and better tool.

##.NET Packages
In BugTracker.NET, we have identified 3 different .NET packages that were being referenced: log4net, Lucene.NET and SharpMimeTools.

Log4net is a logging framework that provides a wrapper to various different logging mechanisms. It is a very commonly used framework that is not updated partiularly frequently.

Lucene.NET is a full text search engine. You can insert records into it and then have Lucene return the matching documents. Lucene is actually is the foundational technology behind Elastic Search as well as Solr.

Finally the SharpMimeTools library provides the ability to read and decode MIME messags. This is the encoding used in e-mail attachments. In BugTracker.NET it is used to allow reading e-mails sent to open bugs.

All appear to be very out-of-date. We'll take a look at each one of these to see what options we might have to replace them.

##JavaScript Libraries

Being a web application you can be fairly certain that there are some JavaScript libraries at work. Indeed we find that jQuery, a number of jQuery plugins and a rich text editor called CKEditor are all being used to some extent.

These packages are pretty much all available in nuget which is a far better option that manually downloading and including the packages. We'll try moving all of the third party JavaScript components to nuget and also reorganizing the code a little. The goal is to make the directory structure as similar as possible to what you would get creating a fresh project in Visual Studio. This will help out future developers.
