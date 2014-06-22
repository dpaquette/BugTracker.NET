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
bool copy = false;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN_OR_PROJECT_ADMIN);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "edit user";

	if (!security.user.is_admin)
	{
		// Check if the current user is an admin for any project
		sql = @"select pu_project
			from project_user_xref
			where pu_user = $us
			and pu_admin = 1";
		sql = sql.Replace("$us", Convert.ToString(security.user.usid));
		DataSet ds_projects = btnet.DbUtil.get_dataset(sql);

		if (ds_projects.Tables[0].Rows.Count == 0)
		{
			Response.Write ("You not allowed to add users.");
			Response.End();
		}

		admin.Visible = false;
		admin_label.Visible = false;
		project_admin_label.Visible = false;
		project_admin.Visible = false;
		project_admin_help.Visible = false;

	}

	if (Request["copy"] != null && Request["copy"] == "y")
	{
		copy = true;
	}

	msg.InnerText = "";

	string var = Request.QueryString["id"];
	if (var == null)
	{
		id = 0;
		// MAW -- 2006/01/27 -- Set default settings when adding a new user
		auto_subscribe_own.Checked = true;
		auto_subscribe_reported.Checked = true;
		enable_popups.Checked = true;
		reported_notifications.Items[4].Selected = true;
		assigned_notifications.Items[4].Selected = true;
		subscribed_notifications.Items[4].Selected = true;
	}
	else
	{
		id = Convert.ToInt32(var);
	}

	if (!IsPostBack)
	{

		if (!security.user.is_admin)
		{

			// logged in user is a project level admin

			// get values for permissions grid
// Table 0
			sql = @"
				select pj_id, pj_name,
				isnull(a.pu_permission_level,$dpl) [pu_permission_level],
				isnull(a.pu_auto_subscribe,0) [pu_auto_subscribe],
				isnull(a.pu_admin,0) [pu_admin]
				from projects
				inner join project_user_xref project_admin on pj_id = project_admin.pu_project
				and project_admin.pu_user = $this_usid
				and project_admin.pu_admin = 1
				left outer join project_user_xref a on pj_id = a.pu_project
				and a.pu_user = $us
				order by pj_name;";


			sql = sql.Replace("$this_usid",Convert.ToString(security.user.usid));

		}
		else // user is a real admin
		{

// Table 0

			// populate permissions grid
			sql = @"
				select pj_id, pj_name,
				isnull(pu_permission_level,$dpl) [pu_permission_level],
				isnull(pu_auto_subscribe,0) [pu_auto_subscribe],
				isnull(pu_admin,0) [pu_admin]
				from projects
				left outer join project_user_xref on pj_id = pu_project
				and pu_user = $us
				order by pj_name;";

		}

// Table 1

		sql += @"/* populate query dropdown */
		    declare @org int
		    set @org = null
		    select @org = us_org from users where us_id = $us

			select qu_id, qu_desc
			from queries
			where (isnull(qu_user,0) = 0 and isnull(qu_org,0) = 0)
			or isnull(qu_user,0) = $us
			or isnull(qu_org,0) = isnull(@org,-1)
			order by qu_desc;";

// Table 2

		if (security.user.is_admin)
		{
			sql += @"/* populate org dropdown 1 */
				select og_id, og_name
				from orgs
				order by og_name;";
		}
		else
		{
			if (security.user.other_orgs_permission_level == Security.PERMISSION_ALL)
			{
				sql += @"/* populate org dropdown 2 */
					select og_id, og_name
					from orgs
					where og_non_admins_can_use = 1
					order by og_name;";
			}
			else
			{
				sql += @"/* populate org dropdown 3 */
					select 1; -- dummy";
			}
		}


// Table 3
		if (id !=0)
		{

			// get existing user values

			sql += @"
			select
				us_username,
				isnull(us_firstname,'') [us_firstname],
				isnull(us_lastname,'') [us_lastname],
				isnull(us_bugs_per_page,10) [us_bugs_per_page],
				us_use_fckeditor,
				us_enable_bug_list_popups,
				isnull(us_email,'') [us_email],
				us_active,
				us_admin,
				us_enable_notifications,
				us_send_notifications_to_self,
                us_reported_notifications,
                us_assigned_notifications,
                us_subscribed_notifications,
				us_auto_subscribe,
				us_auto_subscribe_own_bugs,
				us_auto_subscribe_reported_bugs,
				us_default_query,
				us_org,
				isnull(us_signature,'') [us_signature],
				isnull(us_forced_project,0) [us_forced_project],
				us_created_user
				from users
				where us_id = $us";

		}


		sql = sql.Replace("$us",Convert.ToString(id));
		sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel","2"));

		DataSet ds = btnet.DbUtil.get_dataset(sql);

		// query dropdown
		query.DataSource = ds.Tables[1].DefaultView;
		query.DataTextField = "qu_desc";
		query.DataValueField = "qu_id";
		query.DataBind();

		// forced project dropdown
		forced_project.DataSource = ds.Tables[0].DefaultView;
		forced_project.DataTextField = "pj_name";
		forced_project.DataValueField = "pj_id";
		forced_project.DataBind();
		forced_project.Items.Insert(0, new ListItem("[no forced project]", "0"));

		// org dropdown
		if (security.user.is_admin
		|| security.user.other_orgs_permission_level == Security.PERMISSION_ALL)
		{
			org.DataSource = ds.Tables[2].DefaultView;
			org.DataTextField = "og_name";
			org.DataValueField = "og_id";
			org.DataBind();
			org.Items.Insert(0, new ListItem("[select org]", "0"));
		}
		else
		{
			org.Items.Insert(0, new ListItem(security.user.org_name, Convert.ToString(security.user.org)));
		}

		// populate permissions grid
		MyDataGrid.DataSource=ds.Tables[0].DefaultView;
		MyDataGrid.DataBind();

		// subscribe by project dropdown
		project_auto_subscribe.DataSource = ds.Tables[0].DefaultView;
		project_auto_subscribe.DataTextField = "pj_name";
		project_auto_subscribe.DataValueField = "pj_id";
		project_auto_subscribe.DataBind();

		// project admin dropdown
		project_admin.DataSource = ds.Tables[0].DefaultView;
		project_admin.DataTextField = "pj_name";
		project_admin.DataValueField = "pj_id";
		project_admin.DataBind();


		// add or edit?
		if (id == 0)
		{
			sub.Value = "Create";
			bugs_per_page.Value = "10";
			active.Checked = true;
			enable_notifications.Checked = true;

		}
		else
		{


			sub.Value = "Update";

			// get the values for this existing user
			DataRow dr = ds.Tables[3].Rows[0];

			// check if project admin is allowed to edit this user
			if (!security.user.is_admin)
			{
				if (security.user.usid != (int) dr["us_created_user"])
				{
					Response.Write ("You not allowed to edit this user, because you didn't create it.");
					Response.End();
				}
				else if ((int) dr["us_admin"] == 1)
				{
					Response.Write ("You not allowed to edit this user, because it is an admin.");
					Response.End();
				}
			}


			// select values in dropdowns

			// select forced project
			int current_forced_project = (int) dr["us_forced_project"];
			foreach (ListItem li in forced_project.Items)
			{
				if (Convert.ToInt32(li.Value) == current_forced_project)
				{
					li.Selected = true;
					break;
				}
			}

			// Fill in this form
			if (copy)
			{
				username.Value = "Enter username here";
				firstname.Value = "";
				lastname.Value = "";
				email.Value = "";
				signature.InnerText = "";
			}
			else
			{
				username.Value = (string) dr["us_username"];
				firstname.Value = (string) dr["us_firstname"];
				lastname.Value = (string) dr["us_lastname"];
				email.Value = (string) dr["us_email"];
				signature.InnerText = (string) dr["us_signature"];
			}

			bugs_per_page.Value = Convert.ToString(dr["us_bugs_per_page"]);
			use_fckeditor.Checked = Convert.ToBoolean((int) dr["us_use_fckeditor"]);
			enable_popups.Checked = Convert.ToBoolean((int) dr["us_enable_bug_list_popups"]);
			active.Checked = Convert.ToBoolean((int) dr["us_active"]);
			admin.Checked = Convert.ToBoolean((int) dr["us_admin"]);
			enable_notifications.Checked = Convert.ToBoolean((int) dr["us_enable_notifications"]);
			send_to_self.Checked = Convert.ToBoolean((int) dr["us_send_notifications_to_self"]);
            reported_notifications.Items[(int)dr["us_reported_notifications"]].Selected = true;
            assigned_notifications.Items[(int)dr["us_assigned_notifications"]].Selected = true;
            subscribed_notifications.Items[(int)dr["us_subscribed_notifications"]].Selected = true;
            auto_subscribe.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe"]);
			auto_subscribe_own.Checked = Convert.ToBoolean((int) dr["us_auto_subscribe_own_bugs"]);
			auto_subscribe_reported.Checked = Convert.ToBoolean((int) dr["us_auto_subscribe_reported_bugs"]);


			// org
			foreach (ListItem li in org.Items)
			{
				if (Convert.ToInt32(li.Value) == (int) dr["us_org"])
				{
					li.Selected = true;
					break;
				}
			}

			// query
			foreach (ListItem li in query.Items)
			{
				if (Convert.ToInt32(li.Value) == (int) dr["us_default_query"])
				{
					li.Selected = true;
					break;
				}
			}

			// select projects
			foreach (DataRow dr2 in ds.Tables[0].Rows)
			{
				foreach (ListItem li in project_auto_subscribe.Items)
				{
					if (Convert.ToInt32(li.Value) == (int) dr2["pj_id"])
					{
						if ((int) dr2["pu_auto_subscribe"] == 1)
						{
							li.Selected = true;
						}
						else
						{
							li.Selected = false;
						}
					}
				}
			}

			foreach (DataRow dr3 in ds.Tables[0].Rows)
			{
				foreach (ListItem li in project_admin.Items)
				{
					if (Convert.ToInt32(li.Value) == (int) dr3["pj_id"])
					{
						if ((int) dr3["pu_admin"] == 1)
						{
							li.Selected = true;
						}
						else
						{
							li.Selected = false;
						}
					}
				}
			}

		} // add or edit
	} // if !postback
	else
	{
		on_update();
	}
}


///////////////////////////////////////////////////////////////////////
Boolean validate()
{

	Boolean good = true;
	if (username.Value == "")
	{
		good = false;
		username_err.InnerText = "Username is required.";
	}
	else
	{
		username_err.InnerText = "";
	}

	pw_err.InnerText = "";
	if (id == 0 || copy)
	{
		if (pw.Value == "")
		{
			good = false;
			pw_err.InnerText = "Password is required.";
		}
	}

	if (pw.Value != "")
	{
		if (!Util.check_password_strength(pw.Value))
		{
			good = false;
			pw_err.InnerHtml = "Password is not difficult enough to guess.";
			pw_err.InnerHtml += "<br>Avoid common words.";
			pw_err.InnerHtml += "<br>Try using a mixture of lowercase, uppercase, digits, and special characters.";
		}
	}

	if (confirm_pw.Value != pw.Value)
	{
		good = false;
		confirm_pw_err.InnerText = "Confirm Password must match Password.";
	}
	else
	{
		confirm_pw_err.InnerText = "";
	}

	if (org.SelectedItem.Text == "[select org]")
	{
		good = false;
		org_err.InnerText = "You must select a org";
	}
	else
	{
		org_err.InnerText = "";
	}

	if (!Util.is_int(bugs_per_page.Value))
	{
		good = false;
		bugs_per_page_err.InnerText = Util.get_setting("PluralBugLabel","Bugs") + " Per Page must be a number.";
	}
	else
	{
		bugs_per_page_err.InnerText = "";
	}

	email_err.InnerHtml = "";
	if (email.Value != "")
	{
		if (!Util.validate_email(email.Value))
		{
			good = false;
			email_err.InnerHtml = "Format of email address is invalid.";
		}
	}

	return good;
}

///////////////////////////////////////////////////////////////////////
class Project
{
	public int id = 0;
	public int admin = 0;
	public int permission_level = 0;
	public int auto_subscribe = 0;
	public bool maybe_insert = false;
};


///////////////////////////////////////////////////////////////////////
string replace_vars_in_sql_statement(string sql)
{
	sql = sql.Replace("$un", username.Value.Replace("'","''"));
	sql = sql.Replace("$fn", firstname.Value.Replace("'","''"));
	sql = sql.Replace("$ln", lastname.Value.Replace("'","''"));
	sql = sql.Replace("$bp", bugs_per_page.Value.Replace("'","''"));
	sql = sql.Replace("$fk", Util.bool_to_string(use_fckeditor.Checked));
	sql = sql.Replace("$pp", Util.bool_to_string(enable_popups.Checked));
	sql = sql.Replace("$em", email.Value.Replace("'","''"));
	sql = sql.Replace("$ac", Util.bool_to_string(active.Checked));
	sql = sql.Replace("$en", Util.bool_to_string(enable_notifications.Checked));
	sql = sql.Replace("$ss", Util.bool_to_string(send_to_self.Checked));
	sql = sql.Replace("$rn", reported_notifications.SelectedItem.Value);
	sql = sql.Replace("$an", assigned_notifications.SelectedItem.Value);
	sql = sql.Replace("$sn", subscribed_notifications.SelectedItem.Value);
	sql = sql.Replace("$as", Util.bool_to_string(auto_subscribe.Checked));
	sql = sql.Replace("$ao", Util.bool_to_string(auto_subscribe_own.Checked));
	sql = sql.Replace("$ar", Util.bool_to_string(auto_subscribe_reported.Checked));
	sql = sql.Replace("$dq", query.SelectedItem.Value);
	sql = sql.Replace("$org", org.SelectedItem.Value);
	sql = sql.Replace("$sg", signature.InnerText.Replace("'","''"));
	sql = sql.Replace("$fp", forced_project.SelectedItem.Value);
	sql = sql.Replace("$id", Convert.ToString(id));


	// only admins can create admins.
	if (security.user.is_admin)
	{
		sql = sql.Replace("$ad", Util.bool_to_string(admin.Checked));
	}
	else
	{
		sql = sql.Replace("$ad","0");
	}

	return sql;

}

///////////////////////////////////////////////////////////////////////
void on_update()
{
	Boolean good = validate();

	if (good)
	{

		if (id == 0 || copy)  // insert new
		{
			// See if the user already exists?
			sql = "select count(1) from users where us_username = N'$1'";
			sql = sql.Replace("$1", username.Value.Replace("'","''"));
			int user_count = (int) btnet.DbUtil.execute_scalar(sql);

			if (user_count == 0)
			{

				// MAW -- 2006/01/27 -- Converted to use new notification columns
				sql = @"
insert into users
(us_username, us_password,
us_firstname, us_lastname,
us_bugs_per_page,
us_use_fckeditor,
us_enable_bug_list_popups,
us_email,
us_active, us_admin,
us_enable_notifications,
us_send_notifications_to_self,
us_reported_notifications,
us_assigned_notifications,
us_subscribed_notifications,
us_auto_subscribe,
us_auto_subscribe_own_bugs,
us_auto_subscribe_reported_bugs,
us_default_query,
us_org,
us_signature,
us_forced_project,
us_created_user)

values (
N'$un', N'$pw', N'$fn', N'$ln',
$bp, $fk, $pp, N'$em',
$ac, $ad, $en,  $ss,
$rn, $an, $sn, $as,
$ao, $ar, $dq, $org, N'$sg',
$fp,
$createdby
);

select scope_identity()";

				sql = replace_vars_in_sql_statement(sql);
				sql = sql.Replace("$createdby", Convert.ToString(security.user.usid));

				// only admins can create admins.
				if (security.user.is_admin)
				{
					sql = sql.Replace("$ad", Util.bool_to_string(admin.Checked));
				}
				else
				{
					sql = sql.Replace("$ad","0");
				}

                // fill the password field with some junk, just temporarily.
                sql = sql.Replace("$pw", Convert.ToString(new Random().Next()));

                // insert the user
                id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

                // now encrypt the password and update the db
                btnet.Util.update_user_password(id, pw.Value);

                update_project_user_xref();

				Server.Transfer ("users.aspx");
			}
			else
			{
				username_err.InnerText = "User already exists.   Choose another username.";
				msg.InnerText = "User was not created.";
			}


		}
		else // edit existing
		{

			// See if the user already exists?
			sql = @"select count(1)
				from users where us_username = N'$1' and us_id <> $2" ;
			sql = sql.Replace("$1", username.Value.Replace("'","''"));
			sql = sql.Replace("$2", Convert.ToString(id));
			int user_count = (int) btnet.DbUtil.execute_scalar(sql);

			if (user_count == 0)
			{

				sql = @"
update users set
us_username = N'$un',
us_firstname = N'$fn',
us_lastname = N'$ln',
us_bugs_per_page = N'$bp',
us_use_fckeditor = $fk,
us_enable_bug_list_popups = $pp,
us_email = N'$em',
us_active = $ac,
us_admin = $ad,
us_enable_notifications = $en,
us_send_notifications_to_self = $ss,
us_reported_notifications = $rn,
us_assigned_notifications = $an,
us_subscribed_notifications = $sn,
us_auto_subscribe = $as,
us_auto_subscribe_own_bugs = $ao,
us_auto_subscribe_reported_bugs = $ar,
us_default_query = $dq,
us_org = $org,
us_signature = N'$sg',
us_forced_project = $fp
where us_id = $id";


				sql = replace_vars_in_sql_statement(sql);

				btnet.DbUtil.execute_nonquery(sql);

                // update the password
                if (pw.Value != "")
                {
                    btnet.Util.update_user_password(id, pw.Value);
                }

				update_project_user_xref();

				Server.Transfer ("users.aspx");
			}
			else
			{
				username_err.InnerText = "Username already exists.   Choose another username.";
				msg.InnerText = "User was not updated.";
			}

		}
	}
	else
	{
		if (id == 0)  // insert new
		{
			msg.InnerText = "User was not created.";
		}
		else // edit existing
		{
			msg.InnerText = "User was not updated.";
		}

	}

}

void update_project_user_xref()
{

	System.Collections.Hashtable hash_projects  = new System.Collections.Hashtable();


	foreach (ListItem li in project_auto_subscribe.Items)
	{
		Project p = new Project();
		p.id = Convert.ToInt32(li.Value);
		hash_projects[p.id] = p;

		if (li.Selected)
		{
			p.auto_subscribe = 1;
			p.maybe_insert = true;
		}
		else
		{
			p.auto_subscribe = 0;
		}
	}

	foreach (ListItem li in project_admin.Items)
	{
		Project	p = (Project) hash_projects[Convert.ToInt32(li.Value)];
		if (li.Selected)
		{
			p.admin = 1;
			p.maybe_insert = true;
		}
		else
		{
			p.admin = 0;
		}
	}


	RadioButton rb;
	int permission_level;
	int default_permission_level = Convert.ToInt32(Util.get_setting("DefaultPermissionLevel","2"));

	foreach (DataGridItem dgi in MyDataGrid.Items)
	{
		rb = (RadioButton) dgi.FindControl("none");
		if (rb.Checked)
		{
			permission_level = 0;
		}
		else
		{
			rb = (RadioButton) dgi.FindControl("readonly");
			if (rb.Checked)
			{
				permission_level = 1;
			}
			else
			{
				rb = (RadioButton) dgi.FindControl("reporter");
				if (rb.Checked)
				{
					permission_level = 3;
				}
				else
				{
					permission_level = 2;
				}
			}
		}

		int pj_id = Convert.ToInt32(dgi.Cells[1].Text);

		Project	p = (Project) hash_projects[pj_id];
		p.permission_level = permission_level;

		if (permission_level != default_permission_level)
		{
			p.maybe_insert = true;
		}
	}

	string projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.maybe_insert)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}
	}

	sql = "";

	// Insert new recs - we will update them later
	// Downstream logic is now simpler in that it just deals with existing recs
	if (projects != "")
	{
		sql += @"
			insert into project_user_xref (pu_project, pu_user, pu_auto_subscribe)
			select pj_id, $us, 0
			from projects
			where pj_id in ($projects)
			and pj_id not in (select pu_project from project_user_xref where pu_user = $us);";
		sql = sql.Replace("$projects", projects);
	}

	// First turn everything off, then turn selected ones on.
	sql += @"
		update project_user_xref
		set pu_auto_subscribe = 0,
		pu_admin = 0,
		pu_permission_level = $dpl
		where pu_user = $us;";

	projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.auto_subscribe == 1)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}
	}
	string auto_subscribe_projects = projects; // save for later

	if (projects != "")
	{
		sql += @"
			update project_user_xref
			set pu_auto_subscribe = 1
			where pu_user = $us
			and pu_project in ($projects);";
		sql = sql.Replace("$projects", projects);
	}

	if (security.user.is_admin)
	{
		projects = "";
		foreach (Project p in hash_projects.Values)
		{
			if (p.admin == 1)
			{
				if (projects != "")
				{
					projects += ",";
				}

				projects += Convert.ToString(p.id);
			}
		}

		if (projects != "")
		{
			sql += @"
				update project_user_xref
				set pu_admin = 1
				where pu_user = $us
				and pu_project in ($projects);";

			sql = sql.Replace("$projects", projects);
		}
	}

// update permission levels to 0
	projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.permission_level == 0)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}

	}
	if (projects != "")
	{
		sql += @"
			update project_user_xref
			set pu_permission_level = 0
			where pu_user = $us
			and pu_project in ($projects);";

		sql = sql.Replace("$projects", projects);
	}

// update permission levels to 1
	projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.permission_level == 1)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}

	}
	if (projects != "")
	{
		sql += @"
			update project_user_xref
			set pu_permission_level = 1
			where pu_user = $us
			and pu_project in ($projects);";

		sql = sql.Replace("$projects", projects);
	}


// update permission levels to 2
	projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.permission_level == 2)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}
	}
	if (projects != "")
	{
		sql += @"
			update project_user_xref
			set pu_permission_level = 2
			where pu_user = $us
			and pu_project in ($projects);";

		sql = sql.Replace("$projects", projects);
	}

// update permission levels to 3
	projects = "";
	foreach (Project p in hash_projects.Values)
	{
		if (p.permission_level == 3)
		{
			if (projects != "")
			{
				projects += ",";
			}

			projects += Convert.ToString(p.id);
		}
	}
	if (projects != "")
	{
		sql += @"
			update project_user_xref
			set pu_permission_level = 3
			where pu_user = $us
			and pu_project in ($projects);";

		sql = sql.Replace("$projects", projects);
	}


	// apply subscriptions retroactively
	if (retroactive.Checked)
	{
		sql = @"
			delete from bug_subscriptions where bs_user = $us;";

		if (auto_subscribe.Checked)
		{
			sql += @"
			insert into bug_subscriptions (bs_bug, bs_user)
				select bg_id, $us from bugs;";
		}
		else
		{
			if (auto_subscribe_reported.Checked)
			{
				sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_reported_user = $us
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
			}

			if (auto_subscribe_own.Checked)
			{
				sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_assigned_to_user = $us
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
			}

			if (auto_subscribe_projects != "")
			{
				sql += @"
					insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, $us from bugs where bg_project in ($projects)
					and bg_id not in (select bs_bug from bug_subscriptions where bs_user = $us);";
				sql = sql.Replace("$projects", auto_subscribe_projects);
			}
		}
	}

	sql = sql.Replace("$us", Convert.ToString(id));
	sql = sql.Replace("$dpl", Convert.ToString(default_permission_level));
	btnet.DbUtil.execute_nonquery(sql);

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet edit user</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">

<script>

var cls = (navigator.userAgent.indexOf("MSIE") > 0) ? "className" : "class";

function show_main_settings()
{
	document.getElementById("tab1").style.display = "block";
	document.getElementById("tab2").style.display = "none";
	document.getElementById("tab3").style.display = "none";
	document.getElementById("main_btn").setAttribute(cls, 'tab_btn_pushed');
	document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn');
	document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn');
}

function show_notification_settings()
{
	document.getElementById("tab1").style.display = "none";
	document.getElementById("tab2").style.display = "block";
	document.getElementById("tab3").style.display = "none";
	document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
	document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn_pushed');
	document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn');
}

function show_permissions_settings()
{
	document.getElementById("tab1").style.display = "none";
	document.getElementById("tab2").style.display = "none";
	document.getElementById("tab3").style.display = "block";
	document.getElementById("main_btn").setAttribute(cls, 'tab_btn');
	document.getElementById("notifications_btn").setAttribute(cls, 'tab_btn');
	document.getElementById("permissions_btn").setAttribute(cls, 'tab_btn_pushed');
}
</script>

</head>
<body>
<% security.write_menu(Response, "admin"); %>


<div class=align><table border=0><tr><td>

<a href=users.aspx>back to users</a>
&nbsp;&nbsp;&nbsp;&nbsp;


<form class=frm runat="server">

	<br>
	<a id="main_btn" class="tab_btn_pushed" href="javascript: show_main_settings()">main settings</a>
	<a id="notifications_btn" class="tab_btn" href="javascript: show_notification_settings()">email notification settings</a>
	<a id="permissions_btn" class="tab_btn" href="javascript: show_permissions_settings()">permissions</a>
	<br><br>

	<div id="tab1" style="display:block;">
	<table border=0 cellpadding=3>

	<tr>
	<td class=lbl>Username:</td>
	<td colspan=2><input runat="server" type=text class=txt id="username" maxlength=20 size=20></td>
	</tr>

	<tr>
	<td>&nbsp;</td>
	<td colspan=2 runat="server" class=err id="username_err">&nbsp;</td>
	</tr>


	<tr>
	<td class=lbl>Password:</td>
	<td colspan=2><input runat="server" autocomplete=off type=password class=txt id="pw" maxlength=20 size=20></td>
	</tr>

	<tr>
	<td>&nbsp;</td>
	<td colspan=2 runat="server" class=err id="pw_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Confirm Password:</td>
	<td colspan=2><input runat="server" autocomplete=off type=password class=txt id="confirm_pw" maxlength=20 size=20></td>
	</tr>

	<tr>
	<td>&nbsp;</td>
	<td colspan=2 runat="server" class=err id="confirm_pw_err">&nbsp;</td>
	</tr>


	<tr>
	<td class=lbl>Org:</td>
	<td colspan=2>
		<asp:DropDownList id="org" runat="server">
		</asp:DropDownList>
	</td>
	</tr>

	<tr>
	<td>&nbsp;</td>
	<td colspan=2 runat="server" class=err id="org_err">&nbsp;</td>
	</tr>


	<tr>
	<td class=lbl>First Name:</td>
	<td><input runat="server" type=text class=txt id="firstname" maxlength=20 size=20></td>
	<td runat="server" class=err id="firstname_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Last Name:</td>
	<td><input runat="server" type=text class=txt id="lastname" maxlength=20 size=20></td>
	<td runat="server" class=err id="lastname_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Active:</td>
	<td><asp:checkbox runat="server" class=cb id="active"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl id=admin_label runat="server">Admin:</td>
	<td><asp:checkbox runat="server" class=cb id="admin"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl><% Response.Write(Util.get_setting("PluralBugLabel","Bugs")); %> Per Page:</td>
	<td><input runat="server" type=text class=txt id="bugs_per_page" maxlength=3 size=3></td>
	<td runat="server" class=err id="bugs_per_page_err">&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Enable <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %>list popups:</td>
	<td><asp:checkbox runat="server" class=cb id="enable_popups"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Edit text using colors and fonts:</td>
	<td><asp:checkbox runat="server" class=cb id="use_fckeditor"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<div class=smallnote style="width: 400px;">Default <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %> Query is what you see when you click on the "<% Response.Write(Util.get_setting("PluralBugLabel","bug")); %>" link
	</div>
	</td>
	</tr>

	<tr>
	<td class=lbl>Default <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %> Query:</td>
	<td>
		<asp:DropDownList id="query" runat="server">
		</asp:DropDownList>
	</td>
	<td>&nbsp;</td>
	</tr>


	<tr>
	<td colspan=3>
	&nbsp;
	</td>
	</tr>

	<tr>
	<td class=lbl>Email:</td>
	<td colspan=2><input runat="server" type=text class=txt id="email" maxlength=60 size=60></td>
	</tr>

	<tr>
	<td>&nbsp;</td>
	<td colspan=2 runat="server" class=err id="email_err">&nbsp;</td>
	</tr>



	<tr>
	<td class=lbl valign=top>Outgoing Email Signature:</td>
	<td><textarea class="txt" id="signature" rows="2" cols="50" runat="server"></textarea></td>
	<td runat="server" class=err id="signature_err">&nbsp;</td>
	</tr>

	</table>
	</div>

	<div id="tab2" style="display:none;">
	<table border=0 cellpadding=3>

	<tr>
	<td colspan=3>
	<span class=smallnote>
	<br><br>
	<div style="width: 400px;">
	ADMIN - SEE "NotificationEmailEnabled", "NotificationEmailFrom", "SmtpServer" settings in Web.config.<br><br>
	To receive email notifications when items are added or changed, fill in your email address, enable notifications, and then select "Auto-subscribe to all items" or the other options.<br>
	<br>
	</div>
	</td>
	</tr>

	<tr>
	<td class=lbl>Enable notifications:</td>
	<td><asp:checkbox runat="server" class=cb id="enable_notifications"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td colspan=3>
	<br>
	<br>
	<div class=smallnote style="width: 400px;">You can AUTOMATICALLY subscribe to receive notifications to items by selecting either "Auto-subscribe to all items" or by selecting the other options.<br>
	</div>
	</td>
	</tr>


	<tr>
	<td class=lbl>Auto-subscribe to all items:</td>
	<td><asp:checkbox runat="server" class=cb id="auto_subscribe"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl id="project_auto_subscribe_label" runat="server" nowrap>Auto-subscribe per project:</td>
	<td>
		<span class=smallnote>Hold down Ctrl key to select multiple items.</span>
		<br>
		<asp:ListBox  id="project_auto_subscribe" runat="server" SelectionMode="Multiple" Rows=4>
		</asp:ListBox >
	</td>
	<td>&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Auto-subscribe to all items ASSIGNED TO you:</td>
	<td><asp:checkbox runat="server" class=cb id="auto_subscribe_own"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Auto-subscribe to all items REPORTED BY you:</td>
	<td><asp:checkbox runat="server" class=cb id="auto_subscribe_reported"/></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Apply subscription changes retroactively:</td>
	<td colspan=2><asp:checkbox runat="server" class=cb id="retroactive"/>&nbsp;&nbsp;
	<span class=smallnote>Delete old subscriptions and create new ones, according to above settings.</span></td>
	<td>&nbsp;</td>
	</tr>

	<tr>
	<td colspan=3>
	<br><br>
	<div class=smallnote style="width: 400px;">You can REDUCE or INCREASE the amount of email you receive by selecting the following.<br>
	</div>
	</td>
	</tr>

    <!-- MAW -- 2006/01/27 -- Added new notification controls -->
	<tr>
	<td class=lbl>Notifications for subscribed <% Response.Write(Util.get_setting("PluralBugLabel","bugs")); %> reported by me:</td>
	<td><asp:DropDownList runat="server" id="reported_notifications">
	    <asp:ListItem Value="0" Text="no notifications"/>
	    <asp:ListItem Value="1" Text="when created"/>
	    <asp:ListItem Value="2" Text="when status changes"/>
	    <asp:ListItem Value="3" Text="when status or assigned-to changes"/>
	    <asp:ListItem Value="4" Text="when anything changes"/>
	</asp:DropDownList></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Notifications for subscribed <% Response.Write(Util.get_setting("PluralBugLabel","bugs")); %> assigned to me:</td>
	<td><asp:DropDownList runat="server" id="assigned_notifications">
	    <asp:ListItem Value="0" Text="no notifications"/>
	    <asp:ListItem Value="1" Text="when created"/>
	    <asp:ListItem Value="2" Text="when status changes"/>
	    <asp:ListItem Value="3" Text="when status or assigned-to changes"/>
	    <asp:ListItem Value="4" Text="when anything changes"/>
	</asp:DropDownList></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Notifications for all other subscribed <% Response.Write(Util.get_setting("PluralBugLabel","bugs")); %>:</td>
	<td><asp:DropDownList runat="server" id="subscribed_notifications">
	    <asp:ListItem Value="0" Text="no notifications"/>
	    <asp:ListItem Value="1" Text="when created"/>
	    <asp:ListItem Value="2" Text="when status changes"/>
	    <asp:ListItem Value="3" Text="when status or assigned-to changes"/>
	    <asp:ListItem Value="4" Text="when anything changes"/>
	</asp:DropDownList></td>
	<td>&nbsp</td>
	</tr>

	<tr>
	<td class=lbl>Send notifications even for items you add or change:</td>
	<td><asp:checkbox runat="server" class=cb id="send_to_self"/></td>
	<td>&nbsp</td>
	</tr>


	</table>

	</div>
	<div id="tab3" style="display:none;">
	<table border=0 cellpadding=3>

	<tr><td colspan=3>&nbsp;


	<tr>
	<td colspan=2 class=lbl>Force user to add new <% Response.Write(Util.get_setting("PluralBugLabel","bugs")); %> to this project:&nbsp;&nbsp;
	<asp:DropDownList id="forced_project" runat="server">
	</asp:DropDownList>
	</td>
	</tr>

	<tr>
	<td colspan=3>
	<br><br>
	<div class=smallnote style="width: 400px;">If you have only given view permissions to this user, set the forced project to any of the view-only projects.<br>
	</div>
	</td>
	</tr>


	<tr><td colspan=3>&nbsp;

	<tr>
	<td colspan=2>
	<ASP:DataGrid id="MyDataGrid" runat="server" BorderColor="black" CssClass="datat" CellPadding="3" AutoGenerateColumns="false">
		<HeaderStyle cssclass="datah"></HeaderStyle>
		<ItemStyle cssclass="datad"></ItemStyle>
		<Columns>


		<asp:BoundColumn HeaderText="Project" DataField="pj_name"/>

		<asp:BoundColumn HeaderText="Project" DataField="pj_id" Visible="False"/>

		<asp:TemplateColumn HeaderText="Permissions">
			<ItemTemplate>
				<asp:RadioButton GroupName="permissions" text="none" value=0 ID="none" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 0 ) %>/>

				<asp:RadioButton GroupName="permissions" text="view only" value=1 ID="readonly" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 1 ) %>/>

				<asp:RadioButton GroupName="permissions" text="report (add and comment only)" value=3 ID="reporter" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 3 ) %>/>

				<asp:RadioButton GroupName="permissions" text="all (add and edit)" value=2 ID="edit" runat="server"
				Checked=<%# ((int)(( DataRowView ) Container.DataItem ) [ "pu_permission_level" ] == 2 ) %>/>

			</ItemTemplate>
		</asp:TemplateColumn>


		</Columns>
	</ASP:DataGrid>

	<tr><td colspan=3>&nbsp;

	<tr>
	<td colspan=2 class=lbl id="project_admin_label" runat="server" nowrap>Allowed to add/delete other users for the following projects:

		<br><span id="project_admin_help" runat="server" class=smallnote>Hold down Ctrl key to select multiple items.</span>
		<br><br>
		<asp:ListBox  id="project_admin" runat="server" SelectionMode="Multiple" Rows=4>
		</asp:ListBox >
	</td>
	<td>&nbsp;</td>
	</tr>
	<tr><td colspan=3>&nbsp;

	</table>
	</div>


	<table border=0>


	<tr><td colspan=3 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr>
	<td width=300px>&nbsp;
	<td align=center>
	<input runat="server" class=btn type=submit id="sub" value="Create or Edit">
	<td>&nbsp</td>
	</td>
	</tr>

	</table>
</form>
</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


