<%@ Page language="C#" validateRequest="false"%>

<!--
Copyright 2002-2013 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat=server>

Security security;
int id;
DataSet ds_posts;

int permission_level;
String sql;
bool status_changed;
bool assigned_to_changed;
String err_text;

//SortedDictionary<string, string> hash_custom_cols = new SortedDictionary<string, string>();
//SortedDictionary<string, string> hash_prev_custom_cols = new SortedDictionary<string, string>();

void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

    Util.do_not_cache(Response);

    security = new Security();
    security.check_security(HttpContext.Current, Security.ANY_USER_OK);
    if (btnet.Util.get_setting("EnableMobile", "0") == "0") {
        Response.Write("BugTracker.NET EnableMobile is not set to 1 in Web.config");
        Response.End();
    }

    msg.InnerText = "";
    err_text = "";
    
	string string_bugid = Request["id"];

    if (string_bugid == null || string_bugid == "" || string_bugid == "0")
    {
        id = 0;

        submit_button.Value = "Create";
        titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - Create ";
        my_header.InnerText = titl.InnerText;


        if (IsPostBack)
        {

            if (!validate())
            {
                msg.InnerHtml = err_text;
            }
            else
            {
                String result = insert_bug();
                if (result != "")
                {
                    msg.InnerHtml = err_text;
                }
                else
                {
                    Response.Redirect("mbugs.aspx");
                }
            }

        }
        else
        {

            load_dropdowns(security.user);

            sql = "\nselect top 1 pj_id from projects where pj_default = 1 order by pj_name;"; // 0
            sql += "\nselect top 1 st_id from statuses where st_default = 1 order by st_name;"; // 1

            DataSet ds_defaults = btnet.DbUtil.get_dataset(sql);
            DataTable dt_project_default = ds_defaults.Tables[0];
            DataTable dt_status_default = ds_defaults.Tables[1];

            String default_value;

            // status
            if (ds_defaults.Tables[1].Rows.Count > 0)
            {
                default_value = Convert.ToString((int)dt_status_default.Rows[0][0]);
            }
            else
            {
                default_value = "0";
            }
            foreach (ListItem li in status.Items)
            {
                if (li.Value == default_value)
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }


            // get default values
            string initial_project = (string)Session["project"];

            // project
            if (security.user.forced_project != 0)
            {
                initial_project = Convert.ToString(security.user.forced_project);
            }

            if (initial_project != null && initial_project != "0")
            {
                foreach (ListItem li in project.Items)
                {
                    if (li.Value == initial_project)
                    {
                        li.Selected = true;
                    }
                    else
                    {
                        li.Selected = false;
                    }
                }
            }
            else
            {
                if (dt_project_default.Rows.Count > 0)
                {
                    default_value = Convert.ToString((int)dt_project_default.Rows[0][0]);
                }
                else
                {
                    default_value = "0";
                }

                foreach (ListItem li in project.Items)
                {
                    if (li.Value == default_value)
                    {
                        li.Selected = true;
                    }
                    else
                    {
                        li.Selected = false;
                    }
                }
            }

        } // not postback        
        
    }
    else
    {
       
        id = Convert.ToInt32(string_bugid);

        submit_button.Value = "Update";
        titl.InnerText = btnet.Util.get_setting("AppTitle", "BugTracker.NET") + " - Update";
        my_header.InnerText = titl.InnerText;
        
        
        if (IsPostBack)
        {

            if (!validate())
            {
                msg.InnerHtml = err_text;
            }
            else {            
                String result = update_bug();
                if (result != "") {
                    msg.InnerHtml = err_text;
                }
                else {
                    Response.Redirect("mbugs.aspx");
                }
            }                        

        }
        
        DataRow dr = btnet.Bug.get_bug_datarow(id, security);

        titl.InnerText += " #" + string_bugid;
        my_header.InnerText = titl.InnerText;
        
        created_by.InnerText = Convert.ToString(dr["reporter"]);
        short_desc.Value = Convert.ToString(dr["short_desc"]);

        // load dropdowns
        load_dropdowns(security.user);

        // project
        foreach (ListItem li in project.Items)
        {
            if (Convert.ToInt32(li.Value) == (int)dr["project"])
            {
                li.Selected = true;
            }
            else
            {
                li.Selected = false;
            }
        }
                
        // status
        foreach (ListItem li in status.Items)
        {
            if (Convert.ToInt32(li.Value) == (int)dr["status"])
            {
                li.Selected = true;
            }
            else
            {
                li.Selected = false;
            }
        }

        // status
        foreach (ListItem li in assigned_to.Items)
        {
            if (Convert.ToInt32(li.Value) == (int)dr["assigned_to_user"])
            {
                li.Selected = true;
            }
            else
            {
                li.Selected = false;
            }
        }
                                
        // Posts
        permission_level = (int)dr["pu_permission_level"];
        ds_posts = PrintBug.get_bug_posts(id, security.user.external_user, true);

        // save current values in previous, so that later we can write the audit trail when things change
        prev_short_desc.Value = (string)dr["short_desc"];
        prev_assigned_to.Value = Convert.ToString((int)dr["assigned_to_user"]);
        prev_assigned_to_username.Value = Convert.ToString(dr["assigned_to_username"]);
        prev_status.Value = Convert.ToString((int)dr["status"]);

        
    }        
}

void load_dropdowns(User user)
{

    // only show projects where user has permissions
    // 0
    String sql = @"/* drop downs */ select pj_id, pj_name
		from projects
		left outer join project_user_xref on pj_id = pu_project
		and pu_user = $us
		where pj_active = 1
		and isnull(pu_permission_level,$dpl) not in (0, 1)
		order by pj_name;";

    sql = sql.Replace("$us", Convert.ToString(security.user.usid));
    sql = sql.Replace("$dpl", btnet.Util.get_setting("DefaultPermissionLevel", "2"));

    //1
    sql += "\nselect us_id, us_username from users order by us_username;";

    //2
    sql += "\nselect st_id, st_name from statuses order by st_sort_seq, st_name;";


    // do a batch of sql statements
    DataSet ds_dropdowns = btnet.DbUtil.get_dataset(sql);

    project.DataSource = ds_dropdowns.Tables[0];
    project.DataTextField = "pj_name";
    project.DataValueField = "pj_id";
    project.DataBind();
    project.Items.Insert(0, new ListItem("[not assigned]", "0"));
    
    assigned_to.DataSource = ds_dropdowns.Tables[1];
    assigned_to.DataTextField = "us_username";
    assigned_to.DataValueField = "us_id";
    assigned_to.DataBind();
    assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));
    
    status.DataSource = ds_dropdowns.Tables[2];
    status.DataTextField = "st_name";
    status.DataValueField = "st_id";
    status.DataBind();
    status.Items.Insert(0, new ListItem("[no status]", "0"));

}

///////////////////////////////////////////////////////////////////////
string update_bug() {

    status_changed = false;
    assigned_to_changed = false;
    
    sql = @"update bugs set
				bg_short_desc = N'$sd$',
                        bg_project = $pj$,
						bg_assigned_to_user = $au$,
						bg_status = $st$,
						bg_last_updated_user = $lu$,
						bg_last_updated_date = getdate()
						where bg_id = $id$";

    sql = sql.Replace("$pj$", project.SelectedItem.Value);
    sql = sql.Replace("$au$", assigned_to.SelectedItem.Value);
    sql = sql.Replace("$st$", status.SelectedItem.Value);
	sql = sql.Replace("$lu$", Convert.ToString(security.user.usid));
    sql = sql.Replace("$sd$", short_desc.Value.Replace("'", "''"));
    sql = sql.Replace("$id$", Convert.ToString(id));

    btnet.DbUtil.execute_nonquery(sql);

    bool bug_fields_have_changed = record_changes();
        
    String comment_text = HttpUtility.HtmlDecode(comment.Value);

    bool bugpost_fields_have_changed = (btnet.Bug.insert_comment(
        id,
        security.user.usid,
        comment_text,
        comment_text,
        null, // from
        null, // cc
        "text/plain",
        false) != 0); // internal only

    if (bug_fields_have_changed || bugpost_fields_have_changed)
    {
        btnet.Bug.send_notifications(btnet.Bug.UPDATE, id, security, 0,
            status_changed, 
            assigned_to_changed,
            0); // Convert.ToInt32(assigned_to.SelectedItem.Value));

    }
        
    comment.Value = "";

    return "";
}

///////////////////////////////////////////////////////////////////////
// returns true if there was a change
bool record_changes()
{

    string base_sql = @"
		insert into bug_posts
		(bp_bug, bp_user, bp_date, bp_comment, bp_type)
		values($id, $us, getdate(), N'$3', 'update')";

    base_sql = base_sql.Replace("$id", Convert.ToString(id));
    base_sql = base_sql.Replace("$us", Convert.ToString(security.user.usid));

    string from;
    sql = "";

    bool do_update = false;

    if (prev_short_desc.Value != short_desc.Value)
    {

        do_update = true;
        sql += base_sql.Replace(
            "$3",
            "changed desc from \""
            + prev_short_desc.Value.Replace("'", "''") + "\" to \""
            + short_desc.Value.Replace("'", "''") + "\"");

        prev_short_desc.Value = short_desc.Value;
    }

    if (project.SelectedItem.Value != prev_project.Value)
    {

        // The "from" might not be in the dropdown anymore
        //from = get_dropdown_text_from_value(project, prev_project.Value);

        do_update = true;
        sql += base_sql.Replace(
            "$3",
            "changed project from \""
            + prev_project_name.Value.Replace("'", "''") + "\" to \""
            + project.SelectedItem.Text.Replace("'", "''") + "\"");

        prev_project.Value = project.SelectedItem.Value;
        prev_project_name.Value = project.SelectedItem.Text;

    }    

    if (prev_assigned_to.Value != assigned_to.SelectedItem.Value)
    {

        assigned_to_changed = true; // for notifications

        do_update = true;
        sql += base_sql.Replace(
            "$3",
            "changed assigned_to from \""
            + prev_assigned_to_username.Value.Replace("'", "''") + "\" to \""
            + assigned_to.SelectedItem.Text.Replace("'", "''") + "\"");

        prev_assigned_to.Value = assigned_to.SelectedItem.Value;
        prev_assigned_to_username.Value = assigned_to.SelectedItem.Text;
    }


    if (prev_status.Value != status.SelectedItem.Value)
    {
        status_changed = true; // for notifications

        from = get_dropdown_text_from_value(status, prev_status.Value);

        do_update = true;
        sql += base_sql.Replace(
            "$3",
            "changed status from \""
            + from.Replace("'", "''") + "\" to \""
            + status.SelectedItem.Text.Replace("'", "''") + "\"");

        prev_status.Value = status.SelectedItem.Value;

    }



    if (do_update
    && btnet.Util.get_setting("TrackBugHistory", "1") == "1") // you might not want the debris to grow
    {
        btnet.DbUtil.execute_nonquery(sql);
    }


    // return true if something did change
    return do_update;
}

///////////////////////////////////////////////////////////////////////
String insert_bug() {

    String comment_text = HttpUtility.HtmlDecode(comment.Value);
    
    btnet.Bug.NewIds new_ids = btnet.Bug.insert_bug(
        short_desc.Value,
        security,
        "", //tags.Value,
        Convert.ToInt32(project.SelectedItem.Value),
        0, //Convert.ToInt32(org.SelectedItem.Value),
        0, //Convert.ToInt32(category.SelectedItem.Value),
        0, //Convert.ToInt32(priority.SelectedItem.Value),
        Convert.ToInt32(status.SelectedItem.Value),
        Convert.ToInt32(assigned_to.SelectedItem.Value),
        0, // Convert.ToInt32(udf.SelectedItem.Value),
        "",
        "",
        "",
        comment_text,
        comment_text,
        null, // from
        null, // cc
        "text/plain", // commentType,
        false, // internal_only.Checked,
        null, // hash_custom_cols,
        true); // send notifications

    return "";        
}

///////////////////////////////////////////////////////////////////////
bool validate()
{

    bool is_valid = true;

    if (short_desc.Value == "")
    {
        is_valid = false;
        err_text += "Description is required.<br>";
    }

    return is_valid;
}
 

/// ///////////////////////////////////////////////////////////////////////////
string get_dropdown_text_from_value(DropDownList dropdown, string value)
{
    foreach (ListItem li in dropdown.Items)
    {
        if (li.Value == value)
        {
            return li.Text;
        }
    }

    return dropdown.Items[0].Text;
}

</script>

<html> 
<head> 
<title id=titl runat=server>BugTracker.NET Edit</title> 
<meta name="viewport" content="width=device-width, initial-scale=1"> 
<link rel="stylesheet" href="jquery/jquery.mobile-1.2.0.min.css" />
<link rel="stylesheet" href="mbtnet_base.css" />

<script src="jquery/jquery-1.8.2.min.js"></script>
<script src="jquery/jquery.mobile-1.2.0.min.js"></script>
</head> 
<body> 
<div class="page" data-role="page" data-cache="never">

	<div data-role="header">
		<h1 id=my_header runat=server>BugTracker.NET Edit</h1>
	</div><!-- /header -->

<div data-role="content">	
<a class="ui-submit" data-ajax='false' href="mbugs.aspx" data-role="button" data-icon="arrow-l" data-iconpos="left">Back to List</a>

<form data-ajax="false" id="Form1" class=frm runat=server>
	<div class=err runat=server id="msg">&nbsp;</div>

    <label>Description:</label>
	<textarea runat=server id="short_desc" maxlength=200></textarea>

    <label>Project:</label>
	<asp:DropDownList id="project" runat=server></asp:DropDownList>

    <label>Assigned to:</label>
	<asp:DropDownList id="assigned_to" runat=server></asp:DropDownList>

    <label>Status:</label>
	<asp:DropDownList id="status" runat=server></asp:DropDownList>

    <label>Comment:</label>
    <textarea id="comment" runat=server></textarea>
	<input data-role="button" id="submit_button" type=submit value="Button" runat=server>

    <% if (id != 0) { %>
    <br />
    <div>Reported by <span id="created_by" runat=server></span></div>
    <% } %>

    <input type=hidden id=prev_status runat=server />
    <input type=hidden id=prev_short_desc runat=server />
    <input type=hidden id=prev_assigned_to runat=server />
    <input type=hidden id=prev_assigned_to_username runat=server />
    <input type=hidden id=prev_project runat=server />
    <input type=hidden id=prev_project_name runat=server />
</form>

<div id="posts">

	<%
	// COMMENTS
	if (id != 0)
	{
        PrintBug.write_posts(
            ds_posts,
			Response,
			id,
			permission_level,
			false, // write links
			false, // images inline
			true, // history inline
            false, // internal_posts
			security.user);
	}

	%>


</div>

</div><!-- /content -->

</div><!-- /page -->

</body>
</html>