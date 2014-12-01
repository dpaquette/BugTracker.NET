<%@ Page language="C#" CodeBehind="priorities.aspx.cs" Inherits="btnet.priorities" AutoEventWireup="True" %>
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
		+ "priorities";

	ds = btnet.DbUtil.get_dataset (new SQLString(
		@"select pr_id [id],
		pr_name [description],
		pr_sort_seq [sort seq],
		'<div style=''background:' + pr_background_color + ';''>' + pr_background_color + '</div>' [background<br>color],
		pr_style [css<br>class],
		case when pr_default = 1 then 'Y' else 'N' end [default],
		pr_id [hidden] from priorities"));

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet priorities</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="admin"/>

<div class=align>
<a href=edit_priority.aspx>add new priority</a>
</p>
<%


if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_priority.aspx?id=", "delete_priority.aspx?id=", false);

}
else
{
	Response.Write ("No priorities in the database.");
}
%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
