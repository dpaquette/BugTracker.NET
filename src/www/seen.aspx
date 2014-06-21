<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

String sql;

Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	if (!security.user.is_guest)
	{
		if (Request.QueryString["ses"] != (string) Session["session_cookie"])
		{
			Response.Write ("session in URL doesn't match session cookie");
			Response.End();
		}
	}

	DataView dv = (DataView) Session["bugs"];
	if (dv == null)
	{
		Response.End();
	}

	int bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

	int permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE)
	{
		Response.End();
	}

	for (int i = 0; i < dv.Count; i++)
	{
		if ((int)dv[i][1] == bugid)
		{
			int seen = Convert.ToInt32(Util.sanitize_integer(Request["seen"]));
			dv[i]["$SEEN"] = seen;
			sql = @"
if not exists (select bu_bug from bug_user where bu_bug = $bg and bu_user = $us)
	insert into bug_user (bu_bug, bu_user, bu_flag, bu_seen, bu_vote) values($bg, $us, 0, 1, 0) 
update bug_user set bu_seen = $seen, bu_seen_datetime = getdate() where bu_bug = $bg and bu_user = $us and bu_seen <> $seen";

			sql = sql.Replace("$seen", Convert.ToString(seen));
			sql = sql.Replace("$bg", Convert.ToString(bugid));
			sql = sql.Replace("$us", Convert.ToString(security.user.usid));

			btnet.DbUtil.execute_nonquery(sql);

			break;
		}
	}

}



</script>