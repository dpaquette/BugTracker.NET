---
layout: post
title:  "Evaluating the Code"
date:   2014-08-26
categories: asp.net
---

In the previous blog post in this series we blindly imported the entire project into git and pushed up to github. Having a good source control tool for the next few steps is crucial. There will certainly be times when you get an hour into refactoring, decide you've made a terrible mistake and need to back out. If you're using git and committing regularly then getting out of dodge shouldn't be a big ordeal.

Now that we have the source control set up we can start figuring out what our next steps are going to be. I won't worry too much about outlining what the solutions are in this post but I will attempt to identify problems. The remaining posts in this series will cover what actions we can take to improve our situation.

Opening the project up in Visual Studio the first thing I note is that this is an old school website project. This sort of project tends to encourage higitypigilty coding with source files just thrown anywhere. Indeed there are image files mixed in with .html files mixed in with JavaScript and CSS files. This could use a real good organization. So we would like to move to a web project and add some organiztion. 