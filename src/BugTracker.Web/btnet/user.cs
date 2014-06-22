/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace btnet
{
    public class User
    {
        public int usid = 0;
        public string username = "";
        public string fullname = "";
        public string email = "";
        public bool is_admin = false;
        public bool is_project_admin = false;
        public bool is_guest = false;
        public bool adds_not_allowed = false;
        public int bugs_per_page = 10;
        public bool enable_popups = true;
        public bool use_fckeditor = false;

        public bool external_user = false;
        public bool can_only_see_own_reported = false;
        public bool can_edit_sql = false;
        public bool can_delete_bug = false;

        public bool can_edit_and_delete_posts = false;
        public bool can_merge_bugs = false;
        public bool can_mass_edit_bugs = false;

        public bool can_use_reports = false;
        public bool can_edit_reports = false;
        public bool can_be_assigned_to = true;
        
        public bool can_view_tasks = true;
        public bool can_edit_tasks = true;
        public bool can_search = true;
        public bool can_assign_to_internal_users = false;

        public int other_orgs_permission_level = Security.PERMISSION_ALL;
        public int org = 0;
        public string org_name = "";
        public int forced_project = 0;

        public int assigned_to_field_permission_level = Security.PERMISSION_ALL;
        public int status_field_permission_level = Security.PERMISSION_ALL;
        public int category_field_permission_level = Security.PERMISSION_ALL;
        public int tags_field_permission_level = Security.PERMISSION_ALL;
        public int priority_field_permission_level = Security.PERMISSION_ALL;
        public int project_field_permission_level = Security.PERMISSION_ALL;
        public int org_field_permission_level = Security.PERMISSION_ALL;
        public int udf_field_permission_level = Security.PERMISSION_ALL;
        
        public Dictionary<string,int> dict_custom_field_permission_level = new Dictionary<string, int>();

        public void set_from_db(DataRow dr)
        {
            this.usid = Convert.ToInt32(dr["us_id"]);
            this.username = (string)dr["us_username"];
            this.email = (string)dr["us_email"];

            this.bugs_per_page = Convert.ToInt32(dr["us_bugs_per_page"]);
			if (Util.get_setting("DisableFCKEditor","0") == "1")
			{
				this.use_fckeditor = false;
			}
			else
			{
            	this.use_fckeditor = Convert.ToBoolean(dr["us_use_fckeditor"]);
			}
            this.enable_popups = Convert.ToBoolean(dr["us_enable_bug_list_popups"]);

            this.external_user = Convert.ToBoolean(dr["og_external_user"]);
            this.can_only_see_own_reported  = Convert.ToBoolean(dr["og_can_only_see_own_reported"]);
            this.can_edit_sql = Convert.ToBoolean(dr["og_can_edit_sql"]);
            this.can_delete_bug = Convert.ToBoolean(dr["og_can_delete_bug"]);
            this.can_edit_and_delete_posts = Convert.ToBoolean(dr["og_can_edit_and_delete_posts"]);
            this.can_merge_bugs = Convert.ToBoolean(dr["og_can_merge_bugs"]);
            this.can_mass_edit_bugs = Convert.ToBoolean(dr["og_can_mass_edit_bugs"]);
            this.can_use_reports = Convert.ToBoolean(dr["og_can_use_reports"]);
            this.can_edit_reports = Convert.ToBoolean(dr["og_can_edit_reports"]);
            this.can_be_assigned_to = Convert.ToBoolean(dr["og_can_be_assigned_to"]);
            this.can_view_tasks = Convert.ToBoolean(dr["og_can_view_tasks"]);
            this.can_edit_tasks = Convert.ToBoolean(dr["og_can_edit_tasks"]);
            this.can_search = Convert.ToBoolean(dr["og_can_search"]);
            this.can_assign_to_internal_users = Convert.ToBoolean(dr["og_can_assign_to_internal_users"]);
            this.other_orgs_permission_level = (int)dr["og_other_orgs_permission_level"];
            this.org = (int)dr["og_id"];
            this.org_name = (string) dr["og_name"];
            this.forced_project = (int)dr["us_forced_project"];

            this.category_field_permission_level = (int)dr["og_category_field_permission_level"];

            if (Util.get_setting("EnableTags","0") == "1")
            {
            	this.tags_field_permission_level = (int)dr["og_tags_field_permission_level"];
			}
			else
			{
				this.tags_field_permission_level = Security.PERMISSION_NONE;
			}
            this.priority_field_permission_level = (int)dr["og_priority_field_permission_level"];
            this.assigned_to_field_permission_level = (int)dr["og_assigned_to_field_permission_level"];
            this.status_field_permission_level = (int)dr["og_status_field_permission_level"];
            this.project_field_permission_level = (int)dr["og_project_field_permission_level"];
            this.org_field_permission_level = (int)dr["og_org_field_permission_level"];
            this.udf_field_permission_level = (int)dr["og_udf_field_permission_level"];

			// field permission for custom fields
			DataSet ds_custom = Util.get_custom_columns();
			foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
			{
				string bg_name = (string)dr_custom["name"];
				string og_name = "og_" 
					+ (string)dr_custom["name"]
					+ "_field_permission_level";
				
				try
				{
					object obj = dr[og_name];
					if (Convert.IsDBNull(obj))
					{
						dict_custom_field_permission_level[bg_name] = Security.PERMISSION_ALL;
					}
					else
					{
						dict_custom_field_permission_level[bg_name] = (int) dr[og_name];
					}
				}
				
				catch(Exception ex)
				{
					btnet.Util.write_to_log("exception looking for " + og_name + ":" + ex.Message);
					
					// automatically add it if it's missing
					btnet.DbUtil.execute_nonquery("alter table orgs add [" 
						+ og_name
						+ "] int null");
					dict_custom_field_permission_level[bg_name] = Security.PERMISSION_ALL;
				}
				
			}

            if (((string)dr["us_firstname"]).Trim().Length == 0)
            {
                this.fullname = (string)dr["us_lastname"];
            }
            else
            {
                this.fullname = (string)dr["us_lastname"] + ", " + (string)dr["us_firstname"];
            }


            if ((int)dr["us_admin"] == 1)
            {
                this.is_admin = true;
            }
            else
            {
                if ((int)dr["project_admin"] > 0)
                {
                    this.is_project_admin = true;
                }
                else
                {
                    if (this.username.ToLower() == "guest")
                    {
                        this.is_guest = true;
                    }
                }
            }


            // if user is forced to a specific project, and doesn't have
            // at least reporter permission on that project, than user
            // can't add bugs
            if ((int)dr["us_forced_project"] != 0)
            {
                if ((int)dr["pu_permission_level"] == Security.PERMISSION_READONLY
                || (int)dr["pu_permission_level"] == Security.PERMISSION_NONE)
                {
                    this.adds_not_allowed = true;
                }
            }
        }
        
        public static int copy_user(
        	string username, 
        	string email,
        	string firstname,
        	string lastname,
            string signature,
        	int salt,
        	string password,
        	string template_username,
            bool use_domain_as_org_name)
        {
            // get all the org columns

            btnet.Util.write_to_log("copy_user creating " + username + " from template user " + template_username);
            StringBuilder org_columns = new StringBuilder();

            string sql = "";

            if (use_domain_as_org_name)
            {
                sql = @" /* get org cols */
select sc.name
from syscolumns sc
inner join sysobjects so on sc.id = so.id
where so.name = 'orgs'
and sc.name not in ('og_id', 'og_name', 'og_domain')";

                DataSet ds = btnet.DbUtil.get_dataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    org_columns.Append(",");
                    org_columns.Append("[");
                    org_columns.Append(Convert.ToString(dr["name"]));
                    org_columns.Append("]");
                }

            }


			sql = @"
/* copy user */
declare @template_user_id int
declare @template_org_id int
select @template_user_id = us_id,
@template_org_id = us_org 
from users where us_username = N'$template_user'

declare @org_id int
set @org_id = -1

IF $use_domain_as_org_name = 1
BEGIN
    select @org_id = og_id from orgs where og_domain = N'$domain'
    IF @org_id = -1
    BEGIN
        insert into orgs
        (
            og_name,
            og_domain       
            $ORG_COLUMNS        
        )
        select 
        N'$domain',
        N'$domain'
        $ORG_COLUMNS
        from orgs where og_id = @template_org_id
        select @org_id = scope_identity()
    END
END

declare @new_user_id int
set @new_user_id = -1

IF NOT EXISTS (SELECT us_id FROM users WHERE us_username = '$username')
BEGIN

insert into users
	(us_username, us_email, us_firstname, us_lastname, us_signature, us_salt, us_password,
	us_default_query,
	us_enable_notifications,
	us_auto_subscribe,
	us_auto_subscribe_own_bugs,
	us_auto_subscribe_reported_bugs,
	us_send_notifications_to_self,
	us_active,
	us_bugs_per_page,
	us_forced_project,
	us_reported_notifications,
	us_assigned_notifications,
	us_subscribed_notifications,
	us_use_fckeditor,
	us_enable_bug_list_popups,
	us_org)

select
	N'$username', N'$email', N'$firstname', N'$lastname', N'$signature', $salt, N'$password',
	us_default_query,
	us_enable_notifications,
	us_auto_subscribe,
	us_auto_subscribe_own_bugs,
	us_auto_subscribe_reported_bugs,
	us_send_notifications_to_self,
	1, -- active
	us_bugs_per_page,
	us_forced_project,
	us_reported_notifications,
	us_assigned_notifications,
	us_subscribed_notifications,
	us_use_fckeditor,
	us_enable_bug_list_popups,
	case when @org_id = -1 then us_org else @org_id end
	from users where us_id = @template_user_id

select @new_user_id = scope_identity()

insert into project_user_xref
	(pu_project, pu_user, pu_auto_subscribe, pu_permission_level, pu_admin)

select pu_project, @new_user_id, pu_auto_subscribe, pu_permission_level, pu_admin
	from project_user_xref
	where pu_user = @template_user_id

select @new_user_id

END
";
            sql = sql.Replace("$username", username.Replace("'","''"));
			sql = sql.Replace("$email", email.Replace("'","''"));
			sql = sql.Replace("$firstname", firstname.Replace("'","''"));
			sql = sql.Replace("$lastname", lastname.Replace("'","''"));
            sql = sql.Replace("$signature", signature.Replace("'", "''"));
			sql = sql.Replace("$salt", Convert.ToString(salt));
			sql = sql.Replace("$password", password);
            sql = sql.Replace("$template_user", template_username.Replace("'", "''"));

            sql = sql.Replace("$use_domain_as_org_name", Convert.ToString(use_domain_as_org_name ? "1" : "0"));

            string[] email_parts = email.Split('@');
            if (email_parts.Length == 2)
            {
                sql = sql.Replace("$domain", email_parts[1].Replace("'", "''"));
            }
            else
            {
                sql = sql.Replace("$domain", email.Replace("'", "''"));
            }
            
            sql = sql.Replace("$ORG_COLUMNS", org_columns.ToString());
            return Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));
        
        }
        
    }; // end class
}
