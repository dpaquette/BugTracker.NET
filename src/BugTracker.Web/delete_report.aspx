<%@ Page language="C#" CodeBehind="delete_report.aspx.cs" Inherits="btnet.delete_report" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	if (security.user.is_admin || security.user.can_edit_reports)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}
    SQLString sql;
	if (IsPostBack)
	{
		// do delete here
		sql = new SQLString(@"
delete reports where rp_id = $1;
delete dashboard_items where ds_report = @reportId");
        sql = sql.AddParameterWithValue("reportId", Util.sanitize_integer(row_id.Value));
		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("reports.aspx");
	}
	else
	{
		titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "delete report";

		string id = Util.sanitize_integer(Request["id"] );

		sql = new SQLString(@"select rp_desc from reports where rp_id = @id");
		sql = sql.AddParameterWithValue("id", id);

		DataRow dr = btnet.DbUtil.get_datarow(sql);

		confirm_href.InnerText = "confirm delete of report: "
				+ Convert.ToString(dr["rp_desc"]);

		row_id.Value = id;

	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet delete report</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "reports"); %>
<p>
<div class=align>
<p>&nbsp</p>
<a href=reports.aspx>back to reports</a>

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


