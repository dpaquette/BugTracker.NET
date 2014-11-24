#Risky Business - Testing Large Changes
In the last post, we made fundamental changes to the way BugTracker.NET interacts with the database. These were huge changes that affected over 120 files. In relative terms, this was the riskiest change we have made to date. It should come as no surprise that we introduced some bugs when we made these changes.

Let's take a closer look at some strategies that can be used to mitigate the risks associated with making these types of changes.

##Automated Tests
In a perfect world, our project would have some form of automated tests that would give us nearly immediate feedback if our changes somehow broke the intended functionality. In this case, traditional unit test may not have actually helped. Automated tests would only have helped if we those tests were actually interacting with SQL Server to run the queries that we changed. Automated integration test or automated UI tests would have been fantastic in this situation.

Unfortunately, BugTracker.NET has no form of automated tests which  leaves us to explore other options.

##Code Reviews
Hopefully, you are working with a team of developers. With BugTracker.NET, we happen to be a distributed team of 2 developers. It's a small team, but it is big enough to make use of code reviews as a form of quality check.

Specifically, we are using a workflow commonly referred to as feature branches. When a developer or group of developers works on a feature or bug fix, the changes are done in a new branch off master. Once the feature is considered complete, a pull request is submitted in order to get the changes merged to master. The pull request is an encapsulation of all the changes that can be reviewed and discussed by the team. BitBucket refers to this workflow as [Feature Branch Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/feature-branch-workflow) while GitHub refers to it as [GitHub Flow](https://guides.github.com/introduction/flow/index.html). Regardless of what you call it, I would highly recommend this workflow for any team size greater than 0.

You can see the pull request that Simon submitted for his SQL Parameters changes [here](https://github.com/dpaquette/BugTracker.NET/pull/9). After completing a code review, I merged the changes to master. Unfortunately I merged the changes a little too hastily. In this case, I should have been more thorough and complete some manual testing before merging the changes to master.

##Manual Testing
Manual testing...that annoying thing that no developer wants to do. Unfortunately, it cannot be avoid
