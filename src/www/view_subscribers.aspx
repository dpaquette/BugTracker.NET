<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


Security security;
string sql;
int bugid;
DataSet ds;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "view subscribers";

	bugid = Convert.ToInt32(Util.sanitize_integer(Request["id"]));

	int permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE)
	{
		Response.Write("You are not allowed to view this item");
		Response.End();
	}


	string action = Request["actn"];

	if (action == null)
	{
		action = "";
	}

	if (action != "")
	{
		if (permission_level == Security.PERMISSION_READONLY)
		{
			Response.Write("You are not allowed to edit this item");
			Response.End();
		}
		if (!string.IsNullOrEmpty(Request["userid"]))
		{
            int new_subscriber_userid = Convert.ToInt32(Request["userid"]);

            sql = @"delete from bug_subscriptions where bs_bug = $bg and bs_user = $us;
			insert into bug_subscriptions (bs_bug, bs_user) values($bg, $us)";					;
			sql = sql.Replace("$bg",Convert.ToString(bugid));
			sql = sql.Replace("$us",Convert.ToString(new_subscriber_userid));
			btnet.DbUtil.execute_nonquery(sql);

			// send a notification to this user only
            btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security, new_subscriber_userid);
		}
	}


	// clean up bug subscriptions that no longer fit the security restrictions

	btnet.Bug.auto_subscribe(bugid);

	// show who is subscribed

	if (security.user.is_admin)
	{
		sql = @"
select
'<a href=delete_subscriber.aspx?ses=$ses&bg_id=$bg&us_id=' + convert(varchar,us_id) + '>unsubscribe</a>'	[$no_sort_unsubscriber],
us_username [user],
us_lastname + ', ' + us_firstname [name],
us_email [email],
case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
from bug_subscriptions
inner join users on bs_user = us_id
where bs_bug = $bg
and us_enable_notifications = 1
and us_active = 1
order by 1";

		sql = sql.Replace("$ses", Convert.ToString(Session["session_cookie"]));

	}
	else
	{
		sql = @"
select
us_username [user],
us_lastname + ', ' + us_firstname [name],
us_email [email],
case when us_reported_notifications < 4 or us_assigned_notifications < 4 or us_subscribed_notifications < 4 then 'Y' else 'N' end [user is<br>filtering<br>notifications]
from bug_subscriptions
inner join users on bs_user = us_id
where bs_bug = $bg
and us_enable_notifications = 1
and us_active = 1
order by 1";
	}

	sql = sql.Replace("$bg", Convert.ToString(bugid));
	ds = btnet.DbUtil.get_dataset(sql);

	// Get list of users who could be subscribed to this bug.

	sql = @"
declare @project int;
declare @org int;
select @project = bg_project, @org = bg_org from bugs where bg_id = $bg;";

	// Only users explicitly allowed will be listed
	if (Util.get_setting("DefaultPermissionLevel","2") == "0")
	{
		sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
			from users
			where us_active = 1
			and us_enable_notifications = 1
			and us_id in
				(select pu_user from project_user_xref
				where pu_project = @project
				and pu_permission_level <> 0)
			and us_id not in (
				select us_id
				from bug_subscriptions
				inner join users on bs_user = us_id
				where bs_bug = $bg
				and us_enable_notifications = 1
				and us_active = 1)
			and us_id not in (
				select us_id from users
				inner join orgs on us_org = og_id
				where us_org <> @org
				and og_other_orgs_permission_level = 0)

			order by us_username; ";
	}
	// Only users explictly DISallowed will be omitted
	else
	{
		sql += @"select us_id, case when $fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
			from users
			where us_active = 1
			and us_enable_notifications = 1
			and us_id not in
				(select pu_user from project_user_xref
				where pu_project = @project
				and pu_permission_level = 0)
			and us_id not in (
				select us_id
				from bug_subscriptions
				inner join users on bs_user = us_id
				where bs_bug = $bg
				and us_enable_notifications = 1
				and us_active = 1)
			and us_id not in (
				select us_id from users
				inner join orgs on us_org = og_id
				where us_org <> @org
				and og_other_orgs_permission_level = 0)
			order by us_username; ";
	}

	if (Util.get_setting("UseFullNames","0") == "0")
	{
		// false condition
		sql = sql.Replace("$fullnames","0 = 1");
	}
	else
	{
		// true condition
		sql = sql.Replace("$fullnames","1 = 1");
	}

	sql = sql.Replace("$bg", Convert.ToString(bugid));

	//DataSet ds_users =
	userid.DataSource = btnet.DbUtil.get_dataview(sql);
	userid.DataTextField = "us_username";
	userid.DataValueField = "us_id";
	userid.DataBind();

	if (userid.Items.Count == 0)
	{
		userid.Items.Insert(0, new ListItem("[no users to select]", "0"));
	}
	else
	{
		userid.Items.Insert(0, new ListItem("[select to add]", "0"));
	}

}



</script>

<html>
<head>
<title id="titl" runat="server">btnet view subscribers</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body width=600>
<div class=align>
Subscribers for <% Response.Write(Convert.ToString(bugid)); %>
<p>

<table border=0><tr><td>
<form class=frm runat="server" action="view_subscribers.aspx">
<table>
<tr><td><span class=lbl>add subscriber:</span>

<asp:DropDownList id="userid" runat="server">
</asp:DropDownList>

<tr><td colspan=2><input class=btn type=submit value="Add">
<tr><td colspan=2>&nbsp;<span runat="server" class='err' id="add_err"></span>
</table>
<input type=hidden name="id" value=<% Response.Write(Convert.ToString(bugid));%>>
<input type=hidden name="actn" value="add">

</form>
</td></tr></table>

<%
if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

}
else
{
	Response.Write ("No subscribers for this bug.");
}
%>
</div>
</body>
</html>