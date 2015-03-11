#Updating Grid Components


##The Current Approach
Let's start with a simple example from the Admin section. The Admin -> Projects page displays all of the projects defined in BugTracker in a simple, sortable grid.

![The original Projects grid](Images/ProjectsGridOriginal.png)

From a user interface perspective, this grid is functional but a little dated. I would like to give it an update to make it look more modern.

More importantly, there are a number of things I would like to improve regarding the implementation of this grid. Rendering this grid involves passing the Response object and a DataSet to a static function called `sortable_html_table.create_from_dataset`. This function contains a lot of code along the lines of:

```
r.Write("<tr>\n");

int db_column_count = 0;

foreach (DataColumn dc in ds.Tables[0].Columns)
{

    if ((edit_url != "" || delete_url != "")
    && db_column_count == (ds.Tables[0].Columns.Count - 1))
    {
        if (edit_url != "")
        {
            r.Write("<td class=datah valign=bottom>edit</td>");
        }
        if (delete_url != "")
        {
            r.Write("<td class=datah valign=bottom>delete</td>");
        }

    }
    else
    {

        // determine data type
        string datatype = "";
        if (Util.is_numeric_datatype(dc.DataType))
        {
            datatype = "num";
        }
        else if (dc.DataType == typeof(System.DateTime))
        {
            datatype = "date";
        }
        else
        {
            datatype = "str";
        }

        r.Write("<td class=datah valign=bottom>\n");

        if (dc.ColumnName.StartsWith("$no_sort_"))
        {
            r.Write(dc.ColumnName.Replace("$no_sort_", ""));
        }
        else
        {
            if (write_column_headings_as_links)
            {
                string sortlink = "<a href='javascript: sort_by_col($col, \"$type\")'>";
                sortlink = sortlink.Replace("$col", Convert.ToString(db_column_count));
                sortlink = sortlink.Replace("$type", datatype);
                r.Write(sortlink);
            }
            r.Write(dc.ColumnName);
            if (write_column_headings_as_links)
            {
                r.Write("</a>");
            }
        }

        //r.Write ("<br>"); // for debugging
        //r.Write (dc.DataType);

        r.Write("</td>\n");

    }

    db_column_count++;

}
r.Write("</tr>\n");
```
There are some big assumptions made in this code. If we want to disable sorting for a column, we need the column name to start with `$no_sort_`. That's only the portion of the code that deals with rendering the header.  The code for rendering the rows of data is equally complex and also riddled with assumptions about the contents of the dataset.

In order of this mechanism of rendering a grid to work, the projects.aspx page executes some strange looking SQL:

```
ds = btnet.DbUtil.get_dataset(new SQLString(@"select
		pj_id [id],
		'<a href=edit_project.aspx?&id=' + convert(varchar,pj_id) + '>edit</a>' [$no_sort_edit],
		'<a href=edit_user_permissions2.aspx?projects=y&id=' + convert(varchar,pj_id) + '>permissions</a>' [$no_sort_per user<br>permissions],
		'<a href=delete_project.aspx?id=' + convert(varchar,pj_id) + '>delete</a>' [$no_sort_delete],
		pj_name [project],
		case when pj_active = 1 then 'Y' else 'N' end [active],
		us_username [default user],
		case when isnull(pj_auto_assign_default_user,0) = 1 then 'Y' else 'N' end [auto assign<br>default user],
		case when isnull(pj_auto_subscribe_default_user,0) = 1 then 'Y' else 'N' end [auto subscribe<br>default user],
		case when isnull(pj_enable_pop3,0) = 1 then 'Y' else 'N' end [receive items<br>via pop3],
		pj_pop3_username [pop3 username],
		pj_pop3_email_from [from email address],
		case when pj_default = 1 then 'Y' else 'N' end [default]
		from projects
		left outer join users on us_id = pj_default_user
		order by pj_name"));
```
Notice how there is HTML embedded inside this query. In order to have display edit/delete links, we need the cells in the data set to contain the actual HTML that will be rendered on the client.
This approach is difficult to maintain and the intention is not clear. User Interface concerns such as displaying links to other pages would be best managed by the aspx page.


##Selecting a Grid Control
There are countless ASP.NET grid controls of various price and quality available. Since we are working on an open source project here, our budget of $0 will rule out any of the expensive options.

I have had very good luck with [DataTables](http://datatables.net/), a table plug-in for jQuery. Note that this is not strictly an ASP.NET control. It is a client side jQuery based control that can be used with any server side web framework. It is a very capable, flexible, fast and extremely well document option for adding rich data grid functionality to any web application.

Let's start by adding DataTables to our project.

`Install-Package datatables.net`

Our intention is to use this package on a large number of the existing BugTracker pages. It will be easiest if we reference the JavaScript and CSS files from our master page:

```
<link href="Content/DataTables-1.10.5/media/css/jquery.dataTables.min.css" rel="stylesheet" />
src="Scripts/DataTables-1.10.5/media/js/jquery.dataTables.min.js"></script>
```
[View the commit](https://github.com/dpaquette/BugTracker.NET/commit/769a4d286b90e927ffb67e811f35f3fb073d1826)

DataTables has a number of options of how to initialize the grid. For example, we can retrieve the data via Ajax or we can have the server render a simple HTML table then tell DataTables to convert that HTML table element into a fully featured data grid.

In BugTracker, we will opt for rendering the HTML table on the server. This option will fit nicely with the Layered approach that we discussed in the Styles of Web Forms section.

##Updating the Projects grid
Let's start by rendering the HTML table element in projects.aspx.

We will replace
```
<%
    if (ds.Tables[0].Rows.Count > 0)
    {
        SortableHtmlTable.create_from_dataset(
            Response, ds, "", "", false);
    }
    else
    {
        Response.Write("No projects in the database.");
    }
%>
```
with the following code that is a little more verbose but conveys the intention much more clearly:
```
  <table id="projects-table" class="table table-striped table-bordered" cellspacing="0" width="100%">
      <thead>
          <tr>
              <th>id</th>
              <th>project</th>
              <th>active</th>
              <th>default user</th>
              <th>auto assign<br/> default user</th>
              <th>auto subscribe<br/> default user</th>
              <th>receive items<br/> via pop3</th>
              <th>pop3 username</th>
              <th>from email<br/> address</th>
              <th>default</th>
              <th></th>
          </tr>
      </thead>
      <tbody>
          <% foreach (DataRow dataRow in ds.Tables[0].Rows)
             {
                 string projectId = Convert.ToString(dataRow["id"]);
                 %>
          <tr>
              <td><%=projectId %></td>
              <td><%=dataRow["project"] %></td>
              <td><%=dataRow["active"] %></td>
              <td><%=dataRow["default user"] %></td>
              <td><%=dataRow["auto assign default user"] %></td>
              <td><%=dataRow["auto subscribe default user"] %></td>
              <td><%=dataRow["receive items via pop3"] %></td>
              <td><%=dataRow["pop3 username"] %></td>
              <td><%=dataRow["from email address"] %></td>
              <td><%=dataRow["default"] %></td>
              <td>
                  <a href="edit_project.aspx?id=<%=projectId %>"><i class="glyphicon glyphicon-edit" title="Edit Project"></i></a>
                  <a href="edit_user_permissions2.aspx?projects=y&id=<%=projectId %>"><i class="glyphicon glyphicon-user" title="Edit User Permissions"></i></a>
                  <a href="delete_project.aspx?id=<%=projectId %>"><i class="glyphicon glyphicon-trash" title="Delete Project"></i></a>
              </td>

          </tr>
              <%} %>
      </tbody>
  </table>

```
Now that the Edit / Delete / User Permissions links are rendered clearly in the aspx page, we can simplify our SQL in the code behind.
```
ds = btnet.DbUtil.get_dataset(new SQLString(
                @"select
		pj_id [id],
		pj_name [project],
		case when pj_active = 1 then 'Y' else 'N' end [active],
		us_username [default user],
		case when isnull(pj_auto_assign_default_user,0) = 1 then 'Y' else 'N' end [auto assign default user],
		case when isnull(pj_auto_subscribe_default_user,0) = 1 then 'Y' else 'N' end [auto subscribe default user],
		case when isnull(pj_enable_pop3,0) = 1 then 'Y' else 'N' end [receive items via pop3],
		pj_pop3_username [pop3 username],
		pj_pop3_email_from [from email address],
		case when pj_default = 1 then 'Y' else 'N' end [default]
		from projects
		left outer join users on us_id = pj_default_user
		order by pj_name"));
```

In `projects.aspx` we can remove the reference to sortable.js, some code that was handling the sorting of the grid rendered by `sortable_html_table.create_from_dataset`. Finally, we need to add some JavaScript to initialize the data grid with the default options:
```
<script type="text/javascript">
    $(function() {
        $("#projects-table").dataTable();
    });
</script>
```
After applying some Bootstrap styling and layout, our new Projects grid looks like this:

![Projects grid using DataTables.NET](Images/ProjectsGridNew.png)

We have moved the row actions for edit, user permissions and delete to the last column and changed them to more modern looking icons. By default, the DataTables component gives us column sorting with nice sort indicator icons. We also get Paging and Searching for free. While these feature might seem like overkill when the grid only has 2 rows, it will be nice to get these features across the board.

We now have a modern grid component that provides a better UI and we have greatly simplified the code related to displaying the grid.

[View the commit - Updated Projects grid](https://github.com/dpaquette/BugTracker.NET/commit/e73686ae788fef1fff08df4d824478fea5ddc4f0)

We can simplify this code even further by using the new Entity Framework model that we recently introduced. This eliminates the SQL in the code behind:

```
using (Context context = new Context())
{
    Projects = context.Projects.OrderBy(p => p.Name).ToArray();
}  
```
On the aspx page, we can now loop over the strongly typed array of Projects which further simplifies the code:
```
 <% foreach (Project project in Projects)
     {
  %>
  <tr>
      <td><%=project.Id %></td>
      <td><%=project.Name%></td>
      <td><%=project.Active == 1 ? "Y" : "N" %></td>
      <!-- etc. -->
  </tr>
  <% } %>
```

[View the commit - Using Entity Framework in Projects page]()

#Performance
You might be wondering about the performance of this approach and how well it will scale when dealing with large amounts of data. These are valid concerns as this approach as the potential to be very inefficient if we are trying to display a large number of rows.

For the example above, performance should not be a problem. I can't imagine a situation where we would have more than a handful or projects in the system and I know that this approach will scale well to at least 100 rows. Just to test out the theory, let's test a scenario where we have 1000 projects.

From a server performance, this seems to perform within acceptable parameters. The HTML that is generated comes in at just over 1MB. This is a little on the large side for sure, but by default IIS is actually compressing that before sending it to the client. The actual size of the package delivered to the client is only 73.8KB. I can live with these parameters as a worst case scenario.

![Response size with 1000 projects](Images/1000ProjectsPageSize.png)

One thing that would be annoying from a user's perspective is that the grid flashes momentarily before the DataTables component is initialized. An easy solution to this is to hide the table initially, then show the table after DataTables is done initializing.  While the table is initializing, we will show some *loading* text so the screen is not blank for the user.

```
...
  <div id="table-loading-indicator">Loading...</div>
  <table id="projects-table" class="table table-striped table-bordered" style="display: none">
...
<script type="text/javascript">
    $(function () {
        $("#projects-table").dataTable();
        $("#projects-table").show();
        $("#table-loading-indicator").hide();
    });
</script>
```

This small change will ensure a good user experience for a fairly large number of rows. We can be confident at least that all the admin grids will work using this approach.

[View the commit - Improved initialization of Projects grid](https://github.com/dpaquette/BugTracker.NET/commit/9d6287fe16fd73a32aff7a5abb96c2f3196c476c)

According to guidance provided on the [DataTables.net FAQ](http://datatables.net/faqs/index), the DOM sourced data approach should scale to ~5,000 rows. That seems a little high to me but again gives me confidence that we will be okay for the admin grid. We will get back to the remaining admin grids later. For now, let's turn our attention to the Bugs grid.

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

As you can see, rendering the last page in the table involves iterating through every single row in the table. This is not a responsible use of the web server's CPU.

The approach used to render this grid might result in acceptable performance for a small number of total bugs and a small number of users, it just won't handle load well. Unlike the admin pages, the bugs page needs to be as efficient as possible. It is the main page of the appliction and we should expect that every single user of BugTracker might be using it at the same time.

Let's see what we can do it improve this page.

One of the biggest challenges here is the fact that this page can execute  arbitrary queries that return any number of columns. This seems to be an important extensibility point in BugTracker so I would like to keep this feature.
