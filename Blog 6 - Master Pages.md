#Master Pages

Years ago I read the book The Pragmatic Programmer written by Andrew Hunt and David Thomas. It is one of the best books about development I've ever read. Amongst a number of other great gems is the rule Don't Repeat Yourself. While not the best rule for dealing with 2 year olds it is an excellent rule when developing software. When you have the same piece of code in more than one place and a change needs to be made the change needs to be made in all the places. More often than not one of the places that needs updating will be forgotten. Now you've got a real mess on your hands.

In the BugTracker.NET code base one of the biggest sources of code duplication is found in the style and structure of the pages. For the most part the styles used on the BugTracker.NET are minimal but there is still a menu and some common code. On our list of tasks is to improve the UI of BugTracker.NET so we're going to need to make changes to the styles. So let's begin our adventure into adding master pages to the application.

A master page is like a base class for a web page. In it we can define specific sections that can be filled in by the individual pages on the site. This means that all the common code can be placed into the master page and we can eliminate the duplication.

There are quite a few pages so we had better roll up our sleeves and get dug in. Let's start with a pretty simple page and try to figure out a routine we can follow when we hit more complex pages.

delete_bug.aspx seems like a good choice. It has the typical mixture of C# and HTML that we see with this project.

The first step is to move the C# code to the code behind.

```
using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
  public partial class delete_bug : BasePage
  {

    SQLString sql;

    protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

    ///////////////////////////////////////////////////////////////////////
    protected void Page_Load(Object sender, EventArgs e)
    {

      Util.do_not_cache(Response);

      if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanDeleteBugs())
      {
        //
      }
      else
      {
        Response.Write("You are not allowed to use this page.");
        Response.End();
      }

      string id = Util.sanitize_integer(Request["id"]);

      int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(id), User.Identity);
      if (permission_level != PermissionLevel.All)
      {
        Response.Write("You are not allowed to edit this item");
        Response.End();
      }

      if (IsPostBack)
      {

        Bug.delete_bug(Convert.ToInt32(row_id.Value));
        Server.Transfer("bugs.aspx");

      }
      else
      {

        titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
        + "delete " + Util.get_setting("SingularBugLabel", "bug");

        back_href.HRef = "edit_bug.aspx?id=" + id;

        sql = new SQLString(@"select bg_short_desc from bugs where bg_id = @bugId");
        sql = sql.AddParameterWithValue("bugId", id);

        DataRow dr = DbUtil.get_datarow(sql);

        confirm_href.InnerText = "confirm delete of "
        + Util.get_setting("SingularBugLabel", "bug")
        + ": "
        + Convert.ToString(dr["bg_short_desc"]);

        row_id.Value = id;
      }

    }

  }
}

```

In the process we set the visibility on the Page_Load and Page_Init to be protected instead of private. The methods weren't formally in a class before so there was no need to set the visibility. We also need to pull in some additional namespaces such as ```using btnet.Security;```.

At this point the functionality of the page remains unchanged. We can manually test that and it is probably a good idea to do so. Testing as we go through this process will let you know right away if something you just did is wrong instead of waiting until the end. The .aspx file now looks like

```
<%@ Page language="C#" CodeBehind="delete_bug.aspx.cs" Inherits="btnet.delete_bug" AutoEventWireup="True" %>
<%@ Register Src="~/Controls/MainMenu.ascx" TagPrefix="uc1" TagName="MainMenu" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<html>
<head>
<title id="titl" runat="server">btnet delete bug</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="admin"/>
<p>
<div class=align>
<p>&nbsp</p>
<a id="back_href" runat="server" href="">back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>

<p>or<p>

<script>
function submit_form()
{
  var frm = document.getElementById("frm");
  frm.submit();
  return true;
}

</script>
<form runat="server" id="frm">
<a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
<input type="hidden" id="row_id" runat="server">
</form>


</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
```
Still quite a bit of common code in there, let's extract some of it to a master file. The main menu is typically seen everywhere we are logged in. This page is no exception

![Delete Bug](Images/delete_bug.png)

So let's put the main menu in the master file. Every page is also going to contain the HTML declaration. Each page also contains the footer. The master page we come up with looks like:

```
<%@ Master Language="C#" AutoEventWireup="true" CodeBehind="LoggedIn.master.cs" Inherits="btnet.LoggedIn" %>

<%@ Register Src="~/Controls/MainMenu.ascx" TagPrefix="uc1" TagName="MainMenu" %>

<!DOCTYPE html>

<html>
<head runat="server">
<link rel="shortcut icon" href="favicon.ico">
<title>btnet bugs</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" src="scripts/jquery-1.11.1.min.js"></script>
<asp:ContentPlaceHolder runat="server" ID="headerScripts"></asp:ContentPlaceHolder>

</head>
<body>
<uc1:MainMenu runat="server" ID="MainMenu" />
<asp:ContentPlaceHolder runat="server" ID="body"></asp:ContentPlaceHolder>

</body>

<asp:contentplaceholder runat="server" id="footerScripts"></asp:contentplaceholder>

<% Response.Write(Application["custom_footer"]); %>
</html>
```

You'll note that we added 3 ContentPlaceHolders as well. These are the places where child pages can plug in their content. We have one in the header for injecting any custom style information. One in the middle contains the bulk of the page. This section will take any actual page content. Finally we have ```footerScripts``` where any page specific JavaScript can be added.

In delete_bug.aspx we will add a MasterPageFile declaration

```
<%@ Page language="C#"
        CodeBehind="delete_bug.aspx.cs"
        Inherits="btnet.delete_bug"
        AutoEventWireup="True"
        MasterPageFile="~/LoggedIn.Master"  %>
```

You might note here that the master page is called ```LoggedIn.Master```. It is perfectly legal to have multiple MasterPage files in the application. You can even [nest master pages](http://msdn.microsoft.com/en-us/library/vstudio/bb547109%28v=vs.100%29.aspx) so that the content on a page is filled by another master page whose content was filled by a child. A word of warning, however, too many master pages tend to result in duplicate code between the master pages. It isn't long before you're back in the same duplicate code boat master pages were meant to escape.

We can also set the MasterType which allows child pages to refer to controls on the master page through the Master property. This goes right after the page declaration.

```
<%@ MasterType TypeName="btnet.LoggedIn" %>
```

We can now delete the common code that we previously moved to the master page. This leaves us with just the content that we can put into sections.

```
<p>
<div class=align>
<p>&nbsp</p>
<a id="back_href" runat="server" href="">back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>

<p>or<p>

<script>
function submit_form()
{
  var frm = document.getElementById("frm");
  frm.submit();
  return true;
}

</script>
<form runat="server" id="frm">
<a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
<input type="hidden" id="row_id" runat="server">
</form>


</div>
```

That script section can be pulled out into our new script section in the footer. The rest of the content can go into the body.

```
<%@ Page Language="C#" CodeBehind="delete_bug.aspx.cs" Inherits="btnet.delete_bug" AutoEventWireup="True" MasterPageFile="~/LoggedIn.Master" %>

<%@ MasterType TypeName="btnet.LoggedIn" %>

<asp:Content ContentPlaceHolderID="body" runat="server">
        <p />
        <div class="align">
        <p>&nbsp</p>
        <a id="back_href" runat="server" href="">back to
         <% Response.Write(btnet.Util.get_setting("SingularBugLabel", "bug")); %>
        </a>

        <p>or</p>

        <form runat="server" id="frm">
             <a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
             <input type="hidden" id="row_id" runat="server">
        </form>
     </div>

</asp:Content>

<asp:Content ContentPlaceHolderID="footerScripts" runat="server">
    <script>
      function submit_form() {
         var frm = document.getElementById("frm");
         frm.submit();
         return true;
      }
    </script>
</asp:Content>

```

I was confident this would work but when I compiled it I found an error. The title element is no longer run at the server so it blows up.

```
titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
+ "delete " + Util.get_setting("SingularBugLabel", "bug");
```
It is an easy fix to change the code to read

```
Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
+ "delete " + Util.get_setting("SingularBugLabel", "bug");
```

Everything now compiles but the page doesn't seem to work. Clicking on the delete confirmation button does nothing. The issue is that this button is hooked up to JavaScript and the JavaScript makes a reference to an element called ```frm```.

```
var frm = document.getElementById("frm");
```

The element's name has changed and is now called aspnetForm. We could update this manually but the recommended way is to use the ClientID.

```
var frm = document.getElementById("<%=frm.ClientID%>");
```

Now we just need to repeat this for every. single. page.
