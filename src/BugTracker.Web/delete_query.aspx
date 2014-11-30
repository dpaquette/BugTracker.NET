<%@ Page language="C#" CodeBehind="delete_query.aspx.cs" Inherits="btnet.delete_query" AutoEventWireup="True" %>
<%@ Register TagPrefix="uc1" Namespace="btnet.Controls" Assembly="BugTracker.Web" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

SQLString sql;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	if (IsPostBack)
	{
		// do delete here
		sql = new SQLString(@"delete queries where qu_id = @id");
        sql = sql.AddParameterWithValue("id", Util.sanitize_integer(row_id.Value));
		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("queries.aspx");
	}
	else
	{
		titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "delete query";

		string id = Util.sanitize_integer(Request["id"]);

		sql = new SQLString(@"select qu_desc, isnull(qu_user,0) qu_user from queries where qu_id = @id");
		sql = sql.AddParameterWithValue("id", id);

		DataRow dr = btnet.DbUtil.get_datarow(sql);

		if ((int) dr["qu_user"] != User.Identity.GetUserId())
		{
			if (User.IsInRole(BtnetRoles.Admin)|| security.user.can_edit_sql)
			{
				// can do anything
			}
			else
			{
				Response.Write ("You are not allowed to delete this item");
				Response.End();
			}
		}

		confirm_href.InnerText = "confirm delete of query: "
				+ Convert.ToString(dr["qu_desc"]);

		row_id.Value = id;

	}
}


</script>

<html>
<head>
<title id="titl" runat="server">btnet delete query</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="queries"/>
<p>
<div class=align>
<p>&nbsp</p>
<a href=queries.aspx>back to queries</a>

<p>or<p>

<script>
function submit_form()
{
    var frm = document.getElementById("frm");
    frm.submit();
    return true;
}

</script>
<form runat="server" id="frm">
<a id="confirm_href" runat="server" href="javascript: submit_form()"></a>
<input type="hidden" id="row_id" runat="server">
</form>

</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


