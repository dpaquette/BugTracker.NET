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
		+ "edit custom column metadata";

	msg.InnerText = "";

	id = Convert.ToInt32(Util.sanitize_integer(Request["id"]));

	if (!IsPostBack)
	{

		// Get this entry's data from the db and fill in the form

		sql = @"
select sc.name,
isnull(ccm_dropdown_vals,'') [vals],
isnull(ccm_dropdown_type,'') [dropdown_type],
isnull(ccm_sort_seq, sc.colorder) [column order],
mm.text [default value], dflts.name [default name]
from syscolumns sc
inner join sysobjects so on sc.id = so.id
left outer join custom_col_metadata ccm on ccm_colorder = sc.colorder
left outer join syscomments mm on sc.cdefault = mm.id
left outer join sysobjects dflts on dflts.id = mm.id
where so.name = 'bugs'
and sc.colorder = $co";

		sql = sql.Replace("$co", Convert.ToString(id));
		DataRow dr = btnet.DbUtil.get_datarow(sql);

		name.InnerText = (string) dr["name"];
		dropdown_type.Value = Convert.ToString(dr["dropdown_type"]);

		if (dropdown_type.Value == "normal")
		{
			// show the dropdown vals
		}
		else
		{
			vals.Visible = false;
			vals_label.Visible = false;
			//vals_explanation.Visible = false;
		}

		// Fill in this form
		vals.Value = (string) dr["vals"];
		sort_seq.Value = Convert.ToString(dr["column order"]);
		default_value.Value = Convert.ToString(dr["default value"]);
		hidden_default_value.Value = default_value.Value; // to test if it changed
		hidden_default_name.Value = Convert.ToString(dr["default name"]);

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

	sort_seq_err.InnerText = "";
	vals_err.InnerText = "";

	if (sort_seq.Value == "")
	{
		good = false;
		sort_seq_err.InnerText = "Sort Sequence is required.";
	}


	if (!Util.is_int(sort_seq.Value))
	{
		good = false;
		sort_seq_err.InnerText = "Sort Sequence must be an integer.";
	}


	if (dropdown_type.Value == "normal")
	{
		if (vals.Value == "")
		{
			good = false;
			vals_err.InnerText = "Dropdown values are required for dropdown type of \"normal\".";
		}
		else
		{
			string vals_error_string = btnet.Util.validate_dropdown_values(vals.Value);
			if (!string.IsNullOrEmpty(vals_error_string))
			{
				good = false;
				vals_err.InnerText = vals_error_string;
			}
		}
	}

	return good;
}

///////////////////////////////////////////////////////////////////////
void on_update()
{

	Boolean good = validate();

	if (good)
	{

		sql = @"declare @count int
			select @count = count(1) from custom_col_metadata
			where ccm_colorder = $co

			if @count = 0
				insert into custom_col_metadata
				(ccm_colorder, ccm_dropdown_vals, ccm_sort_seq, ccm_dropdown_type)
				values($co, N'$v', $ss, '$dt')
			else
				update custom_col_metadata
				set ccm_dropdown_vals = N'$v',
				ccm_sort_seq = $ss
				where ccm_colorder = $co";

		sql = sql.Replace("$co", Convert.ToString(id));
		sql = sql.Replace("$v", vals.Value.Replace("'", "''"));
		sql = sql.Replace("$ss", sort_seq.Value);

		btnet.DbUtil.execute_nonquery(sql);
		Application["custom_columns_dataset"]  = null;
		
		if (default_value.Value != hidden_default_value.Value)
		{
			if (hidden_default_name.Value != "")
			{
				sql = "alter table bugs drop constraint [" + hidden_default_name.Value.Replace("'","''") + "]";
				btnet.DbUtil.execute_nonquery(sql);
				Application["custom_columns_dataset"]  = null;
			}
			
			if (default_value.Value != "")
			{
                sql = "alter table bugs add constraint [" + System.Guid.NewGuid().ToString() + "] default " + default_value.Value.Replace("'", "''") + " for [" + name.InnerText + "]";
				btnet.DbUtil.execute_nonquery(sql);
				Application["custom_columns_dataset"]  = null;
			}
		}

		Server.Transfer ("customfields.aspx");
	}
	else
	{
		msg.InnerText = "dropdown values were not updated.";
	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit val</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, Util.get_setting("PluralBugLabel","bugs")); %>


<div class=align><table border=0><tr><td>
<a href=customfields.aspx>back to custom fields</a><p>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td colspan=3>
	Field Name:&nbsp;<span class=smallnote style="font-size: 12pt; font-weight: bold;" id="name" runat="server">
	</span>
	</td>
	</tr>

<% if (dropdown_type.Value == "normal") { %>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote id="vals_explanation" runat="server">
	Use the following if you want the custom field to be a "normal" dropdown.
	<br>Create a pipe seperated list of values as shown below.
	<br>No individiual value should be longer than the length of your custom field.
	<br>Don't use commas, &gt;, &lt;, or quotes in the list of values.
	<br>Line breaks for your readability are ok.
	<br>Here are some examples:
	<br>
	"1.0|1.1|1.2"
	<br>
	"red|blue|green"
	<br>It's ok to have one of the values be blank:<br>
	"|red|blue|green"
	</span>
	</td>
	</tr>

<% } %>

	<tr>
	<td colspan=3>
	<br>
	<div class=lbl id="vals_label" runat="server">Normal Dropdown Values:</div><p>
	<textarea runat="server" class=txt id="vals" rows=6 cols=60></textarea>
	<br>
	<span runat="server" class=err id="vals_err">&nbsp;</span>
	</td>
	</tr>

	<tr>
	<td colspan=2 class=lbl>Default:
	&nbsp;&nbsp;<input runat="server" type=text class=txt id="default_value" maxlength=50 size=50></td>
	<input runat="server" type=hidden id="hidden_default_value">
	<input runat="server" type=hidden id="hidden_default_name">
	<td runat="server" class=err id="default_value_error">&nbsp;</td>
	</tr>

	
	<tr>
	<td colspan=3>
	<span class=smallnote>
	Controls what order the custom fields display on the page.
	</span>
	</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=2 class=lbl>Sort Sequence:
	&nbsp;&nbsp;<input runat="server" type=text class=txt id="sort_seq" maxlength=2 size=2></td>
	<td runat="server" class=err id="sort_seq_err">&nbsp;</td>
	</tr>


	<tr><td colspan=3 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr>
	<td colspan=2 align=center>
	<input runat="server" class=btn type=submit id="sub" value="Update">
	</td>
	<td>&nbsp;</td>
	</tr>

	</table>

	<input type="hidden" id="dropdown_type" runat="server">
</form>

</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


