<%@ Page language="C#" CodeBehind="customfields.aspx.cs" Inherits="btnet.customfields" AutoEventWireup="True" %>
<%@ Register TagPrefix="uc1" Namespace="btnet.Controls" Assembly="BugTracker.Web" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


DataSet ds;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	

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
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="admin"/>


<div class=align>
<a href=add_customfield.aspx>add new custom field</a>

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
