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
		+ "edit project";

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

		default_user.DataSource =
			btnet.DbUtil.get_dataview("select us_id, us_username from users order by us_username");
		default_user.DataTextField = "us_username";
		default_user.DataValueField = "us_id";
		default_user.DataBind();
		default_user.Items.Insert(0, new ListItem("", "0"));


		// add or edit?
		if (id == 0)
		{
			sub.Value = "Create";
			active.Checked = true;
		}
		else
		{
			sub.Value = "Update";

			// Get this entry's data from the db and fill in the form

			sql = @"select
			pj_name,
			pj_active,
			isnull(pj_default_user,0) [pj_default_user],
			pj_default,
			isnull(pj_auto_assign_default_user,0) [pj_auto_assign_default_user],
			isnull(pj_auto_subscribe_default_user,0) [pj_auto_subscribe_default_user],
			isnull(pj_enable_pop3,0) [pj_enable_pop3],
			isnull(pj_pop3_username,'') [pj_pop3_username],
			isnull(pj_pop3_email_from,'') [pj_pop3_email_from],
			isnull(pj_description,'') [pj_description],
			isnull(pj_enable_custom_dropdown1,0) [pj_enable_custom_dropdown1],
			isnull(pj_enable_custom_dropdown2,0) [pj_enable_custom_dropdown2],
			isnull(pj_enable_custom_dropdown3,0) [pj_enable_custom_dropdown3],
			isnull(pj_custom_dropdown_label1,'') [pj_custom_dropdown_label1],
			isnull(pj_custom_dropdown_label2,'') [pj_custom_dropdown_label2],
			isnull(pj_custom_dropdown_label3,'') [pj_custom_dropdown_label3],
			isnull(pj_custom_dropdown_values1,'') [pj_custom_dropdown_values1],
			isnull(pj_custom_dropdown_values2,'') [pj_custom_dropdown_values2],
			isnull(pj_custom_dropdown_values3,'') [pj_custom_dropdown_values3]
			from projects
			where pj_id = $1";
			sql = sql.Replace("$1", Convert.ToString(id));
			DataRow dr = btnet.DbUtil.get_datarow(sql);

			// Fill in this form
			name.Value = (string) dr["pj_name"];
			active.Checked = Convert.ToBoolean((int) dr["pj_active"]);
			auto_assign.Checked = Convert.ToBoolean((int) dr["pj_auto_assign_default_user"]);
			auto_subscribe.Checked = Convert.ToBoolean((int) dr["pj_auto_subscribe_default_user"]);
			default_selection.Checked = Convert.ToBoolean((int) dr["pj_default"]);
			enable_pop3.Checked = Convert.ToBoolean((int) dr["pj_enable_pop3"]);
			pop3_username.Value = (string) dr["pj_pop3_username"];
			pop3_email_from.Value = (string) dr["pj_pop3_email_from"];

			enable_custom_dropdown1.Checked = Convert.ToBoolean((int) dr["pj_enable_custom_dropdown1"]);
			enable_custom_dropdown2.Checked = Convert.ToBoolean((int) dr["pj_enable_custom_dropdown2"]);
			enable_custom_dropdown3.Checked = Convert.ToBoolean((int) dr["pj_enable_custom_dropdown3"]);

			custom_dropdown_label1.Value = (string) dr["pj_custom_dropdown_label1"];
			custom_dropdown_label2.Value = (string) dr["pj_custom_dropdown_label2"];
			custom_dropdown_label3.Value = (string) dr["pj_custom_dropdown_label3"];

			custom_dropdown_values1.Value = (string) dr["pj_custom_dropdown_values1"];
			custom_dropdown_values2.Value = (string) dr["pj_custom_dropdown_values2"];
			custom_dropdown_values3.Value = (string) dr["pj_custom_dropdown_values3"];

			desc.Value = (string) dr["pj_description"];

			foreach (ListItem li in default_user.Items)
			{
				if (Convert.ToInt32(li.Value) == (int) dr["pj_default_user"])
				{
					li.Selected = true;
					break;
				}
			}

			permissions_href.HRef = "edit_user_permissions2.aspx?id=" + Convert.ToString(id)
				+ "&label=" + HttpUtility.UrlEncode(name.Value);

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
	
	string vals_error_string = "";
	bool errors_with_custom_dropdowns = false;
	vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values1.Value);
	if (!string.IsNullOrEmpty(vals_error_string))
	{
		good = false;
		custom_dropdown_values1_err.InnerText = vals_error_string;
		errors_with_custom_dropdowns = true;
	}
	else
	{
		custom_dropdown_values1_err.InnerText = "";
	}
	
	vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values2.Value);
	if (!string.IsNullOrEmpty(vals_error_string))
	{
		good = false;
		custom_dropdown_values2_err.InnerText = vals_error_string;
		errors_with_custom_dropdowns = true;
	}
	else
	{
		custom_dropdown_values2_err.InnerText = "";
	}
	
	vals_error_string = btnet.Util.validate_dropdown_values(custom_dropdown_values3.Value);
	if (!string.IsNullOrEmpty(vals_error_string))
	{
		good = false;
		custom_dropdown_values3_err.InnerText = vals_error_string;
		errors_with_custom_dropdowns = true;
	}
	else
	{
		custom_dropdown_values3_err.InnerText = "";
	}


	if (errors_with_custom_dropdowns)
	{
		msg.InnerText += "Custom fields have errors.  ";
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
			sql = @"insert into projects
			(pj_name, pj_active, pj_default_user, pj_default, pj_auto_assign_default_user, pj_auto_subscribe_default_user,
			pj_enable_pop3,
			pj_pop3_username,
			pj_pop3_password,
			pj_pop3_email_from,
			pj_description,
			pj_enable_custom_dropdown1,
			pj_enable_custom_dropdown2,
			pj_enable_custom_dropdown3,
			pj_custom_dropdown_label1,
			pj_custom_dropdown_label2,
			pj_custom_dropdown_label3,
			pj_custom_dropdown_values1,
			pj_custom_dropdown_values2,
			pj_custom_dropdown_values3)
			values (N'$name', $active, $defaultuser, $defaultsel, $autoasg, $autosub,
			$enablepop, N'$popuser',N'$poppass',N'$popfrom',
			N'$desc', 
			$ecd1,$ecd2,$ecd3,
			N'$cdl1',N'$cdl2',N'$cdl3',
			N'$cdv1',N'$cdv2',N'$cdv3')";

			sql = sql.Replace("$poppass", pop3_password.Value.Replace("'","''"));


		}
		else // edit existing
		{

			sql = @"update projects set
				pj_name = N'$name',
				$POP3_PASSWORD
				pj_active = $active,
				pj_default_user = $defaultuser,
				pj_default = $defaultsel,
				pj_auto_assign_default_user = $autoasg,
				pj_auto_subscribe_default_user = $autosub,
				pj_enable_pop3 = $enablepop,
				pj_pop3_username = N'$popuser',
				pj_pop3_email_from = N'$popfrom',
				pj_description = N'$desc',
				pj_enable_custom_dropdown1 = $ecd1,
				pj_enable_custom_dropdown2 = $ecd2,
				pj_enable_custom_dropdown3 = $ecd3,
				pj_custom_dropdown_label1 = N'$cdl1',
				pj_custom_dropdown_label2 = N'$cdl2',
				pj_custom_dropdown_label3 = N'$cdl3',
				pj_custom_dropdown_values1 = N'$cdv1',
				pj_custom_dropdown_values2 = N'$cdv2',
				pj_custom_dropdown_values3 = N'$cdv3'
				where pj_id = $id";
			sql = sql.Replace("$id", Convert.ToString(id));

			if (pop3_password.Value != "")
			{
				sql = sql.Replace("$POP3_PASSWORD", "pj_pop3_password = N'" +  pop3_password.Value.Replace("'","''") + "',");
			}
			else
			{
				sql = sql.Replace("$POP3_PASSWORD","");
			}

		}



		sql = sql.Replace("$name", name.Value.Replace("'","''"));
		sql = sql.Replace("$active", Util.bool_to_string(active.Checked));
		sql = sql.Replace("$defaultuser", default_user.SelectedItem.Value);
		sql = sql.Replace("$autoasg", Util.bool_to_string(auto_assign.Checked));
		sql = sql.Replace("$autosub", Util.bool_to_string(auto_subscribe.Checked));
		sql = sql.Replace("$defaultsel", Util.bool_to_string(default_selection.Checked));
		sql = sql.Replace("$enablepop", Util.bool_to_string(enable_pop3.Checked));
		sql = sql.Replace("$popuser", pop3_username.Value.Replace("'","''"));
		sql = sql.Replace("$popfrom", pop3_email_from.Value.Replace("'","''"));

		sql = sql.Replace("$desc", desc.Value.Replace("'","''"));

		sql = sql.Replace("$ecd1", Util.bool_to_string(enable_custom_dropdown1.Checked));
		sql = sql.Replace("$ecd2", Util.bool_to_string(enable_custom_dropdown2.Checked));
		sql = sql.Replace("$ecd3", Util.bool_to_string(enable_custom_dropdown3.Checked));


		sql = sql.Replace("$cdl1", custom_dropdown_label1.Value.Replace("'","''"));
		sql = sql.Replace("$cdl2", custom_dropdown_label2.Value.Replace("'","''"));
		sql = sql.Replace("$cdl3", custom_dropdown_label3.Value.Replace("'","''"));

		sql = sql.Replace("$cdv1", custom_dropdown_values1.Value.Replace("'","''"));
		sql = sql.Replace("$cdv2", custom_dropdown_values2.Value.Replace("'","''"));
		sql = sql.Replace("$cdv3", custom_dropdown_values3.Value.Replace("'","''"));

		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("projects.aspx");

	}
	else
	{
		if (id == 0)  // insert new
		{
			msg.InnerText += "Project was not created.";
		}
		else // edit existing
		{
			msg.InnerText += "Project was not updated.";
		}

	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit project</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>


<div class=align><table border=0><tr><td>
<a href=projects.aspx>back to projects</a>
&nbsp;&nbsp;&nbsp;&nbsp;

<% if (id != 0) { %>
<a id="permissions_href" runat="server" href="" style='font-weight: bold;'>per user permissions</a>
<% } %>

<script>

var cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";

function show_main_settings()
{
	document.getElementById("tab1").style.display = "block";
	document.getElementById("tab2").style.display = "none";
	document.getElementById("tab3").style.display = "none";
	document.getElementById("main_btn").setAttribute(cls, 'tab_btn_pushed');
	document.getElementById("custom_btn").setAttribute(cls, 'tab_btn');
}

function show_custom_field_settings()
{
	document.getElementById("tab1").style.display = "none";
	document.getElementById("tab2").style.display = "block";
	document.getElementById("tab3").style.display = "none";
	document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
	document.getElementById("custom_btn").setAttribute(cls, 'tab_btn_pushed');
}




</script>
<br><br>
<form class=frm runat="server">

	<br>
	<a id="main_btn" class="tab_btn_pushed" href="javascript: show_main_settings()">main settings</a>
	<a id="custom_btn" class="tab_btn" href="javascript: show_custom_field_settings()">custom fields</a>
	<br><br>

<div id="tab1" style="display:block;">
	<table border=0 cellpadding=3>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td class=lbl>Project Name:</td>
	<td><input runat="server" type=text class=txt id="name" maxlength=30 size=30></td>
	<td runat="server" class=err id="name_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Active:</td>
	<td><asp:checkbox runat="server" class=cb id="active"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Default Selection in "projects" Dropdown:</td>
	<td><asp:checkbox runat="server" class=cb id="default_selection"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td class=lbl>Default User:</td>
	<td><asp:DropDownList id="default_user" runat="server">
	</asp:DropDownList></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Auto-Assign New <% Response.Write(Util.get_setting("PluralBugLabel","bug")); %>  to Default User:</td>
	<td><asp:checkbox runat="server" class=cb id="auto_assign"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>
	For the following, see also user page.  Make sure user's email is supplied.<br>
	Also see "NotificationEmailEnabled", "NotificationEmailFrom", "SmtpServer" settings in Web.config.
	<br>
	</span>
	</td>
	</tr>

	<tr>
	<td class=lbl>Auto-Subscribe Default User to Notifications:</td>
	<td><asp:checkbox runat="server" class=cb id="auto_subscribe"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>
	The following are used by btnet_pop3.exe and btnet_service_pop3.exe<br>
	Also see the btnet_service.exe.config file.
	<br>
	</span>
	</td>
	</tr>

	<tr>
	<td class=lbl>Enable Receiving <% Response.Write(Util.get_setting("PluralBugLabel","bug")); %> via POP3 (btnet_service.exe):</td>
	<td><asp:checkbox runat="server" class=cb id="enable_pop3"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Pop3 Username:</td>
	<td><input runat="server" type=text class=txt id="pop3_username" maxlength=50 size=30></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Pop3 Password:</td>
	<td><input runat="server" type=password class=txt id="pop3_password" maxlength=20 size=20></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>
	The following is used as the "From" email address when you respond to <% Response.Write(Util.get_setting("PluralBugLabel","bug")); %> generated by emails
	</span>
	</td>
	</tr>

	<tr>
	<td class=lbl>From Email Address:</td>
	<td><input runat="server" type=text class=txt id="pop3_email_from" maxlength=50 size=30></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td class=lbl>Description:</td>
	<td><textarea runat="server" class=txt id="desc" rows=5 cols=40></textarea></td>
	<td runat="server" class=err id="desc_err">&nbsp;</td>
	</tr>


	</table>
</div>
<div id="tab2" style="display:none;">
	<table border=0 cellpadding=3>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<b class=smallnote style="font-size: 11pt;">Custom fields for this project only</b>
	</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<span class=smallnote>
	Use the following if you want to have a custom field for this project only.
	<br>1. Check the box to enable the field.
	<br>2. Fill in the label.
	<br>3. Create a list of values.  One value per line.
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


	<tr>
	<td colspan=3>
	<hr>
	</td>
	</tr>

	<tr>
	<td class=lbl>Enable Custom Dropdown 1:</td>
	<td><asp:checkbox runat="server" class=cb id="enable_custom_dropdown1"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Label 1:</td>
	<td><input runat="server" type=text class=txt id="custom_dropdown_label1" maxlength=30 size=30></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Values 1:</td>
	<td><textarea cols=40 rows=2 runat="server" type=text class=txt id="custom_dropdown_values1"></textarea></td>
	<td runat="server" class=err id="custom_dropdown_values1_err">&nbsp;</td>
	</tr>

	<tr>
	<td colspan=3>
	<hr>
	</td>
	</tr>

	<tr>
	<td class=lbl>Enable Custom Dropdown 2:</td>
	<td><asp:checkbox runat="server" class=cb id="enable_custom_dropdown2"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Label 2:</td>
	<td><input runat="server" type=text class=txt id="custom_dropdown_label2" maxlength=30 size=30></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Values 2:</td>
	<td><textarea cols=40 rows=2 runat="server" type=text class=txt id="custom_dropdown_values2"></textarea></td>
	<td runat="server" class=err id="custom_dropdown_values2_err">&nbsp;</td>
	</tr>

	<tr>
	<td colspan=3>
	<hr>
	</td>
	</tr>

	<tr>
	<td class=lbl>Enable Custom Dropdown 3:</td>
	<td><asp:checkbox runat="server" class=cb id="enable_custom_dropdown3"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Label 3:</td>
	<td><input runat="server" type=text class=txt id="custom_dropdown_label3" maxlength=30 size=30></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Custom Dropdown Values 3:</td>
	<td><textarea cols=40 rows=2 runat="server" type=text class=txt id="custom_dropdown_values3"></textarea></td>
	<td runat="server" class=err id="custom_dropdown_values3_err">&nbsp;</td>
	</tr>

	</table>
</div>

	<table border="0" width="97%" >

	<tr>
	<td>&nbsp;
	</td>
	</tr>

	<tr>
	<td align="left">
	<span runat="server" class="err" id="msg">&nbsp;
	</span>
	</td>
	</tr>

	<tr>
	<td align="center">
	<input runat="server" class="btn" type="submit" id="sub" value="Create or Edit" />
	</td>
	</tr>


	</table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


