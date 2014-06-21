<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

int bugid;
int previd;
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
			+ "relationships";
	
	
	string sql;
	add_err.InnerText = "";

	bugid = Convert.ToInt32(Util.sanitize_integer(Request["bgid"]));
	
	if (string.IsNullOrEmpty(Request["bugid"]))
	{
		previd = 0;
	}
	else
	{
		previd = Convert.ToInt32(Util.sanitize_integer(Request["prev"]));
	}
	
	
	int bugid2 = 0;

	permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE)
	{
		Response.Write("You are not allowed to view this item");
		Response.End();
	}

	ses = (string) Session["session_cookie"];		
	string action = Request["actn"];

	if (!string.IsNullOrEmpty(action))
	{
		if (Request["ses"] != ses)
		{
			Response.Write ("session in Request doesn't match session cookie");
			Response.End();		
		}
		
		if (permission_level == Security.PERMISSION_READONLY)
		{
			Response.Write("You are not allowed to edit this item");
			Response.End();
		}

		if (action == "remove") // remove
		{
			if (security.user.is_guest)
			{
				Response.Write("You are not allowed to delete a relationship");
				Response.End();
			}

			bugid2 = Convert.ToInt32(Util.sanitize_integer(Request["bugid2"]));

			sql = @"
				delete from bug_relationships where re_bug2 = $bg2 and re_bug1 = $bg;
				delete from bug_relationships where re_bug1 = $bg2 and re_bug2 = $bg;
				insert into bug_posts
						(bp_bug, bp_user, bp_date, bp_comment, bp_type)
						values($bg, $us, getdate(), N'deleted relationship to $bg2', 'update')";
			sql = sql.Replace("$bg2",Convert.ToString(bugid2));
			sql = sql.Replace("$bg",Convert.ToString(bugid));
			sql = sql.Replace("$us",Convert.ToString(security.user.usid));
			btnet.DbUtil.execute_nonquery(sql);
		}
		else
		{

			// adding

			if (Request["bugid2"] != null)
			{
				if (!Util.is_int(Request["bugid2"]))
				{
					add_err.InnerText = "Related ID must be an integer.";
				}
				else
				{
					bugid2 = Convert.ToInt32((Request["bugid2"]));

					if (bugid == bugid2)
					{
						add_err.InnerText = "Cannot create a relationship to self.";
					}
					else
					{
						int rows = 0;

						// check if bug exists
						sql = @"select count(1) from bugs where bg_id = $bg2";
						sql = sql.Replace("$bg2",Convert.ToString(bugid2));
						rows = (int) btnet.DbUtil.execute_scalar(sql);

						if (rows == 0)
						{
							add_err.InnerText = "Not found.";
						}
						else
						{
							// check if relationship exists
							sql = @"select count(1) from bug_relationships where re_bug1 = $bg and re_bug2 = $bg2";
							sql = sql.Replace("$bg2",Convert.ToString(bugid2));
							sql = sql.Replace("$bg",Convert.ToString(bugid));
							rows = (int) btnet.DbUtil.execute_scalar(sql);

							if (rows > 0)
							{
								add_err.InnerText = "Relationship already exists.";
							}
							else
							{
								// check permission of related bug
								int permission_level2 = Bug.get_bug_permission_level(bugid2, security);
								if (permission_level2 == Security.PERMISSION_NONE)
								{
									add_err.InnerText = "You are not allowed to view the related item.";
								}
								else
								{

									// insert the relationship both ways
									sql = @"
insert into bug_relationships (re_bug1, re_bug2, re_type, re_direction) values($bg, $bg2, N'$ty', $dir1);
insert into bug_relationships (re_bug2, re_bug1, re_type, re_direction) values($bg, $bg2, N'$ty', $dir2);
insert into bug_posts
	(bp_bug, bp_user, bp_date, bp_comment, bp_type)
	values($bg, $us, getdate(), N'added relationship to $bg2', 'update');";

									sql = sql.Replace("$bg2",Convert.ToString(bugid2));
									sql = sql.Replace("$bg",Convert.ToString(bugid));
									sql = sql.Replace("$us",Convert.ToString(security.user.usid));
									sql = sql.Replace("$ty",Request["type"].Replace("'","''"));


									if (siblings.Checked )
									{
										sql = sql.Replace("$dir2","0");
										sql = sql.Replace("$dir1","0");
									}
									else if (child_to_parent.Checked)
									{
										sql = sql.Replace("$dir2","1");
										sql = sql.Replace("$dir1","2");
									}
									else
									{
										sql = sql.Replace("$dir2","2");
										sql = sql.Replace("$dir1","1");
									}

									btnet.DbUtil.execute_nonquery(sql);
									add_err.InnerText = "Relationship was added.";
								}
							}
						}
					}
				}
			}
		}

	}

	sql = @"
select bg_id [id],
	bg_short_desc [desc],
	re_type [comment],
	st_name [status],
	case
		when re_direction = 0 then ''
		when re_direction = 2 then 'child of $bg'
		else                       'parent of $bg' 
	end as [parent or child],
	'<a target=_blank href=edit_bug.aspx?id=' + convert(varchar,bg_id) + '>view</a>' [view]";

		if (!security.user.is_guest && permission_level == Security.PERMISSION_ALL)
		{

			sql += @"
,'<a href=''javascript:remove(' + convert(varchar,re_bug2) + ')''>detach</a>' [detach]"; 
		}

		sql += @"
from bugs
inner join bug_relationships on bg_id = re_bug2
left outer join statuses on st_id = bg_status
where re_bug1 = $bg
order by bg_id desc";


	sql = sql.Replace("$bg", Convert.ToString(bugid));
	sql = Util.alter_sql_per_project_permissions(sql, security);

	ds = btnet.DbUtil.get_dataset(sql);
	
	bgid.Value = Convert.ToString(bugid);
	
}

///////////////////////////////////////////////////////////////////////
string get_bug_html(DataRow dr)
{
	string s = @"
	
	
<td valign=top>
<div
style='background: #dddddd; border: 1px solid blue; padding 15px;  width: 140px; height: 50px; overflow: hidden;'
><a 
href='relationships.aspx?bgid=$id&prev=$prev'>$id&nbsp;&nbsp;&nbsp;&nbsp;$title</a></div>";

	
	if (previd == (int) dr["id"])
	{
		s = s.Replace("1px solid blue", "2px solid red");
	}
	
	s = s.Replace("$id", Convert.ToString(dr["id"]));
	s = s.Replace("$prev", Convert.ToString(bugid));
	s = s.Replace(
		"$title", 
		Server.HtmlEncode(
				Convert.ToString(dr["desc"])
			)
		);

	return s;

}


///////////////////////////////////////////////////////////////////////
void display_hierarchy()
{

	string parents = "";
	string siblings = "";
	string children = "";
	

	foreach (DataRow dr in ds.Tables[0].Rows)
	{
		string level = (string) dr["parent or child"];
		
		if (level.StartsWith("parent"))
		{
			parents += get_bug_html(dr);
		}
		else if (level.StartsWith("child"))
		{
			children += get_bug_html(dr);
		}
		else
		{
			siblings += get_bug_html(dr);
		}
	}

	Response.Write("Parents:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
	Response.Write(parents);
	Response.Write("</table><p>");
	Response.Write("Siblings:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
	Response.Write(siblings);
	Response.Write("</table><p>");
	Response.Write("Children:&nbsp;<table border=0 cellspacing=15 cellpadding=0><tr>");
	Response.Write(children);
	Response.Write("</table>");
}

</script>

<html>
<head>
<title id="titl" runat="server">btnet related bugs</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>

<script>

var asp_form_id = '<% Response.Write(Util.get_form_name()); %>';

function remove(bugid2_arg)
{
    var frm =  document.getElementById(asp_form_id)
    var actn = document.getElementById("actn")
    actn.value = "remove"
    document.getElementById("bugid2").value = bugid2_arg
    frm.submit()
}

function body_on_load()
{

	opener.set_relationship_cnt(
	<%
		Response.Write(Convert.ToString(bugid));
		Response.Write(",");
		Response.Write(Convert.ToString(ds.Tables[0].Rows.Count));
	%>
	)
}

</script>
</head>

<body onload="body_on_load()">

<div class=align>
Relationships for 
<% 
	Response.Write(btnet.Util.get_setting("SingularBugLabel","bug") 
	+ " " 
	+ Convert.ToString(bugid)); 
%>

<p>
<table border=0><tr><td>

<%
if (permission_level != Security.PERMISSION_READONLY)
{
%>
<p>
<form class=frm runat="server" action="relationships.aspx">
<table>
<tr><td>Related ID:<td><input type="text" class="txt" id="bugid2" name="bugid2" size=8>
<tr><td>Comment:<td><input type="text" class="txt" id="type" name="type" size=90 maxlength=500>

<tr><td colspan=2>
Related ID is sibling<asp:RadioButton runat="server" checked="true" GroupName="direction" value="0" id="siblings"/>
&nbsp;&nbsp;&nbsp;
Related ID is child<asp:RadioButton runat="server" GroupName="direction" value="1" id="child_to_parent"/>
&nbsp;&nbsp;&nbsp;
Related ID is parent<asp:RadioButton runat="server" GroupName="direction" value="2" id="parent_to_child"/>
&nbsp;&nbsp;&nbsp;
<tr><td colspan=2><input class="btn" type="submit" value="Add">
<tr><td colspan=2>&nbsp;
<tr><td colspan=2>&nbsp;<span runat="server" class="err" id="add_err"></span>
</table>
<input runat="server" id="bgid" type=hidden name="bgid" value="">
<input id="actn" type=hidden name="actn" value="add">
<input id="ses" type=hidden name="ses" value="<% Response.Write(ses); %>">

</form>
<% } %>

</td></tr></table>

</p>
<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "", false);

	display_hierarchy();
}
else
{
	Response.Write ("No related " + Util.get_setting("PluralBugLabel","bugs"));
}

%>
</div>
</body>
</html>