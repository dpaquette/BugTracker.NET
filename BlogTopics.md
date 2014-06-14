1.  Why are we evolving this application anyway? Talk about why we should keep things up to date while also covering why we may not wish to do a full rewrite. Netscape as a cautionary tale.
1. Changing to a modern source control tool.
 - What's wrong with leaving it where it is?
 - Lack of experience for new developers, lack of maintenance, changing workflows
 - Which modern tool should we use? Git, of course.
 - Should we attempt to preserve history?
 - Getting rid of unrelated artifacts (todo list sample code,...)
 - breaking out unrelated projects.(I'm not sure but the firefox folder looks to contain a plugin for firefox and chrome for taking screenshots... I can't think how that would even work)
1. Moving to a web project
 - What's the point?
 - How do I get all that code behind generated?
 - How can I move C# code out of the aspx file and into an aspx.cs file. (Dude, did you see that all the code is in C# script blocks inside the aspx files? I didn't even know you could do that)
 - What's the deal with namespaces?
 - Adding some hierarchy so you can actually find things
1. Dealing with old 3rd party dependencies
 - Do we still need them or does the functionality exist in the framework now?
 - Can we get them from nuget?
 - If not can we put them into nuget?
 - Are there more modern alternatives which can be shimmed in? Adapter pattern?
 - Using libraries instead of cut and paste code (POP3Client.cs)
1. Adding master pages to cut down on duplicate code
1. Extracting JavaScript to external files
 - namespacing javascript
 - bundling
1. Naming things properly
 - Using .net coding conventions and not... whatever this stuff is. SQL conventions, perhaps.  Probably PHP, actually: get_connection, execute_reader...
 - Don't preface class names with "My" we're not coding windows 95 here
 - Finding code issues before they find you
 - Look for long methods (hello btnet.MyPop3.fetch_messages() at ~250 lines)
 - Detecting duplicate code
1. Changing up the data access layer
 - Why move to EF
 - Defining models
 - Abstract and replace the data layers
 - Making use of dynamic queries (you should see some of the stuff my project does to avoid writing dynamic SQL)
1. Giving the UI a facelift
 - Introducing LESS or SASS
 - Improving layout
 - Including Bootstrap 
 - Responsive desing?
