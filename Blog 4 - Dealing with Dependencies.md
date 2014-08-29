In this post, we will explore how to deal with references to older 3rd party packages.

When dealing with older .NET applications, there is a good chance the application will be referencing a number of 3rd party packages. It is worth reviewing each of these packages to check for the following:

- Is there a newer version of the package avaiable? Older versions may no longer be supported, or in the case of some web frameworks they may not work in new browsers.
- Is the package available on Nuget?
- Are there better options for the packages

#.NET Packages
In BugTracker.NET, we have identified 3 different .NET packages that were being referenced: log4net, Lucene.NET and SharpMimeTools. All appear to be very out-of-date so let's take a look at what we can do to upgrade these.

****Give a brief overview of each and link to the individual blog posts

Updating these packages has provided us with a number of benefits:

- Ability to easily update using NuGet
- Significant simplification of BugTracker code base
- Large reduction of overall code to maintain
- Using common and current packages with better community support

#JavaScript Libraries
