#Updating the Bugs Grid
With the bugs grid, we need to be a little more careful because we could conceivably have 10s of thousands of bugs stored in the system. In this case, we will probably want to use AJAX sourced data instead of DOM sourced. If I was building this system from scratch, I would have a Web API end-point that served JSON data. The client would make HTTP requests to the server, passing search, sorting and paging parameters. The server would use pass those parameters to the SQL server and all the filtering, sorting and paging would be done in SQL. This is hands down the most efficient way to do this. It minimizes the amount of data passed between the SQL server, the Web Server and the client. Requests and responses would be small, which would help ensure optimal throughput.

But, we are not building BugTracker.NET from scratch. We have an existing system that we are trying to improve.

First, let's review how the bugs.aspx page works today. Here is a screenshot of the current grid:

![The original Bugs grid](Images/BugsGridOriginal.png)

Users can interact with this grid in a number of ways. They can select different queries from the Query dropdown. Selecting a difference query executes a completely different SQL query and renders a completely different grid with different columns. Clicking on the column header will sort the result set by that column. Some of the columns have dropdowns that allow for filtering. Selecting a value from one of these dropdows will filter the grid to only rows that match the selected value for that column.

Behind the scenes, this page works with by submitting a form back to the bugs.aspx page whenever the user completes one of these actions. The form contains hidden fields to indicate which query to execute, which column to sort by and the selected filters.

```
<input type="hidden" name="new_page" id="new_page" runat="server" value="0" />
<input type="hidden" name="actn" id="actn" runat="server" value="" />
<input type="hidden" name="filter" id="filter" runat="server" value="" />
<input type="hidden" name="sort" id="sort" runat="server" value="-1" />
<input type="hidden" name="prev_sort" id="prev_sort" runat="server" value="-1" />
<input type="hidden" name="prev_dir" id="prev_dir" runat="server" value="ASC" />
<input type="hidden" name="tags" id="tags" value="" />
```
All the filtering, sorting and paging is done on the Web server, not the database server. This is probably acceptable in for many scenarios but it is far from ideal. For example, imagine the grid displays bugs in pages of 25 at a time. What would happen if we had 100,000 bugs in BugTracker and we are trying to display the last 25 bugs in the list of 100,000.

First, the Web server would send a query to the database and load all 100,000 rows in a DataSet in .NET. Right off the bat, this is consuming a lot of unnecessary bandwidth between the database and the web server. It will also cause a big spike in memory usage by the web server. Next, the server will create a DataView from the DataSet and apply sorting and filtering. This much less efficient than asking the database to perform these operations. Databases are VERY good at sorting and filtering. DataViews can do an okay job, but they just aren't optimized to the same extend as a database server is. Finally, let's look at the C# code that handles paging.

```
int rows_this_page = 0;
int j = 0;

foreach (DataRowView drv in dv)
{
    // skip over rows prior to this page
    if (j < bugsPerPage * this_page)
    {
        j++;
        continue;
    }
    // do not show rows beyond this page
    rows_this_page++;
    if (rows_this_page > bugsPerPage)
    {
        break;
    }
    //Render the table row
}
```

As you can see, rendering the last page in the table involves iterating through every single row in the table. This is not a responsible use of the web server's CPU. Upon further review of the code, I can see that rendering the grid involves iterating over each row again for each column that contains a dropdown filter. That means that displaying ANY page of a grid involves iterating over every row several times. To make things even harder on server, it appears that the full unfiltered datatable is being stored in session state: `(DataTable)HttpContext.Current.Session["bugs_unfiltered"]`.

The approach used to render this grid might result in acceptable performance for a small number of total bugs and a small number of users, it just won't handle load well. Unlike the admin pages, the bugs page needs to be as efficient as possible. It is the main page of the application and we should expect that every single user of BugTracker might be using it at the same time.

Given the current implementation, we can safely assume that the BugTracker deployments do not typically contain thousands of active bugs. Regardless, I would like to aim for better performance on this page. The current implementation is also very confusing. It took me several days to make sense of it. Let's see if we can both improve the performance and maintainability of this code.

One of the biggest challenges here is the fact that this page can execute  arbitrary queries that return any number of columns. This seems to be an important extensibility point in BugTracker so I would like to keep this feature.

If we want to handle filtering, sorting and paging on the database server, then we will to extend the BugTracker queries feature.

```
declare @ME int
set @ME = 1

declare @order_by nvarchar(255)
set @order_by = '$FLAG'

declare @sort_direction nvarchar(5)
set @sort_direction = 'DESC'

declare @offset int;
set @offset = 0;

declare @page_size int;
set @page_size = 5;

declare @organization_filter nvarchar(max)
SET @organization_filter = 'org1'

SELECT * FROM (
select isnull(pr_background_color,'#ffffff') as bg_color, bg_id [id], isnull(bu_flag,0) [$FLAG],
 bg_short_desc [desc], isnull(pj_name,'') [project], isnull(og_name,'') [organization], isnull(ct_name,'') [category], rpt.us_username [reported by],
 bg_reported_date [reported on], isnull(pr_name,'') [priority], isnull(asg.us_username,'') [assigned to],
 isnull(st_name,'') [status], isnull(lu.us_username,'') [last updated by], bg_last_updated_date [last updated on]
 from bugs
 left outer join bug_user on bu_bug = bg_id and bu_user = @ME
 left outer join users rpt on rpt.us_id = bg_reported_user
 left outer join users asg on asg.us_id = bg_assigned_to_user
 left outer join users lu on lu.us_id = bg_last_updated_user
 left outer join projects on pj_id = bg_project
 left outer join orgs on og_id = bg_org
 left outer join categories on ct_id = bg_category
 left outer join priorities on pr_id = bg_priority
 left outer join statuses on st_id = bg_status

 ) t
 WHERE (@organization_filter IS NULL OR t.[organization] = @organization_filter)
 order by
	CASE WHEN @ORDER_BY = 'id' AND @sort_direction = 'DESC' THEN [id] END DESC,
	CASE WHEN @ORDER_BY = '$FLAG' AND @sort_direction = 'DESC' THEN [$flag] END DESC,

	CASE WHEN @ORDER_BY = 'id' AND @sort_direction = 'ASC' THEN [id] END ASC,
	CASE WHEN @ORDER_BY = '$FLAG' AND @sort_direction = 'ASC' THEN [$flag] END ASC

OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY;

```
##Explain the BugQueryExecutor

###Adding Web API
Create a folder named Controllers. Right click on the Controllers folder and select Add Controller. Select Web API 2. Visual studio will add the references to Web API and provide instructions on how to configure it. Follow those instructions by adding `GlobalConfiguration.Configure(WebApiConfig.Register);` to the Application Start method of global.asax.cs.

That's it...now we can use Web API in our project.

```
[Authorize]
public class BugQueryController : ApiController
{
    public IHttpActionResult Get()
    {
        return Ok("Test");
    }
}
```

Calling the BugQueryExeccutor from WebAPI controller

```
Code sample goes here
```

###Udpating the existing queries in the database
Write a SQL migration script.
