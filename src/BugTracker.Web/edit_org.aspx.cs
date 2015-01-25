using System;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_org : BasePage
    {

        int id;
        SQLString sql;


        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

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
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);
            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
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


                }
                else
                {
                    sub.Value = "Update";

                    // Get this entry's data from the db and fill in the form

                    sql = new SQLString(@"select *,isnull(og_domain,'') og_domain2 from orgs where og_id = @og_id");
                    sql = sql.AddParameterWithValue("og_id", Convert.ToString(id));
                    DataRow dr = btnet.DbUtil.get_datarow(sql);

                    // Fill in this form
                    og_name.Value = (string)dr["og_name"];
                    og_domain.Value = (string)dr["og_domain2"];
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
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {
                if (id == 0)  // insert new
                {
                    sql = new SQLString(@"
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
	)
	values (
	@name, 
	@domain,
	@active,
	@non_admins_can_use,
	@external_user,
	@can_edit_sql,
	@can_delete_bug,
	@can_edit_and_delete_posts,
	@can_merge_bugs,
	@can_mass_edit_bugs,
	@can_use_reports,
	@can_edit_reports,
	@can_be_assigned_to,
	@can_view_tasks,
	@can_edit_tasks,
	@can_search,
	@can_only_see_own_reported,
	@can_assign_to_internal_users,
	@other_orgs,
	@flp_project,
	@flp_org,
	@flp_category,
	@flp_tags,
	@flp_priority,
	@flp_status,
	@flp_assigned_to,
	@flp_udf
)");
                }
                else // edit existing
                {

                    sql = new SQLString(@"
update orgs set
	og_name = @name,
	og_domain = @domain,
	og_active = @active,
	og_non_admins_can_use = @non_admins_can_use,
	og_external_user = @external_user,
	og_can_edit_sql = @can_edit_sql,
	og_can_delete_bug = @can_delete_bug,
	og_can_edit_and_delete_posts = @can_edit_and_delete_posts,
	og_can_merge_bugs = @can_merge_bugs,
	og_can_mass_edit_bugs = @can_mass_edit_bugs,
	og_can_use_reports = @can_use_reports,
	og_can_edit_reports = @can_edit_reports,
	og_can_be_assigned_to = @can_be_assigned_to,
	og_can_view_tasks = @can_view_tasks,
	og_can_edit_tasks = @can_edit_tasks,
	og_can_search = @can_search,
	og_can_only_see_own_reported = @can_only_see_own_reported,
	og_can_assign_to_internal_users = @can_assign_to_internal_users,
	og_other_orgs_permission_level = @other_orgs,
	og_project_field_permission_level = @flp_project,
	og_org_field_permission_level = @flp_org,
	og_category_field_permission_level = @flp_category,
	og_tags_field_permission_level = @flp_tags,
	og_priority_field_permission_level = @flp_priority,
	og_status_field_permission_level = @flp_status,
	og_assigned_to_field_permission_level = @flp_assigned_to,
	og_udf_field_permission_level = @flp_udf
	where og_id = @og_id");

                    sql = sql.AddParameterWithValue("og_id", Convert.ToString(id));

                }

                sql = sql.AddParameterWithValue("name", og_name.Value);
                sql = sql.AddParameterWithValue("domain", og_domain.Value);
                sql = sql.AddParameterWithValue("active", Util.bool_to_string(og_active.Checked));
                sql = sql.AddParameterWithValue("non_admins_can_use", Util.bool_to_string(non_admins_can_use.Checked));
                sql = sql.AddParameterWithValue("external_user", Util.bool_to_string(external_user.Checked));
                sql = sql.AddParameterWithValue("can_edit_sql", Util.bool_to_string(can_edit_sql.Checked));
                sql = sql.AddParameterWithValue("can_delete_bug", Util.bool_to_string(can_delete_bug.Checked));
                sql = sql.AddParameterWithValue("can_edit_and_delete_posts", Util.bool_to_string(can_edit_and_delete_posts.Checked));
                sql = sql.AddParameterWithValue("can_merge_bugs", Util.bool_to_string(can_merge_bugs.Checked));
                sql = sql.AddParameterWithValue("can_mass_edit_bugs", Util.bool_to_string(can_mass_edit_bugs.Checked));
                sql = sql.AddParameterWithValue("can_use_reports", Util.bool_to_string(can_use_reports.Checked));
                sql = sql.AddParameterWithValue("can_edit_reports", Util.bool_to_string(can_edit_reports.Checked));
                sql = sql.AddParameterWithValue("can_be_assigned_to", Util.bool_to_string(can_be_assigned_to.Checked));
                sql = sql.AddParameterWithValue("can_view_tasks", Util.bool_to_string(can_view_tasks.Checked));
                sql = sql.AddParameterWithValue("can_edit_tasks", Util.bool_to_string(can_edit_tasks.Checked));
                sql = sql.AddParameterWithValue("can_search", Util.bool_to_string(can_search.Checked));
                sql = sql.AddParameterWithValue("can_only_see_own_reported", Util.bool_to_string(can_only_see_own_reported.Checked));
                sql = sql.AddParameterWithValue("can_assign_to_internal_users", Util.bool_to_string(can_assign_to_internal_users.Checked));
                sql = sql.AddParameterWithValue("other_orgs", other_orgs.SelectedValue);
                sql = sql.AddParameterWithValue("flp_project", project_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_org", org_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_category", category_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_tags", tags_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_priority", priority_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_status", status_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_assigned_to", assigned_to_field.SelectedValue);
                sql = sql.AddParameterWithValue("flp_udf", udf_field.SelectedValue);


                btnet.DbUtil.execute_nonquery(sql);
                Server.Transfer("orgs.aspx");

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

    }
}
