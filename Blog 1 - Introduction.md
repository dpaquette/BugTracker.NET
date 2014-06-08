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

For BugTracker.NET we decided to simply do an import of the current development snapshot. The commit comments in subversion are of low quality. Many of them are simply blank, those which are not are not helpful. "Fixed bug" is not sufficiently descriptive to be of any real help going forward. There are also no branches at all in the subversion repository. There are also no release tags. This means that subversion was basically just being used as a drop box.

For projects with a small team or, especially, those with a single team member this is the sort of thing you frequently see. People get lazy and don't keep up the discipline of working with code as if they were part of a larger team. Keeping disciplined is difficult but it will pay off in the end with a more stable code base which is more easily adopted by others. Chances are you're not goign to work on the project until you die so it is a professional courtesy to maintain a code base which can be quickly adopted by others.

If your project would benefit from importing from subversion then there are some excelent tools in place to help. It can be as simple as

    git svn clone http://url.to.svn

The initial BugTracker.net import into git and then into github was easy. The first step is to find a .gitignore file which is suitable for the project. This file lists which fiels to ignore when checking into git. Built binaires are generally not desirable artifacts to check into source control. The same is true for object files, the intermediaries between .cs and .exe. I personally use the example file from http://stackoverflow.com/questions/2143956/gitignore-for-visual-studio-projects-and-solutions although others swear by the github version https://github.com/github/gitignore/blob/master/VisualStudio.gitignore. Either of them is an excelent choice.

Place this file into a new directory and then copy the old source control folders from subversion ignore any .svn directories.

In our case this looks like

    gci -Recuse -include *.svn | ri
    git init
    notepad .gitignore (copy in the .gitignore from above)
    git add .
    git commit -m "Initial import from subversion"

Check the files in at once. You may be tempted to rearrange files before the first commit. You should avoid doing this as it breaks the history.

Next we just added github as a remote and pushed to it. We now have a great starting point for the rest of our project.

You can see the results of the import at https://github.com/dpaquette/BugTracker.NET/tree/MoveFromSourceForge.

In our next article we'll start looking at what the issues are with this project and we'll come up with a plan to update the project.
