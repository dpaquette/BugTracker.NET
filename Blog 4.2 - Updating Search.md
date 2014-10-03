###Lucene.NET
Lucene.NET is a search engine library that BugTracker is using to implement text-based search.

![BugTracker Search Box](./Images/SearchBox.png)

BugTracker is referencing v2.4 of Lucene.NET but the current stable release is v3.0.3. Reviewing the [release notes](https://svn.apache.org/repos/asf/lucene.net/tags/Lucene.Net_3_0_3_RC2_final/CHANGES.txt), we see that there are a number of breaking changes in v3.0 as some deprecated methods were removed.

At this point, we need to make a decision whether or not we should stick with Lucene.NET or consider an alternative. It appears that Lucene.NET is not very active, the last release being 2 years ago.

A more modern and active project appears to be [ElasticSearch / NEST](https://github.com/elasticsearch/elasticsearch-net). ElasticSearch differs from Lucene.NET in that Lucene.NET lives entirely within the application process while ElasticSearch runs in a separate process, potentially on a difference server. This has huge implications relating to scalability. Let's consider a load-balancing scenario where we have 2 web server nodes running the BugTracker website. In this case, each web server instance would need to re-create and store the entire search index. Keeping in mind that the indexing process can be a resource intensive step, we can start to see how the Lucene.NET approach does not scale well.

By decoupling the web application from the search server, we are able to independently scale the web server and the search server. We could add web server nodes without effecting the search. Likewise, we could add search nodes without effecting our web server nodes.

Also, using Lucene.NET and storing the search index in the App_Data folder does not made the application very 'cloud-ready'.
In modern application deployment scenarios, such as deploying to the cloud, we want to maintain a separation between application and the infrastructure serving this data out. This allows us to scale them independently and reduces the chance of a catastrophic failure: if the web server goes down we can quickly start up another to replace it. In fact on Azure it is highly inadvisable to store anything more than transient cache data on the web tier. The Azure infrastructure will, from time to time, remove your web server node and replace it with an equivalent one in order to apply patches. While your code will be automatically redeployed any data it may have written to the local machine will be lost.

Giving the scalability concerns and some bad patterns identified in the BugTracker search code, it will be best to move the implementation to use ElasticSearch / NEST instead.

We will start by moving any inlined Lucene related code to code-behind files. This will make it easier to change the code.

[View the Commit](https://github.com/dpaquette/BugTracker.NET/commit/9577e2af0b34b0533297668f79c93c9760b13107)

Next, let's move the search implementation to a folder called Search and introduce an interface to abstract the specific implementation details. When the application uses search, it needs to do one of 2 things: Index a bug and Search for bugs.

    /// <summary>
    /// Provides full text based search of bugs
    /// </summary>
    public interface IBugSearch
    {
        /// <summary>
        /// Re-index all bugs.
        /// Warning: This is a CPU, Database and network intensive operation
        /// </summary>
        void IndexAll();

        /// <summary>
        /// Index of re-index the bug matching the specified id
        /// </summary>
        /// <param name="bugId">The id of the bug to index</param>
        void IndexBug(int bugId);

        /// <summary>
        /// Search for bugs based on the specified input text and security settings
        /// </summary>
        /// <param name="searchText">The user entered search text</param>
        /// <param name="security">The security settings for the current user</param>
        /// <returns>A dataset containing the search results</returns>
        DataSet Search(string searchText, Security security);
    }



Next, let's refactor the code in search_text.aspx.cs to use an instance of IBugSearch. Previously, search_text.aspx had to use several class from Lucene.NET and execute SQL queries. By moving some of these implementation details to BugSearch, we are able to greatly simplify the code in search_text.aspx by replacing over 150 lines of code with 2 simple lines:

     var search = BugSearchFactory.CreateBugSearch();
     var results = search.Search(Request["Query"], security);

Next, we need to add a reference to the NEST and ElasticSearch.NET packages:

    Install Package NEST -PreRelease

Now, we can create an implementation of the IBugSearch interface. Our implementation still has some messy details in it, but at least those details are hidden from the rest of the application. We can clean this up further once we refactor our data access implementation. Overall, the code in BugSearch.cs is much cleaner than the code in the original my_lucene.cs file.

[View BugSearch.cs](https://github.com/dpaquette/BugTracker.NET/blob/fdc471e0ce4900573d61e8873e2c662f94a62dce/src/BugTracker.Web/Search/BugSearch.cs)

Next, we will rename the 2 existing application settings related to search. EnableLucene will become EnableSearch and LuceneIndex will become SearchServerURI. These settings will be used by the BugSearchFactory when creating an instance of IBugSearch.

    <add key="EnableSearch" value="1"/>
    <add key="SearchServerURI" value="http://localhost:9200"/>

Finally, we need to review some code in the application startup. Currently, BugTracker re-indexes all bugs every time the application starts. This is a very costly operation that will be executed every single time the application pool restarts. This could be dozens of times per day depending on the IIS settings. Really, the index should only need to be completely re-created once when an ElasticSearch server is initially configured. Let's move the re-indexing code to make it an explicit action that is triggered by clicking a button on the admin.aspx page:

        public void ReindexAllBugs(object sender, EventArgs e)
        {
            if (Util.get_setting("EnableSearch", "1") == "1")
            {
                IBugSearch search = BugSearchFactory.CreateBugSearch();
                Task.Run(() => search.IndexAll());
                reindexLink.Enabled = false;
                reindexLink.Text = reindexLink.Text + " (Indexing in process)";
            }

        }

Our new search implementation is now complete. We can delete my_lucene.cs and remove the references to Lucene.NET and Highligther.NET.

[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/fdc471e0ce4900573d61e8873e2c662f94a62dce)

###Going Further
A big advantage of work we did to introduce an IBugSearch interface is that it is now much easier to swap our ElasticSearch implementation for an alternate search technology. The day after completing the work on this blog post, [Azure Search Public Preview](http://blogs.technet.com/b/dataplatforminsider/archive/2014/08/21/azure-previews-fully-managed-nosql-database-and-search-services.aspx) was released.  We didn’t get a chance to write an Azure Search version of Bug Search. We decided instead to leave that as an exercise for the reader. If anyone is brave enough to give it a try, feel free to submit your implementation as a pull request to the [GitHub repo](https://github.com/dpaquette/BugTracker.NET).
