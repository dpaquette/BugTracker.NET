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
		+ "edit user defined attribute value";

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

			sql = @"select udf_name, udf_sort_seq, udf_default from user_defined_attribute where udf_id = $1";
			sql = sql.Replace("$1", Convert.ToString(id));
			DataRow dr = btnet.DbUtil.get_datarow(sql);

			// Fill in this form
			name.Value = (string) dr[0];
			sort_seq.Value = Convert.ToString((int) dr[1]);
			default_selection.Checked = Convert.ToBoolean((int) dr["udf_default"]);
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
			sql = "insert into user_defined_attribute (udf_name, udf_sort_seq, udf_default) values (N'$na', $ss, $df)";
		}
		else // edit existing
		{

			sql = @"update user_defined_attribute set
				udf_name = N'$na',
				udf_sort_seq = $ss,
				udf_default = $df
				where udf_id = $id";

			sql = sql.Replace("$id", Convert.ToString(id));

		}
		sql = sql.Replace("$na", name.Value.Replace("'","''"));
		sql = sql.Replace("$ss", sort_seq.Value);
		sql = sql.Replace("$df", Util.bool_to_string(default_selection.Checked));
		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("udfs.aspx");

	}
	else
	{
		if (id == 0)  // insert new
		{
			msg.InnerText = "User defined attribute value was not created.";
		}
		else // edit existing
		{
			msg.InnerText = "User defined attribute value was not updated.";
		}

	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit user defined attribute value</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>


<div class=align><table border=0><tr><td>
<a href=udfs.aspx>back to user defined attribute values</a>
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


