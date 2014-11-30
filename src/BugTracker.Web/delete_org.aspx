<%@ Page language="C#" CodeBehind="delete_org.aspx.cs" Inherits="btnet.delete_org" AutoEventWireup="True" %>
<%@ Register TagPrefix="uc1" Namespace="btnet.Controls" Assembly="BugTracker.Web" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

SQLString sql;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	if (IsPostBack)
	{
		// do delete here
		sql = new SQLString(@"delete orgs where og_id = @orgid");
        sql = sql.AddParameterWithValue("orgid", Util.sanitize_integer(row_id.Value));
		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("orgs.aspx");
	}
	else
	{

		titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "delete organization";

		string id = Util.sanitize_integer(Request["id"]);

		sql = new SQLString(@"declare @cnt int
			select @cnt = count(1) from users where us_org = @orgid;
			select @cnt = @cnt + count(1) from queries where qu_org = @orgid;
			select @cnt = @cnt + count(1) from bugs where bg_org = @orgid;
			select og_name, @cnt [cnt] from orgs where og_id = @orgid");
		sql = sql.AddParameterWithValue("orgid", id);

		DataRow dr = btnet.DbUtil.get_datarow(sql);

		if ((int) dr["cnt"] > 0)
		{
			Response.Write ("You can't delete organization \""
				+ Convert.ToString(dr["og_name"])
				+ "\" because some bugs, users, queries still reference it.");
			Response.End();
		}
		else
		{
			confirm_href.InnerText = "confirm delete of \""
				+ Convert.ToString(dr["og_name"])
				+ "\"";

			row_id.Value = id;

		}

	}

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet delete organization</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<uc1:MainMenu runat="server" ID="MainMenu" SelectedItem="admin"/>
<p>
<div class=align>
<p>&nbsp</p>
<a href=orgs.aspx>back to organizations</a>

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


