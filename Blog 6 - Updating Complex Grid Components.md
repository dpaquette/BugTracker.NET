#Updating the Bugs Grid
In the last section, we showed how easy it is to update some of the simpler grids. In this section, we will take a look at a more complicated example.

With the bugs grid, we need to be a little more careful because we could conceivably have 10s of thousands of bugs stored in the system. In this case, we will probably want to use AJAX sourced data instead of DOM sourced. If I was building this system from scratch, I would create a Web API end-point that served JSON data. The client would make HTTP requests to the server, passing search, sorting and paging parameters. The server would use pass those parameters to the SQL server and all the filtering, sorting and paging would be done in SQL. This is hands down the most efficient way to do this. It minimizes the amount of data passed between the SQL server, the web server and the client. Requests and responses would be small, which would help ensure better throughput for the web server.

But, we are not building BugTracker.NET from scratch. We have an existing system that we are trying to improve.

##Current Approach

First, let's review how the bugs.aspx page works today. Here is a screenshot of the current grid:

![The original Bugs grid](Images/BugsGridOriginal.png)

Users can interact with this grid in a number of ways. They can select different queries from the Query dropdown. Selecting a differt query executes a completely different SQL query and renders a completely different grid with different columns. Clicking on a column header will sort the result set by that column. Some of the columns have dropdowns that allow for filtering. Selecting a value from one of these dropdowns will filter the grid to only rows that match the selected value for that column. If the query returns a large number of bugs, then only 25 bugs will be displayed at a time. In this case, links to show the next and previous 25 bugs will be displayed at the bottom of the grid. This is all functionality that we would expect from a data grid in a business application. We are going to want to keep this base level of functionality.

Behind the scenes, this bugs page works by submitting a form back to the bugs.aspx page whenever the user completes one of these actions mentioned above. The form contains hidden fields to indicate which query to execute, which column to sort by and the selected filters. For example, sorting by a particular column will result in a full refresh of the page. This is a common approach for older ASP.NET applications but it leads to a less than optimal user experience. Ideally, only the grid itself would update.

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

First, the Web server would send a query to the database and load all 100,000 rows in a DataSet in .NET. Immediately we can see some wasted resources here because this is consuming a lot of unnecessary bandwidth between the database and the web server. It will also cause a big spike in memory usage by the web server. Next, the server will create a DataView from the DataSet and apply sorting and filtering. This much less efficient than asking the database to perform these operations. Databases are VERY good at sorting and filtering. DataViews can do an okay job, but they just aren't optimized to the same extend as a database server is. Finally, let's look at the C# code that handles paging.

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

The approach used to render this grid might result in acceptable performance for a small number of total bugs and a small number of users, but it just won't handle load well. Unlike the admin pages, the bugs page needs to be as efficient as possible. It is the main page of the application and we should expect that every single user of BugTracker might be using it at the same time.

Given the current implementation, we can safely assume that the BugTracker deployments do not typically contain thousands of active bugs. Regardless, I would like to aim for better performance on this page. The current implementation is also very confusing. It took me several days to make sense of it. Let's see if we can both improve the performance and maintainability of this code.

#Filtering, Sorting and Paging in SQL
One of the biggest challenges here is the fact that this page can execute  arbitrary queries that return any number of columns. This seems to be an important extensibility point in BugTracker so I would like to keep this feature.

If we want to handle filtering, sorting and paging on the database server, then we will to extend the BugTracker queries feature.

The base installation of BugTracker contains about 10 different queries that can be executed on the bugs.aspx page. These can be added to by administrators, but they all look a little something like this:

```
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
```

In order to support sorting, filtering and paging on in SQL along with dynamic queries like this, we will need to make some compromises in terms of the query.

We can achieve this by dynamically writing a query that wraps the original query and applying sorting, filtering and paging using some SQL parameters. An abbreviated example of the query above would look something like this:

```
SELECT * FROM (
  select bg_id [id], isnull(pj_name,'') [project], isnull(og_name,'') [organization]
   from bugs
   left outer join projects on pj_id = bg_project
   left outer join orgs on og_id = bg_org
   ) t
 WHERE (@organization_filter IS NULL OR t.[organization] = @organization_filter)
 AND (@project_filter IS NULL OR t.[project] = @project_filter)
 order by
	CASE WHEN @ORDER_BY = 'id' AND @sort_direction = 'DESC' THEN [id] END DESC,
	CASE WHEN @ORDER_BY = 'project' AND @sort_direction = 'DESC' THEN [project] END DESC,
  CASE WHEN @ORDER_BY = 'organization' AND @sort_direction = 'DESC' THEN [organization] END DESC,
  CASE WHEN @ORDER_BY = 'id' AND @sort_direction = 'ASC' THEN [id] END ASC,
	CASE WHEN @ORDER_BY = 'project' AND @sort_direction = 'ASC' THEN [project] END ASC,
  CASE WHEN @ORDER_BY = 'organization' AND @sort_direction = 'ASC' THEN [organization] END ASC
OFFSET @offset ROWS
FETCH NEXT @page_size ROWS ONLY;
```

There are a some important things to point out about this query. First, I am using the OFFSET and FETCH syntax which was introduced in SQL Server 2012. If we needed to support older versions of SQL Server, the same thing could be accomplished using the ROW_NUMBER approach. Also, both the WHERE clause and ORDER BY clauses are not optimal. For example, would be far more optimal to apply the WHERE clause to the inner query against the id of organization table rather than applying it against the organization name on the outer query. This is a trade-off I was willing to take to allow for the existing extensibility and we will do some tests to ensure performance of the new approach is still acceptable.

##Wrapping the existing queries
To wrap the existing queries, I created something called the BugQueryExecutor. This class takes in an instance of the existing queries and dynamically builds the wrapping query with the filtering, sorting and paging applied. Calling execute will return a datatable with only a the rows for specified page. It will also return a count of the total number of rows before and after filtering was applied. This will be useful so we know when to show the Next / Prev buttons and to give the user some indication of how many bugs were returned by the query vs. the number of bugs that are actually stored in BugTracker.

```
public class BugQueryExecutor
{
    private readonly Query _query;
    private readonly string[] _columnNames;
    private const int MaxLength = 5000;

    public BugQueryExecutor(Query query)
    {
        _query = query;
        _columnNames = query.ColumnNames;
    }

    public BugQueryResult ExecuteQuery(IIdentity identity, int start, int length, string orderBy,
        string sortDirection, BugQueryFilter[] filters = null)
    {
        return ExecuteQuery(identity, start, length, orderBy, sortDirection, false, filters);
    }

    public BugQueryResult ExecuteQuery(IIdentity identity, int start, int length, string orderBy, string sortDirection, bool idOnly, BugQueryFilter[] filters = null)
    {
        if (!string.IsNullOrEmpty(orderBy) && !_columnNames.Contains(orderBy))
        {
            throw new ArgumentException("Invalid order by column specified: {0}", orderBy);
        }
        string columnsToSelect = idOnly ? "id" : "*";
        var initialSql = string.Format("SELECT t.{0} FROM ({1}) t",columnsToSelect, GetInnerSql(identity));
        SQLString sqlString = new SQLString(initialSql);
        var initialCountSql = string.Format("SELECT COUNT(*) FROM ({0}) t", GetInnerSql(identity));
        SQLString countSqlString = new SQLString(initialCountSql);
        SQLString countUnfilteredSqlString = new SQLString(initialCountSql);
        int userId = identity.GetUserId();
        sqlString.AddParameterWithValue("@ME", userId);
        countSqlString.AddParameterWithValue("@ME", userId);
        countUnfilteredSqlString.AddParameterWithValue("@ME", userId);


        if (filters != null && filters.Any())
        {
            ApplyWhereClause(sqlString, filters);
            ApplyWhereClause(countSqlString, filters);
        }

        sqlString.Append(" ORDER BY ");

        sqlString.Append(BuildDynamicOrderByClause());

        sqlString.AddParameterWithValue("ORDER_BY", orderBy ?? _columnNames.First());
        sqlString.AddParameterWithValue("SORT_DIRECTION", sortDirection);


        sqlString.Append(" OFFSET @offset ROWS FETCH NEXT @page_size ROWS ONLY");
        sqlString.AddParameterWithValue("page_size", length > 0 ? length : MaxLength);
        sqlString.AddParameterWithValue("offset", start);


        return new BugQueryResult
        {
            CountUnfiltered = Convert.ToInt32(DbUtil.execute_scalar(countUnfilteredSqlString)),
            CountFiltered = Convert.ToInt32(DbUtil.execute_scalar(countSqlString)),
            Data = DbUtil.get_dataset(sqlString).Tables[0]
        };

    }

    private string BuildDynamicOrderByClause()
    {
        return string.Join(", ",
            _columnNames.Select(column => string.Format(
                @" CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'DESC' THEN [{0}] END DESC,
CASE WHEN @ORDER_BY = '{0}' AND @SORT_DIRECTION = 'ASC' THEN [{0}] END ASC", column)).ToArray());
    }

    private void ApplyWhereClause(SQLString sqlString, BugQueryFilter[] filters)
    {
        sqlString.Append(" WHERE ");
        List<string> conditions = new List<string>();
        foreach (var filter in filters)
        {
            if (!_columnNames.Contains(filter.Column))
            {
                throw new ArgumentException("Invalid filter column: {0}", filter.Column);
            }
            string parameterName = filter.Column;
            conditions.Add(string.Format("[{0}] = @{1}", filter.Column, parameterName));
            sqlString.AddParameterWithValue(parameterName, filter.Value);
        }
        sqlString.Append(string.Join(" AND ", conditions));
    }

    private string GetInnerSql(IIdentity identity)
    {
        SQLString innerSql = new SQLString(_query.SQL);
        return Util.alter_sql_per_project_permissions(innerSql, identity).ToString();
    }

}
```

In addition to filtering, sorting and paging, this class also applies the user specific project permissions.

[View the commit - Created Bug Query Executor](https://github.com/dpaquette/BugTracker.NET/commit/5274b512db3d5223e41b423b615649723954712b)

Some of the existing queries needed to change slightly to support this. We have added an update script to update the known queries. Any other custom queries will need to be updated by the system administrator.

[View the commit - Updated known queries](https://github.com/dpaquette/BugTracker.NET/commit/2f1ce8e0246c0e4074c99aa5ae24d04005169017)

##Adding Web API
Now that we have added improved bug querying support, we need to expose a way for the application to call and execute the queries without requiring a full page refresh. Since we are planning to use jQuery DataTables, the easiest option will be to expose an HTPP endpoint that returns the data in JSON format. This type of scenario is exactly what Web API was designed for. Let's go ahead and add Web API to the  is the perfect opportunity for us to introduce Web API.

First, create a folder named Controllers. By convention, this is where we add Web API controllers. In Web API, controllers are simple classes that handle HTTP requests. Right click on the new Controllers folder and select Add Controller. Select Web API 2 Controller - Empty from the list of options and name the new controller BugQueryController. Visual studio will add the references to Web API and provide instructions on how to configure it. Follow those instructions by adding `GlobalConfiguration.Configure(WebApiConfig.Register);` to the Application Start method of global.asax.cs.

That's it! We can use Web API in our project.

[View the commit - Added Web API](https://github.com/dpaquette/BugTracker.NET/commit/5c182f13bc1ade3de5037a8e528df7075a2a7f6a)

From the new BugController now, we can add a method that calls the new BugQueryExecutor and returns the results in a format that will be convenient for jQuery DataTables. We also add the Authorize attribute to the BugQueryController. This ensures that only logged in users can access the data from this HTTP endpoint.

```
[Authorize]
public class BugQueryController : ApiController
{
  public IHttpActionResult Get(int queryId, string sortBy, string sortOrder, int start, int length, [FromUri] BugQueryFilter[] filters)
  {
      Query query;
      using (Context context = new Context())
      {
          query = context.Queries.Find(queryId);
      }
      if (query != null)
      {
          BugQueryExecutor queryExecutor = new BugQueryExecutor(query);
          var result = queryExecutor.ExecuteQuery(User.Identity, start, length, sortBy, sortOrder, filters);
          return Ok(new
              {
                  recordsTotal = result.CountUnfiltered,
                  recordsFiltered = result.CountFiltered,
                  data = result.Data
              });
      }
      return NotFound();
  }
}
```
The parameters on the get method correspond to the hidden fields that were used in the original bugs.aspx form.

By default, Web API is configured to use JSON.NET to serialize the result as JSON. Magically, JSON.NET knows how to serialize a `DataTable` so we didn't need to do any extra work here.

##Replacing the Bugs Table
Now that we have all the backend pieces we needed, we can finally replace the bugs table on bugs.aspx with a new jQuery DataTables.

Luckily, jQuery DataTables is extremely flexible, making it relatively easy to configure it to call our Web API endpoint to get data.

To keep things separate, I added a new user control called BugList.ascx. By extracting the bug control to a new page, it gives us the option to potentially re-use it else where in the application.

The markup for this user control simply generates the shell for the table based on the columns that for the selected query. This includes a header with 2 rows: One for the column name and a second row for any filter dropdowns.
```
<table id="bug-table" class="table table-striped table-bordered">
    <thead>
        <!--Main header-->
        <tr>
            <%foreach (string columnName in GetVisibleColumns())
              {%>
            <th><%= GetColumnDisplayName(columnName) %></th>

            <%}%>
        </tr>
        <!--Filter row-->
        <tr class="filter-row">
            <% foreach (string columnName in GetVisibleColumns())
               {%>
            <th><% if (IsFilterableColumn(columnName))
                   {%>
                <select class="table-filter" data-column-name="<%=columnName %>">
                    <% foreach (var option in GetFilterValues(columnName))
                       {%>
                    <option value="<%=option.Value %>"><%=option.Text %></option>
                    <%}%>
                </select>

                <%} %></th>

            <%}%>
        </tr>
    </thead>
</table>
```

Based on the existing logic, only certain columns are visible and a subset of those columns are filterable. I extracted this logic from the old static utility methods and placed them in the code behind for the user control.

Finally, I added the following JavaScript to the BugList control to actually render the grid:

```
$(function() {
    ///Special column rendering functions not shown for brevity.

    var getCurrentFilters = function() {
      var filters = [];
      $("select.table-filter").each(function() {
          var currentFilter = $(this);
          var selectedValue = currentFilter.val();
          if (selectedValue) {
              var selectedColumnName = currentFilter.attr('data-column-name');
              filters.push({Column: selectedColumnName, Value: selectedValue});
          }
      });
      return filters;
    };

    var bugsTable = $("#bug-table").dataTable({
        serverSide: true,
        processing: true,
        paging: true,
        orderCellsTop : true,
        orderMulti: false,
        searching: false,
        ajax: function(data, callback) {
            var sortColumnName = data.columns[data.order[0].column].data;
            var urlParameters = {
                queryId : queryId,
                sortBy : sortColumnName,
                sortOrder : data.order[0].dir,
                start: data.start,
                length: data.length,
                idOnly: false,
                filters: getCurrentFilters()
            }
            BugList.setQueryParams(urlParameters);
            var queryUrl = "api/BugQuery?" + $.param(urlParameters);

            $.get(queryUrl).done(function(d) {
                callback(d);
            }).fail(function() {
                callback();
            });
        }
    }).api();

    //Force a refresh after any of the filters are changed
    $("select.table-filter").on("change", function() {
        bugsTable.draw();
    });
});
```
By specifying an `ajax` function for the dataTable configuration, we are able to tell the application exactly how to call our new Web API method. There is also some custom column rendering logic that was migrated over from the old static utility methods. These are not shown above for brevity. While some of this JavaScript might look a little daunting, it did allow us to delete a lot of complex code from the bugs.aspx page. It will also eventually allow us to delete a lot of very hard to understand code from bug_list.js and bug_list.cs.

![New Bugs Grid](images/BugsGridNew.png)

The columns are a little narrow to fit them all on the page, but the overall look and feel is improved. The biggest improvement for the user is that any sort, filter or paging action no longer triggers a full page refresh. The page generally *feels* snappier.

[View the commit - Updated Bugs page to use jQuery DataTables](https://github.com/dpaquette/BugTracker.NET/commit/c026cb54f69e00ec1860574f20270e3e4a85d7d8)

##Grid Related Functionality
If that was all we needed to do, this change would have been fairly straight forward. Unfortunately, there was are a handful of features that relied on the old data grid implementation and are now broken with the new approach.

Talk about how all these features relied on session state

###Printing
People still print things?

###Export to Excel
Always....there always has to be an export to excel...

###Navigating through individual results
This is actually a useful feature. Too bad it's so hard to implement
