<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

int bugid;
DataSet ds;

Security security;
int permission_level;
string ses;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);
	
	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "tasks";
	
	bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

	permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE)
	{
		Response.Write("You are not allowed to view tasks for this item");
		Response.End();
	}
	
	if (security.user.is_admin || security.user.can_view_tasks)
	{
		// allowed
	}
	else
	{
		Response.Write("You are not allowed to view tasks");
		Response.End();
	}
	

	ses = (string) Session["session_cookie"];
	
	string sql = "select tsk_id [id],";

	if (permission_level == Security.PERMISSION_ALL && !security.user.is_guest && (security.user.is_admin || security.user.can_edit_tasks))
	{
		sql += @"
'<a   href=edit_task.aspx?bugid=$bugid&id=' + convert(varchar,tsk_id) + '>edit</a>'   [$no_sort_edit],
'<a href=delete_task.aspx?ses=$ses&bugid=$bugid&id=' + convert(varchar,tsk_id) + '>delete</a>' [$no_sort_delete],";
	}

	sql += "tsk_description [description]";

	if (btnet.Util.get_setting("ShowTaskAssignedTo","1") == "1")
	{
		sql += ",us_username [assigned to]";
	}

	if (btnet.Util.get_setting("ShowTaskPlannedStartDate","1") == "1")
	{
		sql += ", tsk_planned_start_date [planned start]";
	}
	if (btnet.Util.get_setting("ShowTaskActualStartDate","1") == "1")
	{
		sql += ", tsk_actual_start_date [actual start]";
	}

	if (btnet.Util.get_setting("ShowTaskPlannedEndDate","1") == "1")
	{
		sql += ", tsk_planned_end_date [planned end]";
	}
	if (btnet.Util.get_setting("ShowTaskActualEndDate","1") == "1")
	{
		sql += ", tsk_actual_end_date [actual end]";
	}

	if (btnet.Util.get_setting("ShowTaskPlannedDuration","1") == "1")
	{
		sql += ", tsk_planned_duration [planned<br>duration]";
	}
	if (btnet.Util.get_setting("ShowTaskActualDuration","1") == "1")
	{
		sql += ", tsk_actual_duration  [actual<br>duration]";
	}


	if (btnet.Util.get_setting("ShowTaskDurationUnits","1") == "1")
	{
		sql += ", tsk_duration_units [duration<br>units]";
	}

	if (btnet.Util.get_setting("ShowTaskPercentComplete","1") == "1")
	{
		sql += ", tsk_percent_complete [percent<br>complete]";
	}

	if (btnet.Util.get_setting("ShowTaskStatus","1") == "1")
	{
		sql += ", st_name  [status]";
	}		

	if (btnet.Util.get_setting("ShowTaskSortSequence","1") == "1")
	{
		sql += ", tsk_sort_sequence  [seq]";
	}	

	sql += @"
from bug_tasks 
left outer join statuses on tsk_status = st_id
left outer join users on tsk_assigned_to_user = us_id
where tsk_bug = $bugid 
order by tsk_sort_sequence, tsk_id";

	sql = sql.Replace("$bugid", Convert.ToString(bugid));
	sql = sql.Replace("$ses", ses);
	
	ds = btnet.DbUtil.get_dataset(sql);

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet tasks</title>

<link rel="StyleSheet" href="btnet.css" type="text/css">

<script type="text/javascript" language="JavaScript" src="sortable.js"></script>


<script>

function body_on_load()
{
	parent.set_task_cnt(<% Response.Write(Convert.ToString(ds.Tables[0].Rows.Count)); %>)
}

</script>

</head>
<body onload="body_on_load()">


<div class=align>

Tasks for 
<% 
	Response.Write(btnet.Util.get_setting("SingularBugLabel","bug") 
	+ " " 
	+ Convert.ToString(bugid)); 
%>
<p>

<% if (permission_level == Security.PERMISSION_ALL && (security.user.is_admin || security.user.can_edit_tasks)) { %>
<a href=edit_task.aspx?id=0&bugid=<% Response.Write(Convert.ToString(bugid)); %>>add new task</a>
&nbsp;&nbsp;&nbsp;&nbsp;
<a target=_blank href=tasks_all.aspx>view all tasks</a>
&nbsp;&nbsp;&nbsp;&nbsp;
<a target=_blank href=tasks_all_excel.aspx>export all tasks to excel</a>
<p>

<% } %>

<%
if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false); 
}
else
{
	Response.Write ("No tasks.");
}

%>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


