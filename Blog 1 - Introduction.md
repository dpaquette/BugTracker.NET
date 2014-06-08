I'm always surprised when I think about how long ago it was that ASP.net came out. Although I had a pretty big break in the middle there I started working with ASP.net a dozen years ago. When I started .net was pre version 1.0 and all the cool kids hung out on some website created by a couple of guys from Harvard. I think it was called devhood (I'm having trouble remembering that far back).

The point here is that .net isn't the young development environment it once was. The original release was in February of 2002 this means that there has been a decade of applications developed on the platform which now need maintaining. It has been a pretty busy decade with a great number of changes and advances in what is considered to be best practice.

For the most part the applications developed over the years have just been left to their own devices. New features have been built on top of old with very little regard for how to keep the application healthy. A great deal can be done layering new functionality over old but eventually you end up with a cobbled together application which is difficult to expand and to test.

Both David and I inherited what I would call legacy ASP.net applications that are in need of updating. My application is a core application which is crucial to the business it powers. Everybody at the company logs into the application every single day. We need the application to survive another decade so it needs to be updated and refined to use modern techniques and ideas.

We were fortunate enough to be brought in and given a pretty free hand with updating our respective applications. This is particularly helpful as frequently the political difficulties around updating applications are far more difficult to handle than the technical ones.

We couldn't find much in the way of guidance for updating older applications so we thought we would team up and write a series of blog posts. We hope to cover everything needed to bring an older application up to date.

Since our applications are super secret private applications we need an open source application which exhibits the same legacy issues. Fortunately we found an application which fits the bill perfectly: BugTracker.NET. We dropped the author, Corey Trager a line and he was gracious enough to let us use his application as a base for this series of articles.

# Getting Started -  Source Control

The very first step in bringing a legacy application into the modern world is to update the source control. Source control is a very important piece of the puzzle. It governs how a team works together and it provides a central hub for collaboration. BugTracker.NET is hosted in subversion on sourceforge. This isn't going to cut it for our current needs.

We would like to move the solution over to git and more than that to github. Git is pretty much the defacto source control tool these days. Normally I would argue that we should investigate all the possible source control tools and evaluate them and the such. I'm not going to argue that, though, and the reason is that git has won. A monoculture is dangerous but here we are.

We need to evaluate if it is worthwhile to put in the effort to maintain the history of the project when moving to a new source control system or to just take the latest snapshot and call the history lost. When making that decision I look a a couple of factors:

1. Are the commit comments of high quality? That is to say do they lend good information for understanding the project's history.
2. Are there multiple branches which contain partially complete features?
3. Are there historical releases which need to be maintained from different release branches?

For BugTracker.NET we decided to simply do an import of the current development snapshot. The commit comments in subversion are of low quality. Many of them are simply blank, those which are not are not helpful. "Fixed bug" is not sufficiently descriptive to be of any real help going forward. 
