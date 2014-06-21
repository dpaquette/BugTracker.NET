<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

int id;
int bugid;
String sql;


Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();

	security.check_security( HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

	if (security.user.is_admin || security.user.can_edit_and_delete_posts)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}


	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit attachment";

	msg.InnerText = "";

	string var = Request.QueryString["id"];
	id = Convert.ToInt32(var);

	var = Request.QueryString["bug_id"];
	bugid = Convert.ToInt32(var);

	int permission_level = btnet.Bug.get_bug_permission_level(bugid, security);
	if (permission_level != Security.PERMISSION_ALL)
	{
		Response.Write("You are not allowed to edit this item");
		Response.End();
	}


	if (security.user.external_user || Util.get_setting("EnableInternalOnlyPosts","0") == "0")
	{
		internal_only.Visible = false;
		internal_only_label.Visible = false;
	}

	if (!IsPostBack)
	{

		// Get this entry's data from the db and fill in the form

		sql = @"select bp_comment, bp_file, bp_hidden_from_external_users from bug_posts where bp_id = $1";
		sql = sql.Replace("$1", Convert.ToString(id));
		DataRow dr = btnet.DbUtil.get_datarow(sql);

		// Fill in this form
		desc.Value = (string) dr["bp_comment"];
		filename.InnerText = (string) dr["bp_file"];
		internal_only.Checked = Convert.ToBoolean((int) dr["bp_hidden_from_external_users"]);

	}
	else
	{
		on_update();
	}

}


///////////////////////////////////////////////////////////////////////
Boolean validate()
{

	Boolean good = true;

	return good;
}

///////////////////////////////////////////////////////////////////////
void on_update()
{

	Boolean good = validate();

	if (good)
	{

		sql = @"update bug_posts set
			bp_comment = N'$1',
			bp_hidden_from_external_users = $internal
			where bp_id = $3";

		sql = sql.Replace("$3", Convert.ToString(id));
		sql = sql.Replace("$1", desc.Value.Replace("'", "''"));
		sql = sql.Replace("$internal", btnet.Util.bool_to_string(internal_only.Checked));

		btnet.DbUtil.execute_nonquery(sql);

		if (!internal_only.Checked)
		{
			btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security);
		}

		Response.Redirect ("edit_bug.aspx?id=" + Convert.ToString(bugid));

	}
	else
	{
		msg.InnerText = "Attachment was not updated.";
	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit attachment</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, Util.get_setting("PluralBugLabel","bugs")); %>


<div class=align><table border=0><tr><td>
<a href=edit_bug.aspx?id=<% Response.Write(Convert.ToString(bugid));%>>back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>Description:</td>
	<td><input runat="server" type=text class=txt id="desc" maxlength=80 size=80></td>
	<td runat="server" class=err id="desc_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Filename:</td>
	<td><b><span id=filename runat="server">&nbsp;</span></b></td>
	<td>&nbsp;</td>
	</tr>


	<tr>
	<td colspan=3>
	<asp:checkbox runat="server" class=cb id="internal_only"/>
	<span runat="server" id="internal_only_label">Visible to internal users only</span>
	</td>
	</tr>


	<tr><td colspan=3 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Update">
	<td>&nbsp</td>
	</td>
	</tr>
	</td></tr></table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


