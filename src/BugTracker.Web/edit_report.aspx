<%@ Page language="C#" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">
int id;
String sql;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e) {
	Util.do_not_cache(Response);
	
	security = new Security();

	security.check_security( HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

	if (security.user.is_admin || security.user.can_edit_reports)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit report";

	msg.InnerText = "";

	string var = Request.QueryString["id"];
	if (var == null) {
		id = 0;
	} else {
		id = Convert.ToInt32(var);
	}

	if (!IsPostBack)
	{
		// add or edit?
		if (id == 0)
		{
			sub.Value = "Create";
			sql_text.Value = Request.Form["sql_text"]; // if coming from search.aspx
			table.Checked = true;
		}
		else
		{
			sub.Value = "Update";

			// Get this entry's data from the db and fill in the form
			sql = @"select
				rp_desc, rp_sql, rp_chart_type
				from reports where rp_id = $1";
			sql = sql.Replace("$1", Convert.ToString(id));
			DataRow dr = btnet.DbUtil.get_datarow(sql);

			// Fill in this form
			desc.Value = (string) dr["rp_desc"];
			
//			if (Util.get_setting("HtmlEncodeSql","0") == "1")
//			{
//				sql_text.Value = Server.HtmlEncode((string) dr["rp_sql"]);
//			}
//			else
//			{
				sql_text.Value = (string) dr["rp_sql"];
//			}
			
			switch ((string) dr["rp_chart_type"]) {
				case "pie":
					pie.Checked = true;
					break;
				case "bar":
					bar.Checked = true;
					break;
				case "line":
					line.Checked = true;
					break;
				default:
					table.Checked = true;
					break;
			}
		}
	}
	else
	{
		on_update();
	}
}


///////////////////////////////////////////////////////////////////////
Boolean validate() {
	Boolean good = true;

	if (desc.Value == "") {
		good = false;
		desc_err.InnerText = "Description is required.";
	} else {
		desc_err.InnerText = "";
	}

	if (sql_text.Value == "") {
		good = false;
		msg.InnerText = "The SQL statement is required.  ";
	} else {
		msg.InnerText = "";
	}

	return good;
}

///////////////////////////////////////////////////////////////////////
void on_update()
{
	Boolean good = validate();
	string ct;

	if (good)
	{
		if (id == 0) {  // insert new
			sql = @"insert into reports
				(rp_desc, rp_sql, rp_chart_type)
				values (N'$de', N'$sq', N'$ct')";
		}
		else
		{	// edit existing
			sql = @"update reports set
				rp_desc = N'$de',
				rp_sql = N'$sq',
				rp_chart_type = N'$ct'
				where rp_id = $id";
			sql = sql.Replace("$id", Convert.ToString(id));
		}

		sql = sql.Replace("$de", desc.Value.Replace("'","''"));
		sql = sql.Replace("$sq", Server.HtmlDecode(sql_text.Value.Replace("'", "''")));

		if (pie.Checked) {
			ct = "pie";
		} else if (bar.Checked) {
			ct = "bar";
		} else if (line.Checked) {
			ct = "line";
		} else {
			ct = "table";
		}

		sql = sql.Replace("$ct", ct);

		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer("reports.aspx");
	}
	else
	{
		if (id == 0)
		{  // insert new
			msg.InnerText += "Query was not created.";
		}
		else
		{	// edit existing
			msg.InnerText += "Query was not updated.";
		}
	}
}
</script>

<html>
<head>
<title id="titl" runat="server">btnet edit report</title>
<link rel="stylesheet" href="btnet.css" type="text/css">
<script language="Javascript" type="text/javascript" src="edit_area/edit_area_full.js"></script>

<script>
		editAreaLoader.init({
			id: "sql_text"	// id of the textarea to transform
			,start_highlight: true	// if start with highlight
			,toolbar: "search, go_to_line, undo, redo, help"
			,browsers: "all"
			,language: "en"
			,syntax: "sql"
			,allow_toggle: false
			,min_height: 300
			,min_width: 400
		});
</script>
</head>

<body>
<% security.write_menu(Response, "reports"); %>
<div class='align'>
<a href='reports.aspx'>back to reports</a>
<br><br>
<table border='0'>
	<tr>
		<td>
			<form class='frm' runat="server">
				<table border='0'>
					<tr>
						<td class='lbl'>Description:</td>
						<td><input runat="server" type='text' class='txt' id="desc" maxlength='80' size='80'></td>
						<td runat="server" class='err' id="desc_err">&nbsp;</td>
					</tr>
					<tr>
						<td class='lbl'>Chart Type:</td>
						<td>
							<asp:RadioButton text="Table" runat="server" GroupName="chart_type" id="table" />
							&nbsp;&nbsp;&nbsp;
							<asp:RadioButton text="Pie" runat="server" GroupName="chart_type" id="pie" />
							&nbsp;&nbsp;&nbsp;
							<asp:RadioButton text="Bar" runat="server" GroupName="chart_type" id="bar" />
							&nbsp;&nbsp;&nbsp;
							<asp:RadioButton text="Line" runat="server" GroupName="chart_type" id="line" />
							&nbsp;&nbsp;&nbsp;
						</td>
						<td runat="server" class='err' id="chart_type_err">&nbsp;</td>
					</tr>
					
					<tr>
						<td colspan=3>
						&nbsp;
						</td>
					</tr>
					
					<tr>
						<td class='lbl'>SQL:</td>
						<td colspan='2'>
							<textarea rows='10' cols='70' runat="server" class='txt' name="sql_text" id="sql_text"></textarea>
						</td>
					</tr>
					<tr>
						<td colspan='3' align='center'>
							<span runat="server" class='err' id="msg">&nbsp;</span>
						</td>
					</tr>
					<tr>
						<td colspan='2' align='center'>
							<input runat="server" class='btn' type='submit' id="sub" value="Create or Edit">
						</td>
						<td>&nbsp</td>
					</tr>
					<tr>
						<td>&nbsp</td>
						<td colspan='2' class='cmt'>
							To use "Pie", "Bar", or "Line", your SQL statement should have two columns <br>where the first column is the label and the second column contains the value.
							<br><br>
							You can use the pseudo-variable $ME in your report which will be replaced by your user ID.
							<br>
							For example:
							<ul>
								  select .... from ....<br>
	  							  where bg_assigned_to_user = $ME
	  						</ul>
						</td>
					</tr>
				</table>
			</form>
		</td>
	</tr>
</table>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


