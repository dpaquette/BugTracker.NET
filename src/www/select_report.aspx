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
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	if (security.user.is_admin || security.user.can_use_reports)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "reports";

	string sql = @"
select
rp_desc [report],
case
	when rp_chart_type = 'pie' then
		'<a href=''javascript:select_report(""pie"",' + convert(varchar, rp_id) + ')''>select pie</a>'
	when rp_chart_type = 'line' then
		'<a href=''javascript:select_report(""line"",' + convert(varchar, rp_id) + ')''>select line</a>'
	when rp_chart_type = 'bar' then
		'<a href=''javascript:select_report(""bar"",' + convert(varchar, rp_id) + ')''>select bar</a>'
	else
		'&nbsp;' end [chart],
'<a href=''javascript:select_report(""data"",' + convert(varchar, rp_id) + ')''>select data</a>' [data]
from reports order by rp_desc";

    ds = btnet.DbUtil.get_dataset(sql);

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet reports</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>

<script>

function select_report(type, id)
{
opener.add_selected_report(type, id)
window.close()

}

</script>

</head>

<body>

<div class=align>
</p>

<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

}
else
{
	Response.Write ("No reports in the database.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>