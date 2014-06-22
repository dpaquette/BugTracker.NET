<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


DataSet ds;

Security security;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "custom fields";

	ds = Util.get_custom_columns();

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet custom fields</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>


<div class=align>
<a href=add_customfield.aspx>add new custom field</a>
</p>
<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_customfield.aspx?id=", "delete_customfield.aspx?id=");

}
else
{
	Response.Write ("No custom fields.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>