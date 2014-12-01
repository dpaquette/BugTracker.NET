<%@ Page language="C#" CodeBehind="categories.aspx.cs" Inherits="btnet.categories" AutoEventWireup="True" %>
<%@ Register Src="~/Controls/MainMenu.ascx" TagPrefix="uc1" TagName="MainMenu" %>

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
		+ "categories";
		
	ds = btnet.DbUtil.get_dataset(new SQLString(
		@"select
		ct_id [id],
		ct_name [category],
		ct_sort_seq [sort seq],
		case when ct_default = 1 then 'Y' else 'N' end [default],
		ct_id [hidden]
		from categories order by ct_name"));

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet categories</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="admin"/>


<div class=align>
<a href=edit_category.aspx>add new category</a>
</p>
<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_category.aspx?id=", "delete_category.aspx?id=");

}
else
{
	Response.Write ("No categories in the database.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
