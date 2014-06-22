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
		+ "user defined attribute values";

	ds = btnet.DbUtil.get_dataset(
		@"select udf_id [id],
		udf_name [user defined attribute value],
		udf_sort_seq [sort seq],
		case when udf_default = 1 then 'Y' else 'N' end [default],
		udf_id [hidden]
		from user_defined_attribute order by udf_name");

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet user defined attributes</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>


<div class=align>
<a href=edit_udf.aspx>add new user defined attribute value</a>
</p>
<%


if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "edit_udf.aspx?id=", "delete_udf.aspx?id=");

}
else
{
	Response.Write ("No user defined attributes in the database.");
}
%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>