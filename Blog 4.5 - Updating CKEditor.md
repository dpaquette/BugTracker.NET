#Updating CKEditor

In the last blog post we looked at updating jQuery and a bunch of other components related to jQuery. We skipped looking at the CKEditor because, while writen in JavaScript, it does not have a dependency on jQuery. Some of you youngsters might not know this but it is actually possible to write JavaScript without referencing jQuery. I know what you're thinking: "Old man Timms has gotten into the elderberry wine again" but it is true.

[CKEditor](http://ckeditor.com/) is a rich text editor that allows users to add styles such as bold or underline to their bug reports. This is exactly the sort of thing "business" needs: pretty bugs. That being said, I enjoy using markdown on github so there is legitimate need for this.

##Finding CKEditor

In my previous looking through the application I hadn't actually noticed that CKEditor was being used. As it turns out there are some per user configuration settings hidden away inside the application. For the default admin user (user name admin, password admin by the way) the CKEditor is disabled. This flag can be set directly from the database or in the settings screen where it is called "edit text using fonts and colors". Now that we have that enabled we can see the CKEditor in action.

If we search through the application we find that there are only 3 places in which ckeditor.js is referenced. In fact they are the same places where the resizable text area from the last post is referenced. This makes sense because the code actually uses either the resizable editor or the CKEditor

```
<%

	if (security.user.use_fckeditor)
	{
		Response.Write ("CKEDITOR.replace( 'comment' )");
	}
	else
	{
		Response.Write("$('textarea').resizable();");
	}

	%>
```
There are a number of ways to start the editor but it looks like BugTracker.NET uses the id replacement method outlined in the [CKEditor documentation](http://nightly.ckeditor.com/14-09-21-06-07/standard/samples/replacebycode.html).  This doesn't seem to have changed between the version of the editor used in BugTracker.NET and the current version. This should make updating it pretty easy.

##Replacing CKEditor

Let's start by seeing what version of the editor exists in nuget. There are actually a [whole bunch](http://www.nuget.org/packages?q=ckeditor) of packages matching CKEditor. In this case the most popular download is actually not the best one. It is an older release, instead the [collection of packages](http://www.nuget.org/profiles/markdevilliers) by markdevilliers are the most recent. They are, in fact, right up to date: version 4.4.4.

Now we need to pick which of the 4 packages we need. Looking at the CKEditor download page the standard one looks to be the closest to the current version in BugTracker.NET.

In the package-manager console we enter

```
Install-Package ckeditor-standard -Version 4.4.4
```

This will install ckeditor in the Scripts directory: exactly where we would like to maintain consistency.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/5d1377d2e75f5b648317c91e799b6142b0785499)

To test it we just need to update the JavaScript include to reference the new editor location.

```
<%  if (security.user.use_fckeditor) { %>
<script type="text/javascript" src="scripts/ckeditor/ckeditor.js"></script>
<% } %>
```
[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/d9d00cd9e9b3b05ac624af16b93e3138e237d92f)

Now we can get rid of the old version of CKEditor

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/fc9b05b9c40fdc17515f48c5b0dbd2f0eb7d19a8)

##Next Steps

In the last few posts we've updated a large number of packages and components in BugTracker.NET. These updated packages provide a really solid foundation for the future. In our investigations, we've found a few worrying issues related to security:

 - Potential SQL injection
 - Weak password logic

 In the next series of posts we'll start addressing these issues.
