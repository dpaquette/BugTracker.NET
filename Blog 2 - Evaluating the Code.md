In the previous blog post in this series we blindly imported the entire project into git and pushed up to github. Having a good source control tool for the next few steps is crucial. There will certainly be times when you get an hour into refactoring, decide you've made a terrible mistake and need to back out. If you're using git and committing regularly then getting out of dodge shouldn't be a big ordeal.

Now that we have the source control set up we can start figuring out what our next steps are going to be. I won't worry too much about outlining what the solutions are in this post but I will attempt to identify problems. The remaining posts in this series will cover what actions we can take to improve our situation.

Opening the project up in Visual Studio the first thing I note is that this is an old school website project. This sort of project tends to encourage higitypigilty coding with source files just thrown anywhere. Indeed there are image files mixed in with .html files mixed in with JavaScript and CSS files. This could use a real good organization. So we would like to move to a web project and add some organiztion.

#Third Party Components

It looks like there might be some third party dependencies in the project. Just looking at the included DLLs I found:

 - log4net
 - Lucene
 - SharpMimeTools

The version numbers on these suggest that they're pretty old. There is nothing wrong as a whole with old packages but I'm always keen on updating packages because of potential security holes. Hopefully these packages will just be drop in upgrades without any need for fixes.  I do seem to remember that there was a breaking change to the log4net API that caused some complaints the time. That may need more work. We'll add it to the list of tasks.

There also seems to be a copy of jQuery and jQuery UI as well as a couple of other jQuery plugins. There are, in fact, 3 copies of various versions of jQuery, I'm not sure which is used so we should try to reduce that to just one version - probably a recent one.

There are a few other JavaScript libraries and components as well. CKEditor is the largest of these. Googling it we find that it is an open source text editor for the web. We also find that the version we have is from 2010 and that the project is very active. There must have been [40 releases](http://ckeditor.com/download/releases) since version 3.4.2. We'll put updating it on the list of tasks.

#Code Duplication

One of the best features of ASP.net is master pages. A master page can contain all of the common elements for a site. Things like navigation menus as well as JavaScript includes and CSS includes can live within a master page.

There are no master pages in BugTracker.net. If we open up a couple of files we can see that there are great similarities between them. They all have html elements and body elements as well as some very similar looking JavaScript. If we wanted to change the style of the pages or add a menu it would be quite an ordeal. The change would have to be made in every single page on the site.

We'll put adding a master page and hooking it into all the other pages as a task.

#Code Organization

If you start a new WebForms project you'll see that the pages that come out of the box have three parts:

 - .aspx - This is the HTML portion of the page. I like to think of it as containing all the presentation code.
 - .aspx.designer - This automatically generated file contains the definitions of any controls from the .aspx page that have runat=server set on them.
 - .aspx.cs - The other half of the partial class defined in the designer file. This file contains any of your own logic. I like to think of is as a controller. (Everything in my brain is related to MVC, MVP or MVVM.)

This division of logic and presentation is pretty loose in WebForms. The ease of mixing up presentation and business logic between these files is one of the reasons I don't like WebForms. You _can_ write good, well separated, testable WebForms but, by Thor's nostril hair, it is hard.

 The pages in BugTracker.Net contain all these parts in a single file. To be honest I didn't even know you could do that.  In my mind this is a big problem. We need to, at the very least, split up the single files into the standard triple of files. This will make testing easier as well as refactoring and even distribution.

 There are limited namespaces in the project. Namespaces are used to organize code into logical groups. They make it easier to reference one part of code from another and, shoudl it be needed, provide logical places to split the project into several projects.

 On to the list with you, code reorganization.

#Database Access

I figure an application like this must have some database behind it. So I set out to figure out how it works. I randomly selected a .aspx file and looked through for anything resembling data access. I found something pretty quickly

```
	sql =	"update bugs set
			bg_short_desc = N'$sd',
			bg_tags = N'$tags',
			bg_project = $pj,
			bg_org = $og,
			bg_category = $ct,
			bg_priority = $pr,
			bg_assigned_to_user = $au,
			bg_status = $st,
			bg_last_updated_user = $lu,
			bg_last_updated_date = @now,
			bg_user_defined_attribute = $udf
            $pcd_placeholder
			$custom_cols_placeholder
			where bg_id = $id
		end
		select @now";

	sql = sql.Replace("$sd", short_desc.Value.Replace("'","''"));
	sql = sql.Replace("$tags", tags.Value.Replace("'", "''"));
	sql = sql.Replace("$lu", Convert.ToString(security.user.usid));
	sql = sql.Replace("$id", Convert.ToString(id));
	sql = sql.Replace("$pj", new_project);
	sql = sql.Replace("$og", org.SelectedItem.Value);
	sql = sql.Replace("$ct", category.SelectedItem.Value);
	sql = sql.Replace("$pr", priority.SelectedItem.Value);
	sql = sql.Replace("$au", assigned_to.SelectedItem.Value);
	sql = sql.Replace("$st", status.SelectedItem.Value);
	sql = sql.Replace("$udf", udf.SelectedItem.Value);
	sql = sql.Replace("$snapshot_datetime", snapshot_timestamp.Value);

  ...
  DateTime last_update_date = (DateTime) btnet.DbUtil.execute_scalar(sql);
  ...
  foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)

```
So three things here: the first is that we're using SQL strings in the application instead of any sort of object relational mapper. That's probably okay, I do basically the same thing for applicaitons written using very lighweight ORMs like [Dapper](https://github.com/StackExchange/dapper-dot-net). We can put using an ORM on the list but kind of low down.

The second thing is that we're using data tables. I had some bad experiences with data tables years ago and have avoided them like the plague since them. You can see just by looking at the usage of them that they add a lot of layers of indirection that are unnecessary. I would much rather that we use simple IEnumerables with a strongly typed object. IEnumerable<Bug> here would be perfect.

I left the most important thing for last: sql string replacements. The parameters for the SQL are simply being inserted without sanitizing them or using parameters. This is a huge deal as it opens up the application to SQL injection attacks. With a well crafted entry into a field on the page a user could wipe out our whole database. Some attempts are being made to sanitize the strings doing a replace on single quotes. That isn't good enough. It is very easy to forget to escape something and not every field is escaped. Failing to use parameters is insecure programming and high on the list of common exploits. It is actually number 1 on the [OWASP top 10 list for 2013](https://www.owasp.org/index.php/Top_10_2013-Top_10). Fixing these strings should be a high priority.

#Styling and Various Other Things

The style of the website is a big outdated and could use a face lift. It may seem odd that we would put a styling change on the list but sometimes a slight improvment in a visual area of the program makes people forget about the problems elsewhere.

I once worked on a project which had a lot of problems. It was old code and difficult to maintain. People hated using it because it didn't match their needs and was slow. In the first new release of it I changed the background color from beige to blue. Nothing else. However for the next week I had a series of complements about how much faster and better the new version was. I had literally changed nothing but the color but people were so delighted to see a style change that they thought other improvements had been made.

This story isn't unique, there is a similarly [great story](http://thedailywtf.com/Articles/The-Cool-Cam.aspx) about saving a game through the use of UI tweak. UI might not seem like it is important but it changes people's perception of the application.

There are some other minor things that jumped otu at me as I was going thgouth the application. Inconsistent naming was one of the things that bugged me. By convention C# variables and methods don't use underscores but there lots of places that is done and even some places where variables are named with the "my" prefix: My_Lucene, My_Mime. These aren't a big deal but it will make lives easier for future developers if we fix them. 
