<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">

DataSet ds;

Security security;
int bugid;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

    bugid = Convert.ToInt32(Util.sanitize_integer(Request["id"]));


    int permission_level = Bug.get_bug_permission_level(bugid, security);
    if (permission_level == Security.PERMISSION_NONE)
    {
        Response.Write("You are not allowed to view this item");
        Response.End();
    }

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "view git file commits";


	string sql = @"
select 
gitcom_commit [commit],
gitcom_repository [repo],
gitap_action [action],
gitap_path [file],
replace(replace(gitcom_author,'<','&lt;'),'>','&gt;') [user],
substring(gitcom_git_date,1,19) [date],
replace(substring(gitcom_msg,1,4000),char(13),'<br>') [msg],

case when gitap_action not like '%D%' and gitap_action not like 'A%' then
	'<a target=_blank href=git_diff.aspx?revpathid=' + convert(varchar,gitap_id) + '>diff</a>'
	else
	''
end [view<br>diff],

case when gitap_action not like '%D%' then
'<a target=_blank href=git_log.aspx?revpathid=' + convert(varchar,gitap_id) + '>history</a>'
	else
	''
end [view<br>history<br>(git log)]

from git_commits
inner join git_affected_paths on gitap_gitcom_id = gitcom_id
where gitcom_bug = $bg
order by gitcom_git_date desc, gitap_path";

	sql = sql.Replace("$bg", Convert.ToString(bugid));

	ds = btnet.DbUtil.get_dataset(sql);
}



</script>

<html>
<head>
<title id="titl" runat="server">btnet view git commits</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body width=600>
<div class=align>
Git File Commits for <% Response.Write(btnet.Util.get_setting("SingularBugLabel","bug")); %>&nbsp;<% Response.Write(Convert.ToString(bugid)); %>
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