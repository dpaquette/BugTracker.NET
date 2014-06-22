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
Dictionary<string,int> dict_custom_field_permission_level = new Dictionary<string, int>();
DataSet ds_custom;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}

string radio_template = @"
<tr>
	<td>""$name$"" field permission
	<td colspan=2>
		<table id='$name$_field' border='0'>
		<tr>
		<td>
			<span ID='$name$0'><input id='$name$_field_0' type='radio' name='$name$' value='0' $checked0$/><label for='$name$_field_0'>none</label></span>
		</td>

		<td>
			<span ID='$name$1'><input id='$name$_field_1' type='radio' name='$name$' value='1' $checked1$/><label for='$name$_field_1'>view only</label></span>
		</td>
		<td>
			<span ID='$name$2'><input id='$name$_field_2' type='radio' name='$name$' value='2' $checked2$ /><label for='$name$_field_2'>edit</label></span>
		</td>
		</tr>
		</table>
<tr>";


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit organization";

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

	ds_custom = Util.get_custom_columns();

	if (!IsPostBack)
	{

		// add or edit?
		if (id == 0)
		{
			sub.Value = "Create";
			og_active.Checked = true;
			//other_orgs_permission_level.SelectedIndex = 2;
			can_search.Checked = true;
			can_be_assigned_to.Checked = true;
			other_orgs.SelectedValue = "2";

			project_field.SelectedValue = "2";
			org_field.SelectedValue = "2";
			category_field.SelectedValue = "2";
			tags_field.SelectedValue = "2";
			priority_field.SelectedValue = "2";
			status_field.SelectedValue = "2";
			assigned_to_field.SelectedValue = "2";
			udf_field.SelectedValue = "2";
			
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
				dict_custom_field_permission_level[bg_name] = 2;
			}

			
		}
		else
		{
			sub.Value = "Update";

			// Get this entry's data from the db and fill in the form

			sql = @"select *,isnull(og_domain,'') og_domain2 from orgs where og_id = $og_id";
			sql = sql.Replace("$og_id", Convert.ToString(id));
			DataRow dr = btnet.DbUtil.get_datarow(sql);

			// Fill in this form
			og_name.Value = (string) dr["og_name"];
			og_domain.Value = (string) dr["og_domain2"];
			og_active.Checked = Convert.ToBoolean((int)dr["og_active"]);
			non_admins_can_use.Checked = Convert.ToBoolean((int)dr["og_non_admins_can_use"]);
			external_user.Checked = Convert.ToBoolean((int)dr["og_external_user"]);
			can_edit_sql.Checked = Convert.ToBoolean((int)dr["og_can_edit_sql"]);
			can_delete_bug.Checked = Convert.ToBoolean((int)dr["og_can_delete_bug"]);
			can_edit_and_delete_posts.Checked = Convert.ToBoolean((int)dr["og_can_edit_and_delete_posts"]);
			can_merge_bugs.Checked = Convert.ToBoolean((int)dr["og_can_merge_bugs"]);
			can_mass_edit_bugs.Checked = Convert.ToBoolean((int)dr["og_can_mass_edit_bugs"]);
			can_use_reports.Checked = Convert.ToBoolean((int)dr["og_can_use_reports"]);
			can_edit_reports.Checked = Convert.ToBoolean((int)dr["og_can_edit_reports"]);
			can_be_assigned_to.Checked = Convert.ToBoolean((int)dr["og_can_be_assigned_to"]);
			can_view_tasks.Checked = Convert.ToBoolean((int)dr["og_can_view_tasks"]);
			can_edit_tasks.Checked = Convert.ToBoolean((int)dr["og_can_edit_tasks"]);
			can_search.Checked = Convert.ToBoolean((int)dr["og_can_search"]);			
			can_only_see_own_reported.Checked = Convert.ToBoolean((int)dr["og_can_only_see_own_reported"]);	
			can_assign_to_internal_users.Checked = Convert.ToBoolean((int)dr["og_can_assign_to_internal_users"]);

			other_orgs.SelectedValue = Convert.ToString((int)dr["og_other_orgs_permission_level"]);

			project_field.SelectedValue = Convert.ToString((int)dr["og_project_field_permission_level"]);
			org_field.SelectedValue = Convert.ToString((int)dr["og_org_field_permission_level"]);
			category_field.SelectedValue = Convert.ToString((int)dr["og_category_field_permission_level"]);
			tags_field.SelectedValue = Convert.ToString((int)dr["og_tags_field_permission_level"]);
			priority_field.SelectedValue = Convert.ToString((int)dr["og_priority_field_permission_level"]);
			status_field.SelectedValue = Convert.ToString((int)dr["og_status_field_permission_level"]);
			assigned_to_field.SelectedValue = Convert.ToString((int)dr["og_assigned_to_field_permission_level"]);
			udf_field.SelectedValue = Convert.ToString((int)dr["og_udf_field_permission_level"]);
			
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
				object obj = dr["og_" + bg_name + "_field_permission_level"];
				int permission;
				if (Convert.IsDBNull(obj))
				{
					permission = Security.PERMISSION_ALL;
				}
				else
				{
					permission = (int) obj;
				}
				dict_custom_field_permission_level[bg_name] = permission;
			}			

		}
	}
	else
	{
		foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
		{
			string bg_name = (string)dr_custom["name"];
			dict_custom_field_permission_level[bg_name] = Convert.ToInt32(Request[bg_name]);
		}
		
		on_update();
	}
}


///////////////////////////////////////////////////////////////////////
Boolean validate()
{

	Boolean good = true;
	if (og_name.Value == "")
	{
		good = false;
		name_err.InnerText = "Name is required.";
	}
	else
	{
		name_err.InnerText = "";
	}


	return good;
}

///////////////////////////////////////////////////////////////////////
void on_update ()
{

	Boolean good = validate();

	if (good)
	{
		if (id == 0)  // insert new
		{
			sql = @"
insert into orgs
	(og_name,
	og_domain,
	og_active,
	og_non_admins_can_use,
	og_external_user,
	og_can_edit_sql,
	og_can_delete_bug,
	og_can_edit_and_delete_posts,
	og_can_merge_bugs,
	og_can_mass_edit_bugs,
	og_can_use_reports,
	og_can_edit_reports,
	og_can_be_assigned_to,
	og_can_view_tasks,
	og_can_edit_tasks,
	og_can_search,
	og_can_only_see_own_reported,
	og_can_assign_to_internal_users,
	og_other_orgs_permission_level,
	og_project_field_permission_level,
	og_org_field_permission_level,
	og_category_field_permission_level,
	og_tags_field_permission_level,
	og_priority_field_permission_level,
	og_status_field_permission_level,
	og_assigned_to_field_permission_level,
	og_udf_field_permission_level
	$custom1$
	)
	values (
	N'$name', 
	N'$domain',
	$active,
	$non_admins_can_use,
	$external_user,
	$can_edit_sql,
	$can_delete_bug,
	$can_edit_and_delete_posts,
	$can_merge_bugs,
	$can_mass_edit_bugs,
	$can_use_reports,
	$can_edit_reports,
	$can_be_assigned_to,
	$can_view_tasks,
	$can_edit_tasks,
	$can_search,
	$can_only_see_own_reported,
	$can_assign_to_internal_users,
	$other_orgs,
	$flp_project,
	$flp_org,
	$flp_category,
	$flp_tags,
	$flp_priority,
	$flp_status,
	$flp_assigned_to,
	$flp_udf
	$custom2$
)";
		}
		else // edit existing
		{

			sql = @"
update orgs set
	og_name = N'$name',
	og_domain = N'$domain',
	og_active = $active,
	og_non_admins_can_use = $non_admins_can_use,
	og_external_user = $external_user,
	og_can_edit_sql = $can_edit_sql,
	og_can_delete_bug = $can_delete_bug,
	og_can_edit_and_delete_posts = $can_edit_and_delete_posts,
	og_can_merge_bugs = $can_merge_bugs,
	og_can_mass_edit_bugs = $can_mass_edit_bugs,
	og_can_use_reports = $can_use_reports,
	og_can_edit_reports = $can_edit_reports,
	og_can_be_assigned_to = $can_be_assigned_to,
	og_can_view_tasks = $can_view_tasks,
	og_can_edit_tasks = $can_edit_tasks,
	og_can_search = $can_search,
	og_can_only_see_own_reported = $can_only_see_own_reported,
	og_can_assign_to_internal_users = $can_assign_to_internal_users,
	og_other_orgs_permission_level = $other_orgs,
	og_project_field_permission_level = $flp_project,
	og_org_field_permission_level = $flp_org,
	og_category_field_permission_level = $flp_category,
	og_tags_field_permission_level = $flp_tags,
	og_priority_field_permission_level = $flp_priority,
	og_status_field_permission_level = $flp_status,
	og_assigned_to_field_permission_level = $flp_assigned_to,
	og_udf_field_permission_level = $flp_udf
	$custom3$
	where og_id = $og_id";

			sql = sql.Replace("$og_id", Convert.ToString(id));

		}

		sql = sql.Replace("$name", og_name.Value.Replace("'","''"));
		sql = sql.Replace("$domain", og_domain.Value.Replace("'","''"));
		sql = sql.Replace("$active", Util.bool_to_string(og_active.Checked));
		sql = sql.Replace("$non_admins_can_use", Util.bool_to_string(non_admins_can_use.Checked));
		sql = sql.Replace("$external_user", Util.bool_to_string(external_user.Checked));
		sql = sql.Replace("$can_edit_sql", Util.bool_to_string(can_edit_sql.Checked));
		sql = sql.Replace("$can_delete_bug", Util.bool_to_string(can_delete_bug.Checked));
		sql = sql.Replace("$can_edit_and_delete_posts", Util.bool_to_string(can_edit_and_delete_posts.Checked));
		sql = sql.Replace("$can_merge_bugs", Util.bool_to_string(can_merge_bugs.Checked));
		sql = sql.Replace("$can_mass_edit_bugs", Util.bool_to_string(can_mass_edit_bugs.Checked));
		sql = sql.Replace("$can_use_reports", Util.bool_to_string(can_use_reports.Checked));
		sql = sql.Replace("$can_edit_reports", Util.bool_to_string(can_edit_reports.Checked));
		sql = sql.Replace("$can_be_assigned_to", Util.bool_to_string(can_be_assigned_to.Checked));
		sql = sql.Replace("$can_view_tasks", Util.bool_to_string(can_view_tasks.Checked));
		sql = sql.Replace("$can_edit_tasks", Util.bool_to_string(can_edit_tasks.Checked));
		sql = sql.Replace("$can_search", Util.bool_to_string(can_search.Checked));
		sql = sql.Replace("$can_only_see_own_reported", Util.bool_to_string(can_only_see_own_reported.Checked));
		sql = sql.Replace("$can_assign_to_internal_users", Util.bool_to_string(can_assign_to_internal_users.Checked));
		sql = sql.Replace("$other_orgs", other_orgs.SelectedValue);
		sql = sql.Replace("$flp_project", project_field.SelectedValue);
		sql = sql.Replace("$flp_org", org_field.SelectedValue);
		sql = sql.Replace("$flp_category", category_field.SelectedValue);
		sql = sql.Replace("$flp_tags", tags_field.SelectedValue);
		sql = sql.Replace("$flp_priority", priority_field.SelectedValue);
		sql = sql.Replace("$flp_status", status_field.SelectedValue);
		sql = sql.Replace("$flp_assigned_to", assigned_to_field.SelectedValue);
		sql = sql.Replace("$flp_udf", udf_field.SelectedValue);

		if (id == 0)  // insert new
		{
			string custom1 = "";
			string custom2 = "";
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
				string og_col_name = "og_" 
					+ bg_name
					+ "_field_permission_level";

                custom1 += ",[" + og_col_name + "]";
				custom2 += "," + btnet.Util.sanitize_integer(Request[bg_name]);

			}
			sql = sql.Replace("$custom1$",custom1);
			sql = sql.Replace("$custom2$",custom2);
		}
		else
		{
			string custom3 = "";
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
                string og_col_name = "og_" 
					+ bg_name
					+ "_field_permission_level";

                custom3 += ",[" + og_col_name + "]=" + btnet.Util.sanitize_integer(Request[bg_name]);

			}
			sql = sql.Replace("$custom3$",custom3);
		}

		btnet.DbUtil.execute_nonquery(sql);
		Server.Transfer ("orgs.aspx");

	}
	else
	{
		if (id == 0)  // insert new
		{
			msg.InnerText = "Organization was not created.";
		}
		else // edit existing
		{
			msg.InnerText = "Organization was not updated.";
		}

	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit org</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>


<div class=align><table border=0><tr><td>
<a href=orgs.aspx>back to organizations</a>
<form class=frm runat="server">
	<table border=0>

	<tr>
	<td class=lbl>Organization Name:</td>
	<td><input runat="server" type=text class=txt id="og_name" maxlength=30 size=30></td>
	<td runat="server" class=err id="name_err">&nbsp;</td>
	</tr>
	
	<tr>
	<td class=lbl>Domain (like, "example.com"):
	<td><input runat="server" type=text class=txt id="og_domain" maxlength=80 size=30></td>
	<td runat="server" class=err id="domain_err">&nbsp;</td>
	</tr>
	
	<td class=lbl>Active:</td>
	<td><asp:checkbox runat="server" class=cb id="og_active"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	<br><br>
	<div class=smallnote style="width: 400px;">Can members of this organization view/edit bugs associated with other organizations?<br>
	</div>
	</td>
	</tr>

	<tr>
		<td class=lbl colspan=3>Permission level for bugs associated with other (or no) organizations<br>
		<asp:RadioButtonList RepeatDirection="Horizontal" id="other_orgs" runat="server">
			<asp:ListItem text="none"      value="0" ID="other_orgs0" runat="server"/>
			<asp:ListItem text="view only" value="1" ID="other_orgs1" runat="server"/>
			<asp:ListItem text="edit"      value="2" ID="other_orgs2" runat="server"/>
		</asp:RadioButtonList>


	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>
	
    </table>
	<table border=0>

		
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_search"/>
		<td class=lbl>Can search</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="external_user"/></td>
		<td class=lbl>External users&nbsp;&nbsp; <span class=smallnote>(External users cannot view posts marked "Visible for internal users only")</span></td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_only_see_own_reported"/></td>
		<td class=lbl>Can see only own reported</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_be_assigned_to"/></td>
		<td class=lbl>Members of this org appear in "assigned to" dropdown in edit bug page</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="non_admins_can_use"/></td>
		<td class=lbl>Non-admin with permission to add users can add users to this org</td>
		<td>&nbsp</td>
		</tr>

	</table>

	<table border=0>


		<tr>
		<td colspan=3>
		<br><br>
		<div class=smallnote style="width: 400px;">Field level permissions<br>
		</div>
		</td>
		</tr>


		<tr>
		<td colspan=3>
		&nbsp;
		</td>
		</tr>

		<tr>
		<td>"Project" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="project_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="project0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="project1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="project2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Organization" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="org_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="org0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="org1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="org2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Category" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="category_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="category0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="category1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="category2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Priority" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="priority_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="priority0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="priority1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="priority2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Status" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="status_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="status0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="status1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="status2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Assigned To" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="assigned_to_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="assigned_to0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="assigned_to1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="assigned_to2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>User Defined Attribute field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="udf_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="udf0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="udf1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="udf2" runat="server"/>
			</asp:RadioButtonList>

		<tr>
		<td>"Tags" field permission
		<td colspan=2>
			<asp:RadioButtonList RepeatDirection="Horizontal" id="tags_field" runat="server">
				<asp:ListItem text="none"      value="0" ID="tags0" runat="server"/>
				<asp:ListItem text="view only" value="1" ID="tags1" runat="server"/>
				<asp:ListItem text="edit"      value="2" ID="tags2" runat="server"/>
			</asp:RadioButtonList>

<%
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
				string og_name = "og_" 
					+ bg_name
					+ "_field_permission_level";


                string radio = radio_template;
                int selected_val = dict_custom_field_permission_level[bg_name];
                radio = radio.Replace("$name$", bg_name);
                radio = radio.Replace("$checked0$", selected_val == 0 ? "checked=true" : "");
                radio = radio.Replace("$checked1$", selected_val == 1 ? "checked=true" : "");
                radio = radio.Replace("$checked2$", selected_val == 2 ? "checked=true" : "");
				Response.Write(radio);
                				
			}
%>

	</table>
	<table border=0>

		<tr>
		<td colspan=3>
		<br><br>
		<div class=smallnote style="width: 400px;">Use the following settings to control permissions for non-admins.<br>Admins have all permissions regardless of these settings.<br>
		</div>
		</td>
		</tr>


		<tr>
		<td colspan=3>
		&nbsp;
		</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_edit_sql"/></td>
		<td class=lbl>Can edit sql and create/edit queries for everybody</td>
		<td>&nbsp</td>
		</tr>
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_delete_bug"/></td>
		<td class=lbl>Can delete bugs</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_edit_and_delete_posts"/></td>
		<td class=lbl>Can edit and delete comments and attachments</td>
		<td>&nbsp</td>
		</tr>
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_merge_bugs"/></td>
		<td class=lbl>Can merge two bugs into one</td>
		<td>&nbsp</td>
		</tr>
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_mass_edit_bugs"/></td>
		<td class=lbl>Can mass edit bugs on search page</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_use_reports"/></td>
		<td class=lbl>Can use reports</td>
		<td>&nbsp</td>
		</tr>
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_edit_reports"/></td>
		<td class=lbl>Can create/edit reports</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_view_tasks"/></td>
		<td class=lbl>Can view tasks/time</td>
		<td>&nbsp</td>
		</tr>
		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_edit_tasks"/></td>
		<td class=lbl>Can edit tasks/time</td>
		<td>&nbsp</td>
		</tr>

		<tr>
		<td><asp:checkbox runat="server" class=cb id="can_assign_to_internal_users"/></td>
		<td class=lbl>Can assign to internal users (even if external org)</td>
		<td>&nbsp</td>
		</tr>
		

		
	</table>

	<table border=0>


		<tr><td colspan=2 align=left>
		<span runat="server" class=err id="msg">&nbsp;</span>
		</td></tr>

		<tr>
		<td colspan=2 align=center>
		<input runat="server" class=btn type=submit id="sub" value="Create or Edit">
		<td>&nbsp</td>
		</td>
		</tr>
		</td></tr>

	</table>

</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


