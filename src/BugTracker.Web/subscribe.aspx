<%@ Page language="C#" CodeBehind="subscribe.aspx.cs" Inherits="btnet.subscribe" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

SQLString sql;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	int bugid = Convert.ToInt32(Request["id"]);
	int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
	if (permission_level ==PermissionLevel.None)
	{
		Response.End();
	}

	if (Request.QueryString["ses"] != (string) Session["session_cookie"])
	{
		Response.Write ("session in URL doesn't match session cookie");
		Response.End();
	}

	if (Request.QueryString["actn"] == "1")
	{
		sql = new SQLString(@"insert into bug_subscriptions (bs_bug, bs_user)
			values(@bg, @us)");
	}
	else
	{
		sql = new SQLString(@"delete from bug_subscriptions
			where bs_bug = @bg and bs_user = @us");
	}

	sql = sql.AddParameterWithValue("bg", Util.sanitize_integer(Request["id"]));
	sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
	btnet.DbUtil.execute_nonquery(sql);

}



</script>
