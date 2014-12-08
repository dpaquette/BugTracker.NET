<%@ Page language="C#" CodeBehind="hg_view_revisions.aspx.cs" Inherits="btnet.hg_view_revisions" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">

DataSet ds;

int bugid;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
    bugid = Convert.ToInt32(Util.sanitize_integer(Request["id"]));


    int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
    if (permission_level ==PermissionLevel.None)
    {
        Response.Write("You are not allowed to view this item");
        Response.End();
    }

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "view hg file revisions";


	var sql = new SQLString(@"
select 
hgrev_revision [revision],
hgrev_repository [repo],
hgap_action [action],
hgap_path [file],
replace(replace(hgrev_author,'<','&lt;'),'>','&gt;') [user],
substring(hgrev_hg_date,1,19) [date],
replace(substring(hgrev_msg,1,4000),char(13),'<br>') [msg],

case when hgap_action not like '%D%' and hgap_action not like 'A%' then
	'<a target=_blank href=hg_diff.aspx?revpathid=' + convert(varchar,hgap_id) + '>diff</a>'
	else
	''
end [view<br>diff],

case when hgap_action not like '%D%' then
'<a target=_blank href=hg_log.aspx?revpathid=' + convert(varchar,hgap_id) + '>history</a>'
	else
	''
end [view<br>history<br>(hg log)]

from hg_revisions
inner join hg_affected_paths on hgap_hgrev_id = hgrev_id
where hgrev_bug = @bg
order by hgrev_hg_date desc, hgap_path");

	sql = sql.AddParameterWithValue("bg", Convert.ToString(bugid));

	ds = btnet.DbUtil.get_dataset(sql);
}


</script>

<html>
<head>
<title id="titl" runat="server">btnet view hg revisions</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body width=600>
<div class=align>
hg File revisions for <% Response.Write(btnet.Util.get_setting("SingularBugLabel","bug")); %>&nbsp;<% Response.Write(Convert.ToString(bugid)); %>
<p>
<%
if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

}
else
{
	Response.Write ("No revisions.");
}
%>
</div>
</body>
</html>
