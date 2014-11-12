using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace btnet
{
    public partial class search : BasePage
    {


        protected Security security;
        protected bool show_udf;
        protected bool use_full_names = false;

        protected DataTable dt_users = null;

        protected string project_dropdown_select_cols = "";

        ///////////////////////////////////////////////////////////////////////
        class ProjectDropdown
        {
            public bool enabled = false;
            public string label = "";
            public string values = "";
        };

        class BtnetProject
        {
            public Dictionary<int, ProjectDropdown> map_dropdowns = new Dictionary<int, ProjectDropdown>();
        };

        Dictionary<int, BtnetProject> map_projects = new Dictionary<int, BtnetProject>();

        ///////////////////////////////////////////////////////////////////////
        public void Page_Load(Object sender, EventArgs e)
        {
            Util.do_not_cache(Response);

            security = new Security();
            security.check_security(HttpContext.Current, Security.ANY_USER_OK);

            if (security.user.is_admin || security.user.can_search)
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            titl.InnerText = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "search";

            show_udf = (Util.get_setting("ShowUserDefinedBugAttribute", "1") == "1");
            use_full_names = (Util.get_setting("UseFullNames", "0") == "1");

            ds_custom_cols = Util.get_custom_columns();

            dt_users = Util.get_related_users(security, false);

            if (!IsPostBack)
            {
                load_drop_downs();
                project_custom_dropdown1_label.Style["display"] = "none";
                project_custom_dropdown1.Style["display"] = "none";

                project_custom_dropdown2_label.Style["display"] = "none";
                project_custom_dropdown2.Style["display"] = "none";

                project_custom_dropdown3_label.Style["display"] = "none";
                project_custom_dropdown3.Style["display"] = "none";

                // are there any project dropdowns?

                var sql = new SQLString(@"
select count(1)
from projects
where isnull(pj_enable_custom_dropdown1,0) = 1
or isnull(pj_enable_custom_dropdown2,0) = 1
or isnull(pj_enable_custom_dropdown3,0) = 1");

                int projects_with_custom_dropdowns = (int)btnet.DbUtil.execute_scalar(sql);

                if (projects_with_custom_dropdowns == 0)
                {
                    project.AutoPostBack = false;
                }

            }
            else
            {

                // get the project dropdowns

                var sql = new SQLString(@"
select
pj_id,
isnull(pj_enable_custom_dropdown1,0) pj_enable_custom_dropdown1,
isnull(pj_enable_custom_dropdown2,0) pj_enable_custom_dropdown2,
isnull(pj_enable_custom_dropdown3,0) pj_enable_custom_dropdown3,
isnull(pj_custom_dropdown_label1,'') pj_custom_dropdown_label1,
isnull(pj_custom_dropdown_label2,'') pj_custom_dropdown_label2,
isnull(pj_custom_dropdown_label3,'') pj_custom_dropdown_label3,
isnull(pj_custom_dropdown_values1,'') pj_custom_dropdown_values1,
isnull(pj_custom_dropdown_values2,'') pj_custom_dropdown_values2,
isnull(pj_custom_dropdown_values3,'') pj_custom_dropdown_values3
from projects
where isnull(pj_enable_custom_dropdown1,0) = 1
or isnull(pj_enable_custom_dropdown2,0) = 1
or isnull(pj_enable_custom_dropdown3,0) = 1");

                DataSet ds_projects = btnet.DbUtil.get_dataset(sql);

                foreach (DataRow dr in ds_projects.Tables[0].Rows)
                {
                    BtnetProject btnet_project = new BtnetProject();

                    ProjectDropdown dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown1"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label1"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values1"];
                    btnet_project.map_dropdowns[1] = dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown2"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label2"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values2"];
                    btnet_project.map_dropdowns[2] = dropdown;

                    dropdown = new ProjectDropdown();
                    dropdown.enabled = Convert.ToBoolean((int)dr["pj_enable_custom_dropdown3"]);
                    dropdown.label = (string)dr["pj_custom_dropdown_label3"];
                    dropdown.values = (string)dr["pj_custom_dropdown_values3"];
                    btnet_project.map_dropdowns[3] = dropdown;

                    map_projects[(int)dr["pj_id"]] = btnet_project;

                }

                // which button did the user hit?

                if (project_changed.Value == "1" && project.AutoPostBack == true)
                {
                    handle_project_custom_dropdowns();
                }
                else if (hit_submit_button.Value == "1")
                {
                    handle_project_custom_dropdowns();
                    do_query();
                }
                else
                {
                    dv = (DataView)Session["bugs"];
                    if (dv == null)
                    {
                        do_query();
                    }
                    call_sort_and_filter_buglist_dataview();
                }
            }

            hit_submit_button.Value = "0";
            project_changed.Value = "0";

            if (security.user.is_admin || security.user.can_edit_sql)
            {

            }
            else
            {
                visible_sql_label.Style["display"] = "none";
                visible_sql_text.Style["display"] = "none";
            }

        }



        ///////////////////////////////////////////////////////////////////////
        string build_where(string where, string clause)
        {
            if (clause == "")
            {
                return where;
            }

            string sql = "";

            if (where == "")
            {
                sql = " where ";
                sql += clause;
            }
            else
            {
                sql = where;
                string and_or = and.Checked ? "and " : "or ";
                sql += and_or;
                sql += clause;
            }

            return sql;
        }


        ///////////////////////////////////////////////////////////////////////
        string build_clause_from_listbox(ListBox lb, string column_name)
        {

            string clause = "";
            foreach (ListItem li in lb.Items)
            {
                if (li.Selected)
                {
                    if (clause == "")
                    {
                        clause += column_name + " in (";
                    }
                    else
                    {
                        clause += ",";
                    }

                    clause += li.Value;

                }
            }

            if (clause != "")
            {
                clause += ") ";
            }

            return clause;

        }


        ///////////////////////////////////////////////////////////////////////
        string format_in_not_in(string s)
        {

            string vals = "(";
            string opts = "";

            string[] s2 = Util.split_string_using_commas(s);
            for (int i = 0; i < s2.Length; i++)
            {
                if (opts != "")
                {
                    opts += ",";
                }

                string one_opt = "N'";
                one_opt += s2[i].Replace("'", "''");
                one_opt += "'";

                opts += one_opt;
            }
            vals += opts;
            vals += ")";

            return vals;

        }


        ///////////////////////////////////////////////////////////////////////
        List<ListItem> get_selected_projects()
        {
            List<ListItem> selected_projects = new List<ListItem>();

            foreach (ListItem li in project.Items)
            {
                if (li.Selected)
                {
                    selected_projects.Add(li);
                }
            }

            return selected_projects;
        }


        ///////////////////////////////////////////////////////////////////////
        void do_query()
        {
            prev_sort.Value = "-1";
            prev_dir.Value = "ASC";
            new_page.Value = "0";

            // Create "WHERE" clause

            string where = "";

            string reported_by_clause = build_clause_from_listbox(reported_by, "bg_reported_user");
            string assigned_to_clause = build_clause_from_listbox(assigned_to, "bg_assigned_to_user");
            string project_clause = build_clause_from_listbox(project, "bg_project");

            string project_custom_dropdown1_clause
                = build_clause_from_listbox(project_custom_dropdown1, "bg_project_custom_dropdown_value1");
            string project_custom_dropdown2_clause
                = build_clause_from_listbox(project_custom_dropdown2, "bg_project_custom_dropdown_value2");
            string project_custom_dropdown3_clause
                = build_clause_from_listbox(project_custom_dropdown3, "bg_project_custom_dropdown_value3");

            string org_clause = build_clause_from_listbox(org, "bg_org");
            string category_clause = build_clause_from_listbox(category, "bg_category");
            string priority_clause = build_clause_from_listbox(priority, "bg_priority");
            string status_clause = build_clause_from_listbox(status, "bg_status");
            string udf_clause = "";

            if (show_udf)
            {
                udf_clause = build_clause_from_listbox(udf, "bg_user_defined_attribute");
            }


            // SQL "LIKE" uses [, %, and _ in a special way

            string like_string = like.Value.Replace("'", "''");
            like_string = like_string.Replace("[", "[[]");
            like_string = like_string.Replace("%", "[%]");
            like_string = like_string.Replace("_", "[_]");

            string like2_string = like2.Value.Replace("'", "''");
            like2_string = like2_string.Replace("[", "[[]");
            like2_string = like2_string.Replace("%", "[%]");
            like2_string = like2_string.Replace("_", "[_]");

            string desc_clause = "";
            if (like.Value != "")
            {
                desc_clause = " bg_short_desc like";
                desc_clause += " N'%" + like_string + "%'\n";
            }

            string comments_clause = "";
            if (like2.Value != "")
            {
                comments_clause = " bg_id in (select bp_bug from bug_posts where bp_type in ('comment','received','sent') and isnull(bp_comment_search,bp_comment) like";
                comments_clause += " N'%" + like2_string + "%'";
                if (security.user.external_user)
                {
                    comments_clause += " and bp_hidden_from_external_users = 0";
                }
                comments_clause += ")\n";
            }


            string comments_since_clause = "";
            if (comments_since.Value != "")
            {
                comments_since_clause = " bg_id in (select bp_bug from bug_posts where bp_type in ('comment','received','sent') and bp_date > '";
                comments_since_clause += format_to_date(comments_since.Value);
                comments_since_clause += "')\n";
            }

            string from_clause = "";
            if (from_date.Value != "")
            {
                from_clause = " bg_reported_date >= '" + format_from_date(from_date.Value) + "'\n";
            }

            string to_clause = "";
            if (to_date.Value != "")
            {
                to_clause = " bg_reported_date <= '" + format_to_date(to_date.Value) + "'\n";
            }

            string lu_from_clause = "";
            if (lu_from_date.Value != "")
            {
                lu_from_clause = " bg_last_updated_date >= '" + format_from_date(lu_from_date.Value) + "'\n";
            }

            string lu_to_clause = "";
            if (lu_to_date.Value != "")
            {
                lu_to_clause = " bg_last_updated_date <= '" + format_to_date(lu_to_date.Value) + "'\n";
            }


            where = build_where(where, reported_by_clause);
            where = build_where(where, assigned_to_clause);
            where = build_where(where, project_clause);
            where = build_where(where, project_custom_dropdown1_clause);
            where = build_where(where, project_custom_dropdown2_clause);
            where = build_where(where, project_custom_dropdown3_clause);
            where = build_where(where, org_clause);
            where = build_where(where, category_clause);
            where = build_where(where, priority_clause);
            where = build_where(where, status_clause);
            where = build_where(where, desc_clause);
            where = build_where(where, comments_clause);
            where = build_where(where, comments_since_clause);
            where = build_where(where, from_clause);
            where = build_where(where, to_clause);
            where = build_where(where, lu_from_clause);
            where = build_where(where, lu_to_clause);

            if (show_udf)
            {
                where = build_where(where, udf_clause);
            }

            foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
            {
                string column_name = (string)drcc["name"];
                if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                {
                    continue;
                }

                string values = Request[column_name];

                if (values != null)
                {

                    values = values.Replace("'", "''");

                    string custom_clause = "";

                    string datatype = (string)drcc["datatype"];

                    if ((datatype == "varchar" || datatype == "nvarchar" || datatype == "char" || datatype == "nchar")
                    && (string)drcc["dropdown type"] == "")
                    {
                        if (values != "")
                        {
                            custom_clause = " [" + column_name + "] like '%" + values + "%'\n";
                            where = build_where(where, custom_clause);
                        }
                    }
                    else if (datatype == "datetime")
                    {
                        if (values != "")
                        {
                            custom_clause = " [" + column_name + "] >= '" + format_from_date(values) + "'\n";
                            where = build_where(where, custom_clause);

                            // reset, and do the to date
                            custom_clause = "";
                            values = Request["to__" + column_name];
                            if (values != "")
                            {
                                custom_clause = " [" + column_name + "] <= '" + format_to_date(values) + "'\n";
                                where = build_where(where, custom_clause);
                            }
                        }
                    }
                    else
                    {
                        if (values == "" && (datatype == "int" || datatype == "decimal"))
                        {
                            // skip
                        }
                        else
                        {
                            string in_not_in = format_in_not_in(values);
                            custom_clause = " [" + column_name + "] in " + in_not_in + "\n";
                            where = build_where(where, custom_clause);
                        }
                    }
                }
            }

            // The rest of the SQL is either built in or comes from Web.config

            string search_sql = Util.get_setting("SearchSQL", "");

            if (search_sql == "")
            {

                /*
                select isnull(pr_background_color,'#ffffff') [color], bg_id [id],
                bg_short_desc [desc],
                bg_reported_date [reported on],
                isnull(rpt.us_username,'') [reported by],
                isnull(pj_name,'') [project],
                isnull(og_name,'') [organization],
                isnull(ct_name,'') [category],
                isnull(pr_name,'') [priority],
                isnull(asg.us_username,'') [assigned to],
                isnull(st_name,'') [status],
                isnull(udf_name,'') [MyUDF],
                isnull([mycust],'') [mycust],
                isnull([mycust2],'') [mycust2]
                from bugs
                left outer join users rpt on rpt.us_id = bg_reported_user
                left outer join users asg on asg.us_id = bg_assigned_to_user
                left outer join projects on pj_id = bg_project
                left outer join orgs on og_id = bg_org
                left outer join categories on ct_id = bg_category
                left outer join priorities on pr_id = bg_priority
                left outer join statuses on st_id = bg_status
                left outer join user_defined_attribute on udf_id = bg_user_defined_attribute
                order by bg_id desc
                */

                string select = "select isnull(pr_background_color,'#ffffff') [color], bg_id [id],\nbg_short_desc [desc]";

                // reported
                if (use_full_names)
                {
                    select += "\n,isnull(rpt.us_lastname + ', ' + rpt.us_firstname,'') [reported by]";
                }
                else
                {
                    select += "\n,isnull(rpt.us_username,'') [reported by]";
                }
                select += "\n,bg_reported_date [reported on]";

                // last updated
                if (use_full_names)
                {
                    select += "\n,isnull(lu.us_lastname + ', ' + lu.us_firstname,'') [last updated by]";
                }
                else
                {
                    select += "\n,isnull(lu.us_username,'') [last updated by]";
                }
                select += "\n,bg_last_updated_date [last updated on]";


                if (security.user.tags_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(bg_tags,'') [tags]";
                }

                if (security.user.project_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(pj_name,'') [project]";
                }

                if (security.user.org_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(og_name,'') [organization]";
                }

                if (security.user.category_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(ct_name,'') [category]";
                }

                if (security.user.priority_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(pr_name,'') [priority]";
                }

                if (security.user.assigned_to_field_permission_level != Security.PERMISSION_NONE)
                {
                    if (use_full_names)
                    {
                        select += ",\nisnull(asg.us_lastname + ', ' + asg.us_firstname,'') [assigned to]";
                    }
                    else
                    {
                        select += ",\nisnull(asg.us_username,'') [assigned to]";
                    }
                }

                if (security.user.status_field_permission_level != Security.PERMISSION_NONE)
                {
                    select += ",\nisnull(st_name,'') [status]";
                }

                if (security.user.udf_field_permission_level != Security.PERMISSION_NONE)
                {
                    if (show_udf)
                    {
                        string udf_name = Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE");
                        select += ",\nisnull(udf_name,'') [" + udf_name + "]";
                    }
                }

                // let results include custom columns
                string custom_cols_sql = "";
                int user_type_cnt = 1;
                foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
                {
                    string column_name = (string)drcc["name"];
                    if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                    {
                        continue;
                    }

                    if (Convert.ToString(drcc["dropdown type"]) == "users")
                    {
                        custom_cols_sql += ",\nisnull(users"
                            + Convert.ToString(user_type_cnt++)
                            + ".us_username,'') "
                            + "["
                            + column_name + "]";
                    }
                    else
                    {
                        if (Convert.ToString(drcc["datatype"]) == "decimal")
                        {
                            custom_cols_sql += ",\nisnull(["
                                + column_name
                                + "],0)["
                                + column_name + "]";
                        }
                        else
                        {
                            custom_cols_sql += ",\nisnull(["
                                + column_name
                                + "],'')["
                                + column_name + "]";
                        }
                    }
                }

                select += custom_cols_sql;

                // Handle project custom dropdowns
                List<ListItem> selected_projects = get_selected_projects();

                string project_dropdown_select_cols_server_side = "";

                string alias1 = null;
                string alias2 = null;
                string alias3 = null;

                foreach (ListItem selected_project in selected_projects)
                {
                    if (selected_project.Value == "0")
                        continue;

                    int pj_id = Convert.ToInt32(selected_project.Value);

                    if (map_projects.ContainsKey(pj_id))
                    {

                        BtnetProject btnet_project = map_projects[pj_id];

                        if (btnet_project.map_dropdowns[1].enabled)
                        {
                            if (alias1 == null)
                            {
                                alias1 = btnet_project.map_dropdowns[1].label;
                            }
                            else
                            {
                                alias1 = "dropdown1";
                            }
                        }

                        if (btnet_project.map_dropdowns[2].enabled)
                        {
                            if (alias2 == null)
                            {
                                alias2 = btnet_project.map_dropdowns[2].label;
                            }
                            else
                            {
                                alias2 = "dropdown2";
                            }
                        }

                        if (btnet_project.map_dropdowns[3].enabled)
                        {
                            if (alias3 == null)
                            {
                                alias3 = btnet_project.map_dropdowns[3].label;
                            }
                            else
                            {
                                alias3 = "dropdown3";
                            }
                        }


                    }
                }

                if (alias1 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value1,'') [" + alias1 + "]";
                }
                if (alias2 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value2,'') [" + alias2 + "]";
                }
                if (alias3 != null)
                {
                    project_dropdown_select_cols_server_side
                        += ",\nisnull(bg_project_custom_dropdown_value3,'') [" + alias3 + "]";
                }

                select += project_dropdown_select_cols_server_side;

                select += @" from bugs
			left outer join users rpt on rpt.us_id = bg_reported_user
			left outer join users lu on lu.us_id = bg_last_updated_user
			left outer join users asg on asg.us_id = bg_assigned_to_user
			left outer join projects on pj_id = bg_project
			left outer join orgs on og_id = bg_org
			left outer join categories on ct_id = bg_category
			left outer join priorities on pr_id = bg_priority
			left outer join statuses on st_id = bg_status
			";

                user_type_cnt = 1;
                foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
                {

                    string column_name = (string)drcc["name"];
                    if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                    {
                        continue;
                    }

                    if (Convert.ToString(drcc["dropdown type"]) == "users")
                    {
                        select += "left outer join users users"
                            + Convert.ToString(user_type_cnt)
                            + " on users"
                            + Convert.ToString(user_type_cnt)
                            + ".us_id = bugs."
                            + "[" + column_name + "]\n";

                        user_type_cnt++;

                    }
                }


                if (show_udf)
                {
                    select += "left outer join user_defined_attribute on udf_id = bg_user_defined_attribute";
                }

                sql = new SQLString(select + where + " order by bg_id desc");

            }
            else
            {
                search_sql = search_sql.Replace("[br]", "\n");
                sql = new SQLString(search_sql.Replace("$WHERE$", where));
            }

            sql = Util.alter_sql_per_project_permissions(sql, security);

            DataSet ds = btnet.DbUtil.get_dataset(sql);
            dv = new DataView(ds.Tables[0]);
            Session["bugs"] = dv;
            Session["bugs_unfiltered"] = ds.Tables[0];
        }


        string format_from_date(string dt)
        {
            return Util.format_local_date_into_db_format(dt).Replace(" 12:00:00", "").Replace(" 00:00:00", "");
        }

        string format_to_date(string dt)
        {
            return Util.format_local_date_into_db_format(dt).Replace(" 12:00:00", " 23:59:59").Replace(" 00:00:00", " 23:59:59");
        }

        ///////////////////////////////////////////////////////////////////////
        void load_project_custom_dropdown(ListBox dropdown, string vals_string, Dictionary<String, String> duplicate_detection_dictionary)
        {
            string[] vals_array = btnet.Util.split_dropdown_vals(vals_string);
            for (int i = 0; i < vals_array.Length; i++)
            {
                if (!duplicate_detection_dictionary.ContainsKey(vals_array[i]))
                {
                    dropdown.Items.Add(new ListItem(vals_array[i], "'" + vals_array[i].Replace("'", "''") + "'"));
                    duplicate_detection_dictionary.Add(vals_array[i], vals_array[i]);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void handle_project_custom_dropdowns()
        {

            // How many projects selected?
            List<ListItem> selected_projects = get_selected_projects();
            Dictionary<String, String>[] dupe_detection_dictionaries = new Dictionary<String, String>[3];
            Dictionary<String, String>[] previous_selection_dictionaries = new Dictionary<String, String>[3];
            for (int i = 0; i < dupe_detection_dictionaries.Length; i++)
            {
                // Initialize Dictionary to accumulate ListItem values as they are added to the ListBox
                // so that duplicate values from multiple projects are not added to the ListBox twice.
                dupe_detection_dictionaries[i] = new Dictionary<String, String>();

                previous_selection_dictionaries[i] = new Dictionary<String, String>();
            }

            // Preserve user's previous selections (necessary if this is called during a postback).
            foreach (ListItem li in project_custom_dropdown1.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[0].Add(li.Value, li.Value);
                }
            }
            foreach (ListItem li in project_custom_dropdown2.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[1].Add(li.Value, li.Value);
                }
            }
            foreach (ListItem li in project_custom_dropdown3.Items)
            {
                if (li.Selected)
                {
                    previous_selection_dictionaries[2].Add(li.Value, li.Value);
                }
            }

            project_dropdown_select_cols = "";

            project_custom_dropdown1_label.InnerText = "";
            project_custom_dropdown2_label.InnerText = "";
            project_custom_dropdown3_label.InnerText = "";

            project_custom_dropdown1.Items.Clear();
            project_custom_dropdown2.Items.Clear();
            project_custom_dropdown3.Items.Clear();

            foreach (ListItem selected_project in selected_projects)
            {
                // Read the project dropdown info from the db.
                // Load the dropdowns as necessary

                if (selected_project.Value == "0")
                    continue;

                int pj_id = Convert.ToInt32(selected_project.Value);

                if (map_projects.ContainsKey(pj_id))
                {

                    BtnetProject btnet_project = map_projects[pj_id];

                    if (btnet_project.map_dropdowns[1].enabled)
                    {
                        if (project_custom_dropdown1_label.InnerText == "")
                        {
                            project_custom_dropdown1_label.InnerText = btnet_project.map_dropdowns[1].label;
                            project_custom_dropdown1_label.Style["display"] = "inline";
                            project_custom_dropdown1.Style["display"] = "block";
                        }
                        else if (project_custom_dropdown1_label.InnerText != btnet_project.map_dropdowns[1].label)
                        {
                            project_custom_dropdown1_label.InnerText = "dropdown1";
                        }
                        load_project_custom_dropdown(project_custom_dropdown1, btnet_project.map_dropdowns[1].values, dupe_detection_dictionaries[0]);
                    }

                    if (btnet_project.map_dropdowns[2].enabled)
                    {
                        if (project_custom_dropdown2_label.InnerText == "")
                        {
                            project_custom_dropdown2_label.InnerText = btnet_project.map_dropdowns[2].label;
                            project_custom_dropdown2_label.Style["display"] = "inline";
                            project_custom_dropdown2.Style["display"] = "block";
                        }
                        else if (project_custom_dropdown2_label.InnerText != btnet_project.map_dropdowns[2].label)
                        {
                            project_custom_dropdown2_label.InnerText = "dropdown2";
                        }
                        load_project_custom_dropdown(project_custom_dropdown2, btnet_project.map_dropdowns[2].values, dupe_detection_dictionaries[1]);
                    }

                    if (btnet_project.map_dropdowns[3].enabled)
                    {
                        if (project_custom_dropdown3_label.InnerText == "")
                        {
                            project_custom_dropdown3_label.InnerText = btnet_project.map_dropdowns[3].label;
                            project_custom_dropdown3_label.Style["display"] = "inline";
                            project_custom_dropdown3.Style["display"] = "block";
                            load_project_custom_dropdown(project_custom_dropdown3, btnet_project.map_dropdowns[3].values, dupe_detection_dictionaries[2]);
                        }
                        else if (project_custom_dropdown3_label.InnerText != btnet_project.map_dropdowns[3].label)
                        {
                            project_custom_dropdown3_label.InnerText = "dropdown3";
                        }
                        load_project_custom_dropdown(project_custom_dropdown3, btnet_project.map_dropdowns[3].values, dupe_detection_dictionaries[2]);
                    }
                }
            }

            if (project_custom_dropdown1_label.InnerText == "")
            {
                project_custom_dropdown1.Items.Clear();
                project_custom_dropdown1_label.Style["display"] = "none";
                project_custom_dropdown1.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown1_label.Style["display"] = "inline";
                project_custom_dropdown1.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value1,'') [" + project_custom_dropdown1_label.InnerText + "]";
            }

            if (project_custom_dropdown2_label.InnerText == "")
            {
                project_custom_dropdown2.Items.Clear();
                project_custom_dropdown2_label.Style["display"] = "none";
                project_custom_dropdown2.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown2_label.Style["display"] = "inline";
                project_custom_dropdown2.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value2,'') [" + project_custom_dropdown2_label.InnerText + "]";
            }

            if (project_custom_dropdown3_label.InnerText == "")
            {
                project_custom_dropdown3.Items.Clear();
                project_custom_dropdown3_label.Style["display"] = "none";
                project_custom_dropdown3.Style["display"] = "none";
            }
            else
            {
                project_custom_dropdown3_label.Style["display"] = "inline";
                project_custom_dropdown3.Style["display"] = "block";
                project_dropdown_select_cols
                    += ",\\nisnull(bg_project_custom_dropdown_value3,'') [" + project_custom_dropdown3_label.InnerText + "]";
            }

            // Restore user's previous selections.
            foreach (ListItem li in project_custom_dropdown1.Items)
            {
                li.Selected = (previous_selection_dictionaries[0].ContainsKey(li.Value));
            }
            foreach (ListItem li in project_custom_dropdown2.Items)
            {
                li.Selected = (previous_selection_dictionaries[1].ContainsKey(li.Value));
            }
            foreach (ListItem li in project_custom_dropdown3.Items)
            {
                li.Selected = (previous_selection_dictionaries[2].ContainsKey(li.Value));
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void load_drop_downs()
        {

            reported_by.DataSource = dt_users;
            reported_by.DataTextField = "us_username";
            reported_by.DataValueField = "us_id";
            reported_by.DataBind();


            // only show projects where user has permissions
            if (security.user.is_admin)
            {
                sql = new SQLString( "/* drop downs */ select pj_id, pj_name from projects order by pj_name;");
            }
            else
            {
                sql = new SQLString(@"/* drop downs */ select pj_id, pj_name
			from projects
			left outer join project_user_xref on pj_id = pu_project
			and pu_user = @us
			where isnull(pu_permission_level,@dpl) <> 0
			order by pj_name;");

                sql = sql.AddParameterWithValue("us", Convert.ToString(security.user.usid));
                sql = sql.AddParameterWithValue("dpl", Util.get_setting("DefaultPermissionLevel", "2"));
            }


            if (security.user.other_orgs_permission_level != 0)
            {
                sql.Append(" select og_id, og_name from orgs order by og_name;");
            }
            else
            {
                sql.Append(" select og_id, og_name from orgs where og_id = @ogId order by og_name;");
                sql.AddParameterWithValue("ogId", security.user.org.ToString());
                org.Visible = false;
                org_label.Visible = false;
            }

            sql.Append(@"
	select ct_id, ct_name from categories order by ct_sort_seq, ct_name;
	select pr_id, pr_name from priorities order by pr_sort_seq, pr_name;
	select st_id, st_name from statuses order by st_sort_seq, st_name;
	select udf_id, udf_name from user_defined_attribute order by udf_sort_seq, udf_name");

            DataSet ds_dropdowns = btnet.DbUtil.get_dataset(sql);

            project.DataSource = ds_dropdowns.Tables[0];
            project.DataTextField = "pj_name";
            project.DataValueField = "pj_id";
            project.DataBind();
            project.Items.Insert(0, new ListItem("[no project]", "0"));

            org.DataSource = ds_dropdowns.Tables[1];
            org.DataTextField = "og_name";
            org.DataValueField = "og_id";
            org.DataBind();
            org.Items.Insert(0, new ListItem("[no organization]", "0"));

            category.DataSource = ds_dropdowns.Tables[2];
            category.DataTextField = "ct_name";
            category.DataValueField = "ct_id";
            category.DataBind();
            category.Items.Insert(0, new ListItem("[no category]", "0"));

            priority.DataSource = ds_dropdowns.Tables[3];
            priority.DataTextField = "pr_name";
            priority.DataValueField = "pr_id";
            priority.DataBind();
            priority.Items.Insert(0, new ListItem("[no priority]", "0"));

            status.DataSource = ds_dropdowns.Tables[4];
            status.DataTextField = "st_name";
            status.DataValueField = "st_id";
            status.DataBind();
            status.Items.Insert(0, new ListItem("[no status]", "0"));

            assigned_to.DataSource = reported_by.DataSource;
            assigned_to.DataTextField = "us_username";
            assigned_to.DataValueField = "us_id";
            assigned_to.DataBind();
            assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));

            if (show_udf)
            {
                udf.DataSource = ds_dropdowns.Tables[5];
                udf.DataTextField = "udf_name";
                udf.DataValueField = "udf_id";
                udf.DataBind();
                udf.Items.Insert(0, new ListItem("[none]", "0"));
            }

            if (security.user.project_field_permission_level == Security.PERMISSION_NONE)
            {
                project_label.Style["display"] = "none";
                project.Style["display"] = "none";
            }
            if (security.user.org_field_permission_level == Security.PERMISSION_NONE)
            {
                org_label.Style["display"] = "none";
                org.Style["display"] = "none";
            }
            if (security.user.category_field_permission_level == Security.PERMISSION_NONE)
            {
                category_label.Style["display"] = "none";
                category.Style["display"] = "none";
            }
            if (security.user.priority_field_permission_level == Security.PERMISSION_NONE)
            {
                priority_label.Style["display"] = "none";
                priority.Style["display"] = "none";
            }
            if (security.user.status_field_permission_level == Security.PERMISSION_NONE)
            {
                status_label.Style["display"] = "none";
                status.Style["display"] = "none";
            }
            if (security.user.assigned_to_field_permission_level == Security.PERMISSION_NONE)
            {
                assigned_to_label.Style["display"] = "none";
                assigned_to.Style["display"] = "none";
            }
            if (security.user.udf_field_permission_level == Security.PERMISSION_NONE)
            {
                udf_label.Style["display"] = "none";
                udf.Style["display"] = "none";
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void write_custom_date_control(string name)
        {

            Response.Write("<input type=text class='txt date'");
            Response.Write("  onkeyup=\"on_change()\" ");
            int size = 10;
            string size_string = Convert.ToString(size);

            Response.Write(" size=" + size_string);
            Response.Write(" maxlength=" + size_string);

            Response.Write(" name=\"" + name + "\"");
            Response.Write(" id=\"" + name.Replace(" ", "") + "\"");

            Response.Write(" value=\"");
            if (Request[name] != "")
            {
                Response.Write(HttpUtility.HtmlEncode(Request[name]));
            }
            Response.Write("\"");
            Response.Write(">");

            Response.Write("<a style='font-size: 8pt;'  href=\"javascript:show_calendar('");
            Response.Write(name.Replace(" ", ""));
            Response.Write("')\">&nbsp;[select]</a>");
        }

        protected void write_custom_date_controls(string name)
        {
            Response.Write("from:&nbsp;&nbsp;");
            write_custom_date_control(name);
            Response.Write("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;to:&nbsp;&nbsp;");
            write_custom_date_control("to__" + name); // magic
        }

        protected SQLString sql;
        protected DataView dv;
        protected DataSet ds_custom_cols = null;

        ///////////////////////////////////////////////////////////////////////
        protected void display_bugs(bool show_checkboxes)
        {
            btnet.BugList.display_bugs(
                show_checkboxes,
                dv,
                Response,
                security,
                new_page.Value,
                IsPostBack,
                ds_custom_cols,
                filter.Value);
        }

        void call_sort_and_filter_buglist_dataview()
        {
            string filter_val = filter.Value;
            string sort_val = sort.Value;
            string prev_sort_val = prev_sort.Value;
            string prev_dir_val = prev_dir.Value;


            btnet.BugList.sort_and_filter_buglist_dataview(dv, IsPostBack,
                actn.Value,
                ref filter_val,
                ref sort_val,
                ref prev_sort_val,
                ref prev_dir_val);

            filter.Value = filter_val;
            sort.Value = sort_val;
            prev_sort.Value = prev_sort_val;
            prev_dir.Value = prev_dir_val;

        }

    }
}