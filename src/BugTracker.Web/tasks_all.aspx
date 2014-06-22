<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

DataSet ds_tasks;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);
	
	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "all tasks";
	
	if (security.user.is_admin || security.user.can_view_tasks)
	{
		// allowed
	}
	else
	{
		Response.Write("You are not allowed to view tasks");
		Response.End();
	}
	
	ds_tasks = btnet.Util.get_all_tasks(security,0);
}

</script>

<html>
<head>
<title id="titl" runat="server">btnet all tasks</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>
<body>
<div class=align>

All Tasks

<p>


<%
if (ds_tasks.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds_tasks, "", "", false); 
}
else
{
	Response.Write ("No tasks.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


