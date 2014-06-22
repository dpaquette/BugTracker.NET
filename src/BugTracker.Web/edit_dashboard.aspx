<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


Security security;
DataSet ds = null;
string ses = "";

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit dashboard";

	if (security.user.is_admin || security.user.can_use_reports)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}


	ses = (string) Session["session_cookie"];

	string sql = @"
select ds_id, ds_col, ds_row, ds_chart_type, rp_desc
from dashboard_items ds
inner join reports on rp_id = ds_report
where ds_user = $user
order by ds_col, ds_row";

	sql = sql.Replace("$user", Convert.ToString(security.user.usid));

	ds = btnet.DbUtil.get_dataset(sql);

}

void write_link(int id, string action, string text)
{

	Response.Write("<a href=update_dashboard.aspx?actn=");
	Response.Write(action);
	Response.Write("&ds_id=");
	Response.Write(Convert.ToString(id));
	Response.Write("&ses=");
	Response.Write(ses);
	Response.Write(">[");
	Response.Write(text);
	Response.Write("]</a>&nbsp;&nbsp;&nbsp;");

}

void write_column(int col)
{

	bool first_row = true;
	int last_row = -1;

	foreach (DataRow dr in ds.Tables[0].Rows)
	{

		if ((int) dr["ds_col"] == col)
		{
			last_row = (int) dr["ds_row"];
		}
	}


	foreach (DataRow dr in ds.Tables[0].Rows)
	{

		if ((int) dr["ds_col"] == col)
		{
			Response.Write("<div class=panel>");

				write_link((int) dr["ds_id"], "delete", "delete");

				if (first_row)
				{
					first_row = false;
				}
				else
				{
					write_link((int) dr["ds_id"], "moveup", "move up");
				}

				if ((int) dr["ds_row"] == last_row)
				{
					// skip
				}
				else
				{
					write_link((int) dr["ds_id"], "movedown", "move down");
				}

				//write_link((int) dr["ds_id"], "switchcols", "switch columns");

				Response.Write("<p><div style='text-align: center; font-weight: bold;'>");
				Response.Write((string) dr["rp_desc"] + "&nbsp;-&nbsp; " + (string) dr["ds_chart_type"]) ;
				Response.Write("</div>");

			Response.Write("</div>");
		}
	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet dashboard</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">


<style>

body {background: #ffffff;}

.panel {
	background: #ffffff;
	border: 3px solid #cccccc;
	padding: 10px;
	margin-bottom: 10px;
}

</style>



<script>

var col = 0

function show_select_report_page(which_col)
{

	col = which_col
	popup_window = window.open('select_report.aspx')
}

function add_selected_report(chart_type, id)
{
	var frm = document.getElementById("addform")
	frm.rp_chart_type.value = chart_type
	frm.rp_id.value = id
	frm.rp_col.value = col
	frm.submit()
}


</script>

</head>
<body>
<% security.write_menu(Response, "admin"); %>
<a href=dashboard.aspx>back to dashboard</a>
<table border=0 cellspacing=0 cellpadding=10>
<tr>

<td valign=top>&nbsp;<br>

<% write_column(1); %>

<div class=panel>
<a href="javascript:show_select_report_page(1)">[add report to dashboard column 1]</a>
</div>

<td valign=top>&nbsp;<br>

<% write_column(2); %>

<div class=panel>
<a href="javascript:show_select_report_page(2)">[add report to dashboard column 2]</a>
</div>


</table>
<form id="addform" method="get" action="update_dashboard.aspx">
<input type="hidden" name="rp_id">
<input type="hidden" name="rp_chart_type">
<input type="hidden" name="rp_col">
<input type="hidden" name="actn" value="add">
<input type="hidden" name="ses" value=<% Response.Write(ses); %>>
</form>


<% Response.Write(Application["custom_footer"]); %>
</body>
</html>