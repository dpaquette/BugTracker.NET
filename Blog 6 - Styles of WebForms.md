#Styles of WebForms

I seen and written a lot of web forms code over the years. It seems to me that there are basically four different forms that the code can take:

- Classic ASP
- WebForms
- Model View Presenter
- Layered

In this chapter we'll look at how you can recognize each one and discuss the advantages and disadvantages of each style.

##Classic ASP

Before ASP.net there was classic ASP. At that time technologies like server side includes and cgi scripts were all the rage. The content of the web page is written  with minimal help of any sort of domain specific language.

If you encounter an ASP.net application written in this style you'll see a lot of calls to Response.Write. This call writes directly to the output stream that is being sent to the browser.

For example in BugTracker.net there are some code blocks that look like

```
<p>Server Info:
<%
Response.Write("<br>Path=");
Response.Write(HttpContext.Current.Server.MapPath(null));
Response.Write("<br>MachineName=");
Response.Write(HttpContext.Current.Server.MachineName);
Response.Write("<br>ScriptTimeout=");
Response.Write(HttpContext.Current.Server.ScriptTimeout);
Response.Write("<br>.NET Version=");
Response.Write(Environment.Version.ToString());
Response.Write("<br>CurrentCulture=");
Response.Write(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
%>
```
Here a number of variables are written out directly to the output stream. In this case the writing out is embedded in some HTML markup.

This style can be quite performant as there is very little overhead to writing to the response stream. It is also quite easy for maintenance developers to understand what is going on.

It is, however, difficult to provide any sort of structure to applications written in this style. Should a component in the middle of the page need to change some earlier part of the page then it is almost impossible. You have to write to some buffer and then rewrite the existing content. The code quickly turns into a spagetti mess.  You also end up writing out complex escape sequences just to get the actual markdown you want. For instance in this example code also taken from BugTracker.NET:

```
Response.Write("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td align=left valign=top>");
Response.Write(paging_string);
Response.Write("<td align=right valign=top>");
Response.Write("<span class=smallnote>clicking while holding Ctrl key ");
Response.Write("toggles \"NOT\" in a filter: \"NOT project 1\"</span></table>");
Response.Write("\n<table class=bugt border=1 ><tr>\n");

```

There is need to escape the quotation marks and also to escape new line characters - something I don't really understand.

While performant the complexity of building an intricate application using this style requires a great deal of care. I would recommend against using this style due to the potential to create a terrible mess.

If you happen to have an application that uses this style then a good trick to migrate away from it is to simply start by replacing the Response.Write with the language provided for you: the aspx domain specific language.

In the case of the above code we could quite easily replace it with inline code:

```
<table border="0" cellpadding="0" cellspacing="0" width="100%">
<tr>
<td align="left" valign="top">
<%: paging_string %>
</td>
<td align="left" valign="top">
<span class=smallnote>clicking while holding Ctrl key toggles "NOT" in a filter: "NOT project 1"</span>
</td>
</tr>
</table>
<table class="bugt" border="1">
<tr>
```

While writing this version of the code I came across a number of bugs and fixed them. Most of the issues were related to incorrect tag termination. The domain specific language that is a mixture of C# and HTML makes issues far more apparent and the code easier to read.

Code of this sort will probably date to the early days of ASP.net. Developers coming over from CGI approaches to web programming or even those comign from structured languages such as C are most likely to use this style.

##WebForms

In the late 90s and early 2000s Microsoft had a problem: their strategy called for moving to web development but very few of their existing developers had any experience doing web development. Most of their developers had experience with Visual Basic 6 which provided a drag and drop method of building applications out of a series of controls. These controls had event listeners attached to them that were where the majority of the programming occurred.

In order to allow developers to use their existing skills Microsoft developed WebForms which brought much of the same programming paradigm to the web. Controls can be paced onto the page and the event handlers execute on the server side. Custom controls can be built to provide reusable functionality beween pages.

The normal method of interacting with a web page were overridden using JavaScript to send post messages back to the server. The code on the server would update the page model and perform a full page refresh, sending the entire page back to the client. A significant amount of binding logic is handled server side as well.

Very little of BugTracker.net is written using this style. One section that uses it is the log management screen, manage_logs.aspx.

```
<asp:DataGrid ID="MyDataGrid"
runat="server"
BorderColor="black"
CssClass="datat"
CellPadding="3"
AutoGenerateColumns="false"
OnItemCommand="my_button_click">
<HeaderStyle CssClass="datah"></HeaderStyle>
<ItemStyle CssClass="datad"></ItemStyle>
<Columns>
<asp:BoundColumn HeaderText="File" DataField="file" />
<asp:HyperLinkColumn HeaderText="Download"
Text="Download"
DataNavigateUrlField="url"
Target="_blank" />
<asp:ButtonColumn HeaderText="Delete"
ButtonType="LinkButton"
Text="Delete"
CommandName="dlt" />
</Columns>
</asp:DataGrid>
```

Here you can see the famous ```runat="server"``` which is what tells ASP.net that this is a control that it should manage.

To this day this is probably the most common method of building web forms application. Huge numbers of vendor controls exist that follow this model. If you need a chart or a tree or a drop down box there are vendors anxious to sell you on that.

I've never been a huge fan of this approach. It is difficult to test and blurs any lines between presentation code and application logic. It is also contrary to how the web is supposed to work: state is stored on the server, the POST verb is used even when GET is more appropriate.

That being said there are tens of millions of very successful web sites that are written using this style. An application written in this style is not, on its own, a reason to replace an application.

##Model View Presenter

Model View Presenter is one of the family of patterns I call MV-Something along with Model View Controller and Model View View-Model. It is a fairly well known pattern in the Microsoft world and is generally used to structure WPF and Silverlight applications. It is pretty rare to see this pattern in the wild with WebForms but if the original developers are from a Silverlight background.

![Model View Presenter](Images/MVP.png)

In effect this style of developing web forms splits out the .aspx pages as views. The presenter is a class that governs the communication between the view and the model. It is what actually contains the application and contains the business logic. Finally the model is a simple plain old C#/VB object that contains no real logic.

I said that is is rare to see this in WebForms but there is a pretty popular project out there called [Web Forms MVP](https://github.com/webformsmvp/webformsmvp) that promotes this model. It is an additive framework (meaning you can adopt it as you go, one page at a time) that helps build testable, maintainable code.

Projects that use this structure properly are likely of higher quality than most Web Forms projects. If you happen to encounter a project that uses this structure it is advisable to continue using it.

##Layered

The final style I've seen is the one that I have adopted when working with WebForms. I attempt to keep all the controller code in the code behind and all the display code in the view. In order to accomplish this I create protected properties on the code behind and use these to communicate with the aspx file.

A great example is creating a data grid. Using the WebForms method above one would create column definitions and then perform a data bind to attach some data source to the grid.

.aspx file

```
<asp:DataGrid ID="usersGrid" runat="server">
<Columns>
<asp:BoundColumn HeaderText="CustomerId" DataField="CustomerId" />
<asp:BoundColumn HeaderText="First Name" DataField="FirstName" />
<asp:BoundColumn HeaderText="Last Name" DataField="LastName" />
<asp:BoundColumn HeaderText="Phone Number" DataField="Phone" />
<asp:BoundColumn HeaderText="Email Address" DataField="Email" />
</Columns>
</asp:DataGrid>
```

Code behind

```
grid.DataSource = GetData();
grid.DataBind();
```

You can see there is a mixture, in the code behind, of controller and view logic. Not only does the code behind know how to get the data but also where to bind it. The latter should be the responsibility of the presentation or view layer.

Using the layered approach one would shift the display logic to the .aspx file.

Code behind

```
protected IEnumerable<User> Users{ get; set; }
protected void Page_Load(object sender, EventArgs e)
{
  Users = GetData();
}

```

.aspx file

```
<table>
<thead>
<tr>
<th>Customer Id</th>
<th>First Name</th>
<th>Last Name</th>
<th>Phone Number</th>
<th>EMail</th>
</tr>
</thead>
<tbody>
<% foreach(var row in Users){ %>
  <tr>
  <td><%: row.CustomerId %></td>
  <td><%: row.FirstName %></td>
  <td><%: row.LastName %></td>
  <td><%: row.PhoneNumber %></td>
  <td><%: row.EMail %></td>
  </tr>
  <%}%>  
  </tbody>
  </table>
  ```

  As you can see this approach is somewhat more verbose than the pure WebForms approach. It also means that you can't take advantage of most the controls you might typically use in a WebFroms application. For an experienced WebForms developer this would almost certainly slow you down. Adding sorting to a GridView is as easy as adding a sortable property to it. For my version you need to build an entire control yourself. There are, however, plenty of client side options that provide functionality similar to what is lost by doing away with WebForms. [Datatables](http://www.datatables.net/) is a jQuery based solution for grids and there are equivalent solutions for all other controls. The controls that are available are frequently lighter weight and more modern than the WebForms version.

  We also totally abandon the event model in WebForms. This means that we don't have to deal with post backs and view state. It does, however, put more logic in the Page_Load. In my mind this encourages smaller, simpler pages, which is A-Okay.

  The advantages, I believe, outweigh the drawbacks. First is that this style gets us far closer to using pure HTML. One of my major frustrations with WebForms is that it is a leaky abstraction over top of HTML. This means that many of the things that are easy with regular HTML become very difficult with WebForms. Up until a recent release of WebForms is was non-trivial to predict the client side Ids for elements on a page. This made programming against it using JavaScript tricky. This has since been ameliorated by allowing developers to set the client Id generation strategy. However there are numerous other situations in which WebForms seems to act as a huge barrier to adopting good (or at least modern) HTML practices.

  The second advantage I see is around readability. To me it is much easier to understand what is happing in the layered approach. There is less magic going on in the background much of which is unnecessary magic. Developers coming from other platforms such as PHP or even Rails will have an easy time understanding the code.

  Finally keeping view logic away from the controller logic makes unit testing far easier. It is, of course, still difficult to test the monolithic HttpRequest and HttpResponse objects. Getting around that requires a much more dramatic change such as adopting OWIN. Because the view logic in the layered approach is split from the back end logic it is actually quite easy to move the code to ASP.net MVC. The example above might be changed by simply doing

  ```
  <table>
  <thead>
  <tr>
  <th>Customer Id</th>
  <th>First Name</th>
  <th>Last Name</th>
  <th>Phone Number</th>
  <th>EMail</th>
  </tr>
  </thead>
  <tbody>
  @foreach(var row in Model.Users){
    <tr>
    <td>@row.CustomerId</td>
    <td>@row.FirstName</td>
    <td>@row.LastName</td>
    <td>@row.PhoneNumber</td>
    <td>@row.EMail</td>
    </tr>
  }
  </tbody>
  </table>
  ```

  Not only did I change this to MVC I also altered the view engine to Razor. We could drop the view into a Rails application with almost no changes as WebForms has an almost identical syntax to ERB/erubis.

  ##Conclusion

  Each of the styles has its advantages and disadvantages. Choosing the right solution is a matter of examining the tradeoffs within the context of your application and your team.
