<%@ Page language="C#"%>
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
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit priority";

	msg.InnerText = "";

	string var = Request.QueryString["id"];
	if (var == null)
	{
		id = 0;
	}
	else
	{
		id = Convert.ToInt32(var);
	}

	if (!IsPostBack)
	{

		// add or edit?
		if (id == 0)
		{
			sub.Value = "Create";
		}
		else
		{
			sub.Value = "Update";

			// Get this entry's data from the db and fill in the form

			sql = @"select
				pr_name, pr_sort_seq, pr_background_color, isnull(pr_style,'') [pr_style], pr_default
				from priorities where pr_id = $1";

			sql = sql.Replace("$1", Convert.ToString(id));
			DataRow dr = btnet.DbUtil.get_datarow(sql);

			// Fill in this form
			name.Value = (string) dr["pr_name"];
			sort_seq.Value = Convert.ToString((int) dr["pr_sort_seq"]);
			color.Value = (string) dr["pr_background_color"];
			style.Value = (string) dr["pr_style"];
			default_selection.Checked = Convert.ToBoolean((int) dr["pr_default"]);

		}
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
	if (name.Value == "")
	{
		good = false;
		name_err.InnerText = "Description is required.";
	}
	else
	{
		name_err.InnerText = "";
	}

	if (sort_seq.Value == "")
	{
		good = false;
		sort_seq_err.InnerText = "Sort Sequence is required.";
	}
	else
	{
		sort_seq_err.InnerText = "";
	}

	if (!Util.is_int(sort_seq.Value))
	{
		good = false;
		sort_seq_err.InnerText = "Sort Sequence must be an integer.";
	}
	else
	{
		sort_seq_err.InnerText = "";
	}


	if (color.Value == "")
	{
		good = false;
		color_err.InnerText = "Background Color in #FFFFFF format is required.";
	}
	else
	{
		color_err.InnerText = "";
	}


	return good;
}

///////////////////////////////////////////////////////////////////////
void on_update()
{

	Boolean good = validate();

	if (good)
	{
		if (id == 0)  // insert new
		{
			sql = @"insert into priorities
				(pr_name, pr_sort_seq, pr_background_color, pr_style, pr_default)
				values (N'$na', $ss, N'$co', N'$st', $df)";
		}
		else // edit existing
		{

			sql = @"update priorities set
				pr_name = N'$na',
				pr_sort_seq = $ss,
				pr_background_color = N'$co',
				pr_style = N'$st',
				pr_default = $df
				where pr_id = $id";

			sql = sql.Replace("$id", Convert.ToString(id));

		}
		sql = sql.Replace("$na", name.Value.Replace("'","''"));
		sql = sql.Replace("$ss", sort_seq.Value);
		sql = sql.Replace("$co", color.Value.Replace("'","''"));
		sql = sql.Replace("$st", style.Value.Replace("'","''"));
		sql = sql.Replace("$df", Util.bool_to_string(default_selection.Checked));
		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("priorities.aspx");

	}
	else
	{
		if (id == 0)  // insert new
		{
			msg.InnerText = "Priority was not created.";
		}
		else // edit existing
		{
			msg.InnerText = "Priority was not updated.";
		}

	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit priority</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">

<script>

function change_sample_color()
{

	var sample = document.getElementById("sample");
	var color = document.getElementById("color");

	try
	{
		sample.style.background = color.value;
	}
	catch(e)
	{
	}


}
</script>

</head>
<body onload="change_sample_color()">
<% security.write_menu(Response, "admin"); %>


<div class=align><table border=0><tr><td>
<a href=priorities.aspx>back to priorities</a>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>Description:</td>
	<td><input runat="server" type=text class=txt id="name" maxlength=20 size=20></td>
	<td runat="server" class=err id="name_err">&nbsp;</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>Sort Sequence controls the sort order in the dropdowns.</span>
	</td>
	</tr>

	<tr>
	<td class=lbl>Sort Sequence:</td>
	<td><input runat="server" type=text class=txt id="sort_seq" maxlength=2 size=2></td>
	<td runat="server" class=err id="sort_seq_err">&nbsp;</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>Background Color and CSS Class can be used to control the look of lists.<br>See the example queries.</span>
	</td>
	</tr>

	<tr>
	<td class=lbl>Background Color:</td>
	<td><input onkeyup="change_sample_color()" runat="server" type=text class=txt id="color" value="#ffffff" maxlength=7 size=7>
	&nbsp;&nbsp;&nbsp;&nbsp;<span style="padding: 3px;" id="sample">&nbsp;&nbsp;Sample&nbsp;&nbsp;</span></td>
	<td runat="server" class=err id="color_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>CSS Class:</td>
	<td><input runat="server" type=text class=txt id="style" value="" maxlength=10 size=10>
	&nbsp;&nbsp;<a target=_blank href=edit_styles.aspx>more CSS info...</a>
	</td>
	<td runat="server" class=err id="style_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Default Selection:</td>
	<td><asp:checkbox runat="server" class=cb id="default_selection"/></td>
	<td>&nbsp</td>
	</tr>

	<tr><td colspan=3 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Create or Edit">
	<td>&nbsp</td>
	</td>
	</tr>
	</td></tr></table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


