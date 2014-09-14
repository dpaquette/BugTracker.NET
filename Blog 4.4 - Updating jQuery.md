#jQuery Update

jQuery was a revolution in how we use JavaScript. I'm not at all surprised to see that jQuery is included in BugTracker.Net.  As we discussed in section 2 there are a number of different versions of jQuery included in the project.

 - jquery-1.3.2.min.js
 - jquery-1.8.2.min.js
 - jquery-1.9.0.min.js

We would like to cut that down to a single, up to date, version. The latest version of jQuery, as of writing, is either 1.11.1 or 2.1.1.  About a year ago the jQuery foundation decided to branch jQuery into to two versions. The 1.X branch is designed to work on older versions of Internet Explorer up until IE8. The 2.X branch drops a lot of the dirty code that was needed to support old browsers. The uncompressed version of the 2.x branch is about 30 kib larger than the 1.x version as a result.

So now we have a decision to make: do we want to support older versions of Internet Explorer? It is a tough decision. If you're working on updating a production website then you can take a look at the logs to see which browsers are the most prevalent. That should give you some foundation on which to make a decision.

We don't have a production website at the moment so we don't have the best data available on which to make a decision. There are plenty of sites out there with browser statistics. The vary greatly from site to site and from region to region. For instance IE is very popular in South Korea due to government websites requiring it. Google and a number of other large companies have adopted as last two version policy. As IE 11 is the latest this would mean that IE 10 would be the oldest browser we would support.

Because of the general age of the code base I think it likely that some of the users will still be on older software. We'll stick with the 1.X branch for now but keep an eye on stats to see when we can upgrade.

Let's start by taking a look at how well used jQuery is before we start upgrading. The jQuery site suggests using a migration plugin for updating older code bases. If the usage of jQuery is sufficiently basic then there may be no need to do so.

##jQuery Usage

Weirdly the lack of master pages on the site is actually very helpful to us. Had the jQuery been included on a master page we would have had to check every part of the site. As it stands jQuery is only included in 10 files. This limits our search greatly. In fact there is no reference to the 1.9.0 version of jQuery so we can delete that right away.

The reset of the references seem to fall into 2 categories. Either jQuery is being used to do simple selection using CSS selectors or it is being used to support a jQuery plugin.

The plugins are

 - jQuery UI 1.7.2
 - jQuery Mobile 1.2.0
 - jQuery TextAreaResizer

 The first two are fairly well known jQuery plugins. As expected the versions in BugTracker.NET are pretty old ones. The latest jQuery UI is 1.11 and jQuery Mobile is at 1.4.3 at the time of writing. TextAreaResizer is a more difficult prospect.

 The version of the plugin included in BugTracker.NET is compressed and has comments stripped out. This means that there is no real way to figure out a source for the plugin. Googling around it seems like the plugin might be used to add resize handles to text areas. I was surprised by this as it was my understanding that this functionality was built into browsers. It seems that back in the IE8 days this didn't exist. A lone [demo page](http://itsavesyou.com/TextArea_Resizer_example.htm) was all I could find of the plugin. As we've decided to support browsers that old then we are going to need to keep this functionality.

 It is likely that we'll be able to replace the TextAreaResizer with a newer and better documented project.

In every case the usage of jQuery is either simplistic or it is tied to a plugin. If we update the plugins in line with the jQuery then there should be relativly little risk of breaking the plugin functionality.

##Updating jQuery

Now we have an idea of the problems with which we're dealing. Let's dive in and see if we can get jQuery updated.

To start with let's use the latest 1.X version of jQuery. This is available in nuget and we can install it with

```
Install-Package jQuery -version 1.11.1
```

This will create a Scripts directory at the base of the project. This is different from the directory that contains the existing jQuery files; they exist in a jquery directory. I like the scripts directory far more than a jquery directory. As we add more and more scripts that are not jQuery related naming the directory jquery does not make sense.

Now we can go through the application and replace all the script tags referencing old versions of jQuery with new ones.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/c6420d93fd81f84f7663521effb8ff14d452bbc1)

##Updating jQuery UI

Up next is to replace the old version of jQuery UI with a newer one. The latest is, coincidentally, also numbered 1.11.1. jQuery UI contains a large number of different controls. However, it is built in a way that permits assembling custom builds that only reference a couple of these widgets. If we open up the existing jQuery UI it looks like a lot of components are referenced.

[View the Code](https://github.com/dpaquette/BugTracker.NET/blob/3c64d84de9af96763713eae862d2b2eeeb1cf665/src/BugTracker.Web/jquery/jquery-ui-1.7.2.custom.min.js)

 However most of the components listed are actually behaviours like draggable, resizable and selectable. The only widgets we're actually using are

  - Dialog
  - Tabs
  - DatePicker

Packages for the individual jQuery UI widgets exist in nuget, however they are pretty out of date. In fact they are barely any newer than the version we have. We're going to need to assemble our own version. This can be done using the [jQuery UI download builder](http://jqueryui.com/download/#!version=1.11.1&components=1111111110011100000100000000000000000).

The package we download contains JavaScript, CSS and images. In the current version of BugTracker.NET these files are all included in the jquery directory. If we put all of them into the scripts directory we've added then we introduce confusion by mixing style and script.

Let's create a new directory called Content and add script and image directories under it. We've chosen to call the directory Content as that is a bit of a standard that has been established by ASP.net MVC projects. It isn't as hard of a standard as the one that Maven established in the Java world, but it will still help future developers find things.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/777264aac15f6cbfb4c3b1608591feebc5062f7d)

Again we need to do a pass through the application and update all the jQuery UI references.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/dd3e1e858f82111f9b997b94f13a8e78132eb48d)

Because we've put the images in a different directory from the CSS we'll need to update the CSS to reference the new path.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/aacf56e7806ee40149b61f0dcf285cda83b8b5de)

Now we can jump to some of the pages that use jQuery UI as well as jQuery and check if they work. search.aspx is a prime candidate. Indeed as we click around that page the data picker works as does everything else. I couldn't actually find anywhere that uses either the dialog or the tabs. Perhaps these were intended for future use. I'll leave the jQuery UI we added with those additional widgets for now, but we'll keep it in mind and review it again in a little while.

You don't have to make all the changes to a project at once. As I mentioned some time ago updating an application is like paying down a mortgage: chip away at it. It is, however, worth keeping around some notes.

##Updating jQuery UI

The latest of jQuery mobile is 1.4.4. Again this is a package that contains a number of components and we can pick and choose them. Let's poke about inside BugTracker.NET and see if we can figure out which components are being used.

It seems that jQuery mobile is only referenced in three places
 - mbug.aspx
 - mbugs.aspx
 - mlogin.aspx

 Looking at all of these files I cannot see any way in which jQuery mobile is actually being used. To be honest the 
