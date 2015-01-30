using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace btnet
{
    public partial class edit_bug : BasePage
    {
        protected int id;
        SQLString sql;

        DataRow dr_bug;
        DataTable dt_users = null;
        protected DataSet ds_posts = null;

        SortedDictionary<string, string> hash_custom_cols = new SortedDictionary<string, string>();

        protected int permission_level;

        protected bool images_inline = true;
        protected bool history_inline = false;

        bool status_changed = false;
        bool assigned_to_changed = false;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        bool good = true;

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {
            Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");
            Util.do_not_cache(Response);

            set_msg("");
            set_custom_field_msg("");


            string string_bugid = Request["id"];
            if (string_bugid == null || string_bugid == "0" || (string_bugid != "0" && clone_ignore_bugid.Value == "1"))
            {
                // New
                id = 0;
                bugid_label.InnerHtml = "Description:&nbsp;";

            }
            else
            {
                if (!Util.is_int(string_bugid))
                {
                    display_bugid_must_be_integer();
                    return;
                }
                else
                {
                    // Existing
                    id = Convert.ToInt32(string_bugid);
                }

                bugid_label.Visible = true;
                bugid_label.InnerHtml = Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")) + " ID:&nbsp;";

            }

            if (!IsPostBack)
            {
                // Fetch stuff from db and put on page

                if (id == 0)
                {
                    prepare_for_insert();
                }
                else
                {
                    get_cookie_values_for_show_hide_toggles();

                    // Get this entry's data from the db and fill in the form
                    dr_bug = Bug.get_bug_datarow(id, User.Identity);

                    prepare_for_update();
                }

                if (User.Identity.GetIsExternalUser() || Util.get_setting("EnableInternalOnlyPosts", "0") == "0")
                {
                    internal_only.Visible = false;
                    internal_only_label.Visible = false;
                }

            }
            else
            {
                get_cookie_values_for_show_hide_toggles();

                load_incoming_custom_col_vals_into_hash();

                if (did_user_hit_submit_button()) // or is this a project dropdown autopostback?
                {

                    // Get this entry's data from the db and fill in the form
                    dr_bug = Bug.get_bug_datarow(id, User.Identity);

                    good = validate();

                    if (good)
                    {
                        // Actually do the update
                        if (id == 0)
                        {
                            do_insert();
                        }
                        else
                        {
                            do_update();
                        }
                    }
                    else // bad, invalid
                    {
                        // Say we didn't do anything.
                        if (id == 0)
                        {
                            set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")) + " was not created.");
                        }
                        else
                        {
                            set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")) + " was not updated.");
                        }
                    }
                }
                else
                {
                    // This is the project dropdown autopost back.
                    load_user_dropdown();
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////
        bool did_user_hit_submit_button()
        {
            String val = Request["user_hit_submit"];
            return (val == "1");
        }

        ///////////////////////////////////////////////////////////////////////
        void get_cookie_values_for_show_hide_toggles()
        {

            HttpCookie cookie = Request.Cookies["images_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                images_inline = false;
            }
            else
            {
                images_inline = true;
            }

            cookie = Request.Cookies["history_inline"];
            if (cookie == null || cookie.Value == "0")
            {
                history_inline = false;
            }
            else
            {
                history_inline = true;
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void prepare_for_insert()
        {

            if (!User.Identity.GetCanAddBugs())
            {
                Util.display_bug_not_found(Response, id); // TODO wrong message
                return;
            }

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - Create ";
            Page.Header.Title += Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"));

            submit_button.Value = "Create";

            if (Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
            {
                submit_button2.Value = "Create";
            }

            load_dropdowns_for_insert();

            // We don't know the project yet, so all permissions
            set_controls_field_permission(PermissionLevel.All);

            // Execute code not written by me
            Workflow.custom_adjust_controls(null, User.Identity, this);

        }

        ///////////////////////////////////////////////////////////////////////
        void load_dropdowns_for_insert()
        {
            load_dropdowns();

            // Get the defaults
            sql = new SQLString("\nselect top 1 pj_id from projects where pj_default = 1 order by pj_name;"); // 0
            sql.Append("\nselect top 1 ct_id from categories where ct_default = 1 order by ct_name;");  // 1
            sql.Append("\nselect top 1 pr_id from priorities where pr_default = 1 order by pr_name;"); // 2
            sql.Append("\nselect top 1 st_id from statuses where st_default = 1 order by st_name;"); // 3
            sql.Append("\nselect top 1 udf_id from user_defined_attribute where udf_default = 1 order by udf_name;"); // 4

            DataSet ds_defaults = DbUtil.get_dataset(sql);

            load_project_and_user_dropdown_for_insert(ds_defaults.Tables[0]);

            load_other_dropdowns_and_select_defaults(ds_defaults);

        }

        ///////////////////////////////////////////////////////////////////////
        void prepare_for_update()
        {

            if (dr_bug == null)
            {
                Util.display_bug_not_found(Response, id);
                return;
            }

            // look at permission level and react accordingly
            permission_level = (int)dr_bug["pu_permission_level"];

            if (permission_level == PermissionLevel.None)
            {
                Util.display_you_dont_have_permission(Response);
                return;
            }



            // move stuff to the page

            bugid.InnerText = Convert.ToString((int)dr_bug["id"]);

            // Fill in this form
            short_desc.Value = (string)dr_bug["short_desc"];
            tags.Value = (string)dr_bug["bg_tags"];
            Page.Header.Title = Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))
                + " ID " + Convert.ToString(dr_bug["id"]) + " " + (string)dr_bug["short_desc"];

            // reported by
            string s;
            s = "Created by ";
            s += PrintBug.format_email_username(
                    true,
                    Convert.ToInt32(dr_bug["id"]),
                    permission_level,
                    Convert.ToString(dr_bug["reporter_email"]),
                    Convert.ToString(dr_bug["reporter"]),
                    Convert.ToString(dr_bug["reporter_fullname"]));
            s += " on ";
            s += Util.format_db_date_and_time(dr_bug["reported_date"]);
            s += ", ";
            s += Util.how_long_ago((int)dr_bug["seconds_ago"]);

            reported_by.InnerHtml = s;

            // save current values in previous, so that later we can write the audit trail when things change
            prev_short_desc.Value = (string)dr_bug["short_desc"];
            prev_tags.Value = (string)dr_bug["bg_tags"];
            prev_project.Value = Convert.ToString((int)dr_bug["project"]);
            prev_project_name.Value = Convert.ToString(dr_bug["current_project"]);
            prev_org.Value = Convert.ToString((int)dr_bug["organization"]);
            prev_org_name.Value = Convert.ToString(dr_bug["og_name"]);
            prev_category.Value = Convert.ToString((int)dr_bug["category"]);
            prev_priority.Value = Convert.ToString((int)dr_bug["priority"]);
            prev_assigned_to.Value = Convert.ToString((int)dr_bug["assigned_to_user"]);
            prev_assigned_to_username.Value = Convert.ToString(dr_bug["assigned_to_username"]);
            prev_status.Value = Convert.ToString((int)dr_bug["status"]);
            prev_udf.Value = Convert.ToString((int)dr_bug["udf"]);
            prev_pcd1.Value = (string)dr_bug["bg_project_custom_dropdown_value1"];
            prev_pcd2.Value = (string)dr_bug["bg_project_custom_dropdown_value2"];
            prev_pcd3.Value = (string)dr_bug["bg_project_custom_dropdown_value3"];

            load_dropdowns_for_update();

            load_project_and_user_dropdown_for_update(); // must come before set_controls_field_permission, after assigning to prev_ values

            set_controls_field_permission(permission_level);

            snapshot_timestamp.Value = Convert.ToDateTime(dr_bug["snapshot_timestamp"]).ToString("yyyyMMdd HH\\:mm\\:ss\\:fff");

            prepare_a_bunch_of_links_for_update();

            format_prev_next_bug();

            // save for next bug
            if (project.SelectedItem != null)
            {
                Session["project"] = project.SelectedItem.Value;
            }

            // Execute code not written by me
            Workflow.custom_adjust_controls(dr_bug, User.Identity, this);
        }

        ///////////////////////////////////////////////////////////////////////
        void prepare_a_bunch_of_links_for_update()
        {

            string toggle_images_link = "<a href='javascript:toggle_images2("
                + Convert.ToString(id) + ")'><span id=hideshow_images>"
                + (images_inline ? "hide" : "show")
                + " inline images"
                + "</span></a>";
            toggle_images.InnerHtml = toggle_images_link;

            string toggle_history_link = "<a href='javascript:toggle_history2("
                + Convert.ToString(id) + ")'><span id=hideshow_history>"
                + (history_inline ? "hide" : "show")
                + " change history"
                + "</span></a>";
            toggle_history.InnerHtml = toggle_history_link;

            if (permission_level == PermissionLevel.All)
            {
                string clone_link = "<a class=warn href=\"javascript:clone()\" "
                    + " title='Create a copy of this item'><img src=paste_plain.png border=0 align=top>&nbsp;create copy</a>";
                clone.InnerHtml = clone_link;
            }


            if (permission_level != PermissionLevel.ReadOnly)
            {
                string attachment_link = "<img src=attach.gif align=top>&nbsp;<a href=\"javascript:open_popup_window('add_attachment.aspx','add attachment ',"
                    + Convert.ToString(id)
                    + ",600,300)\" title='Attach an image, document, or other file to this item'>add attachment</a>";
                attachment.InnerHtml = attachment_link;
            }
            else
            {
                attachment.Visible = false;
            }


            if (!User.IsInRole(BtnetRoles.Guest))
            {
                if (permission_level != PermissionLevel.ReadOnly)
                {
                    string send_email_link = "<a href='javascript:send_email("
                        + Convert.ToString(id)
                        + ")' title='Send an email about this item'><img src=email_edit.png border=0 align=top>&nbsp;send email</a>";
                    send_email.InnerHtml = send_email_link;
                }
                else
                {
                    send_email.Visible = false;
                }

            }
            else
            {
                send_email.Visible = false;
            }

            if (permission_level != PermissionLevel.ReadOnly)
            {
                string subscribers_link = "<a target=_blank href=view_subscribers.aspx?id="
                    + Convert.ToString(id)
                    + " title='View users who have subscribed to email notifications for this item'><img src=telephone_edit.png border=0 align=top>&nbsp;subscribers</a>";
                subscribers.InnerHtml = subscribers_link;
            }
            else
            {
                subscribers.Visible = false;
            }

            if (Util.get_setting("EnableRelationships", "0") == "1")
            {
                int relationship_cnt = 0;
                if (id != 0)
                {
                    relationship_cnt = (int)dr_bug["relationship_cnt"];
                }
                string relationships_link = "<a target=_blank href=relationships.aspx?bgid="
                    + Convert.ToString(id)
                    + " title='Create a relationship between this item and another item'><img src=database_link.png border=0 align=top>&nbsp;relationships(<span id=relationship_cnt>" + relationship_cnt + "</span>)</a>";
                relationships.InnerHtml = relationships_link;
            }
            else
            {
                relationships.Visible = false;
            }

            if (Util.get_setting("EnableSubversionIntegration", "0") == "1")
            {
                int revision_cnt = 0;
                if (id != 0)
                {
                    revision_cnt = (int)dr_bug["svn_revision_cnt"];
                }
                string svn_revisions_link = "<a target=_blank href=svn_view_revisions.aspx?id="
                    + Convert.ToString(id)
                + " title='View Subversion svn_revisions related to this item'><img src=svn.png border=0 align=top>&nbsp;svn revisions(" + revision_cnt + ")</a>";
                svn_revisions.InnerHtml = svn_revisions_link;
            }
            else
            {
                svn_revisions.Visible = false;
            }

            if (Util.get_setting("EnableGitIntegration", "0") == "1")
            {
                int revision_cnt = 0;
                if (id != 0)
                {
                    revision_cnt = (int)dr_bug["git_commit_cnt"];
                }
                string git_commits_link = "<a target=_blank href=git_view_revisions.aspx?id="
                    + Convert.ToString(id)
                + " title='View git git_commits related to this item'><img src=git.png border=0 align=top>&nbsp;git commits(" + revision_cnt + ")</a>";
                git_commits.InnerHtml = git_commits_link;
            }
            else
            {
                git_commits.Visible = false;
            }

            if (Util.get_setting("EnableMercurialIntegration", "0") == "1")
            {
                int revision_cnt = 0;
                if (id != 0)
                {
                    revision_cnt = (int)dr_bug["hg_commit_cnt"];
                }
                string hg_revisions_link = "<a target=_blank href=hg_view_revisions.aspx?id="
                    + Convert.ToString(id)
                + " title='View mercurial git_hg_revisions related to this item'><img src=hg.png border=0 align=top>&nbsp;hg revisions(" + revision_cnt + ")</a>";
                hg_revisions.InnerHtml = hg_revisions_link;
            }
            else
            {
                hg_revisions.Visible = false;
            }


            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanViewTasks())
            {
                if (Util.get_setting("EnableTasks", "0") == "1")
                {
                    int task_cnt = 0;
                    if (id != 0)
                    {
                        task_cnt = (int)dr_bug["task_cnt"];
                    }
                    string tasks_link = "<a target=_blank href=tasks_frame.aspx?bugid="
                        + Convert.ToString(id)
                    + " title='View sub-tasks/time-tracking entries related to this item'><img src=clock.png border=0 align=top>&nbsp;tasks/time(<span id=task_cnt>" + task_cnt + "</span>)</a>";
                    tasks.InnerHtml = tasks_link;
                }
                else
                {
                    tasks.Visible = false;
                }
            }
            else
            {
                tasks.Visible = false;
            }

            format_subcribe_cancel_link();


            print.InnerHtml = "<a target=_blank href=print_bug.aspx?id="
                + Convert.ToString(id)
                + " title='Display this item in a printer-friendly format'><img src=printer.png border=0 align=top>&nbsp;print</a>";


            // merge
            if (!User.IsInRole(BtnetRoles.Guest))
            {
                if (User.IsInRole(BtnetRoles.Admin)
                || User.Identity.GetCanMergeBugs())
                {
                    string merge_bug_link = "<a href=merge_bug.aspx?id="
                        + Convert.ToString(id)
                        + " title='Merge this item and another item together'><img src=database_refresh.png border=0 align=top>&nbsp;merge</a>";

                    merge_bug.InnerHtml = merge_bug_link;
                }
                else
                {
                    merge_bug.Visible = false;
                }
            }
            else
            {
                merge_bug.Visible = false;
            }

            // delete 
            if (!User.IsInRole(BtnetRoles.Guest))
            {
                if (User.IsInRole(BtnetRoles.Admin)
                || User.Identity.GetCanDeleteBugs())
                {
                    string delete_bug_link = "<a href=delete_bug.aspx?id="
                        + Convert.ToString(id)
                        + " title='Delete this item'><img src=delete.png border=0 align=top>&nbsp;delete</a>";

                    delete_bug.InnerHtml = delete_bug_link;
                }
                else
                {
                    delete_bug.Visible = false;
                }
            }
            else
            {
                delete_bug.Visible = false;
            }

            // custom bug link
            if (Util.get_setting("CustomBugLinkLabel", "") != "")
            {
                string custom_bug_link = "<a href="
                    + Util.get_setting("CustomBugLinkUrl", "")
                    + "?bugid="
                    + Convert.ToString(id)
                    + "><img src=brick.png border=0 align=top>&nbsp;"
                    + Util.get_setting("CustomBugLinkLabel", "")
                    + "</a>";

                custom.InnerHtml = custom_bug_link;
            }
            else
            {
                custom.Visible = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void load_dropdowns_for_update()
        {

            load_dropdowns();

            // select the dropdowns

            foreach (ListItem li in category.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)dr_bug["category"])
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }

            foreach (ListItem li in priority.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)dr_bug["priority"])
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }

            foreach (ListItem li in status.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)dr_bug["status"])
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }

            foreach (ListItem li in udf.Items)
            {
                if (Convert.ToInt32(li.Value) == (int)dr_bug["udf"])
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }

            // special logic for org
            if (id != 0)
            {
                // Org
                if (prev_org.Value != "0")
                {
                    bool already_in_dropdown = false;
                    foreach (ListItem li in org.Items)
                    {
                        if (li.Value == prev_org.Value)
                        {
                            already_in_dropdown = true;
                            break;
                        }
                    }

                    // Add to the list, even if permissions don't allow it now, because, in the past, they did allow it.
                    if (!already_in_dropdown)
                    {
                        org.Items.Add(
                            new ListItem(
                                prev_org_name.Value,
                                prev_org.Value));

                    }
                }

                foreach (ListItem li in org.Items)
                {
                    if (li.Value == prev_org.Value)
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

        ///////////////////////////////////////////////////////////////////////
        void display_bugid_must_be_integer()
        {
            // Display an error because the bugid must be an integer

            Response.Write("<link rel=StyleSheet href=btnet.css type=text/css>");
            Response.Write("<p>&nbsp;</p><div class=align>");
            Response.Write("<div class=err>Error: ");
            Response.Write(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")));
            Response.Write(" ID must be an integer.</div>");
            Response.Write("<p><a href=bugs.aspx>View ");
            Response.Write(Util.get_setting("PluralBugLabel", "bugs"));
            Response.Write("</a>");
            Response.End();

        }



        ///////////////////////////////////////////////////////////////////////

        string comment_formated;
        string comment_search;
        string commentType;

        ///////////////////////////////////////////////////////////////////////

        void get_comment_text_from_control()
        {
            if (User.Identity.GetUseFCKEditor())
            {
                comment_formated = Util.strip_dangerous_tags(comment.Value);
                comment_search = Util.strip_html(comment.Value);
                commentType = "text/html";
            }
            else
            {
                comment_formated = HttpUtility.HtmlDecode(comment.Value);
                comment_search = comment_formated;
                commentType = "text/plain";
            }

        }

        ///////////////////////////////////////////////////////////////////////

        void load_incoming_custom_col_vals_into_hash()
        {
            // Fetch the values of the custom columns from the Request and stash them in a hash table.


        }

        ///////////////////////////////////////////////////////////////////////
        void do_insert()
        {

            get_comment_text_from_control();



            Bug.NewIds new_ids = Bug.insert_bug(
                short_desc.Value,
                User.Identity,
                tags.Value,
                Convert.ToInt32(project.SelectedItem.Value),
                Convert.ToInt32(org.SelectedItem.Value),
                Convert.ToInt32(category.SelectedItem.Value),
                Convert.ToInt32(priority.SelectedItem.Value),
                Convert.ToInt32(status.SelectedItem.Value),
                Convert.ToInt32(assigned_to.SelectedItem.Value),
                Convert.ToInt32(udf.SelectedItem.Value),
                comment_formated,
                comment_search,
                null, // from
                null, // cc
                commentType,
                internal_only.Checked,
                hash_custom_cols,
                true); // send notifications

            if (tags.Value != "" && Util.get_setting("EnableTags", "0") == "1")
            {
                Tags.build_tag_index(Application);
            }

            id = new_ids.bugid;

            WhatsNew.add_news(id, short_desc.Value, "added", User.Identity);

            new_id.Value = Convert.ToString(id);
            set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")) + " was created.");

            // save for next bug
            Session["project"] = project.SelectedItem.Value;

            Response.Redirect("edit_bug.aspx?id=" + Convert.ToString(id));

        }

        ///////////////////////////////////////////////////////////////////////
        void do_update()
        {

            permission_level = fetch_permission_level(project.SelectedItem.Value);

            //if (project.SelectedItem.Value == prev_project.Value)
            //{
            //    set_controls_field_permission(permission_level);
            //}

            bool bug_fields_have_changed = false;
            bool bugpost_fields_have_changed = false;

            get_comment_text_from_control();

            string new_project;
            if (project.SelectedItem.Value != prev_project.Value)
            {
                new_project = project.SelectedItem.Value;
                int permission_level_on_new_project = fetch_permission_level(new_project);
                if (PermissionLevel.None == permission_level_on_new_project
                || PermissionLevel.ReadOnly == permission_level_on_new_project)
                {
                    set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))
                        + " was not updated. You do not have the necessary permissions to change this "
                        + Util.get_setting("SingularBugLabel", "bug") + " to the specified Project.");
                    return;
                }
                permission_level = permission_level_on_new_project;
            }
            else
            {
                new_project = Util.sanitize_integer(prev_project.Value);
            }

            sql = new SQLString(@"declare @now datetime
        declare @last_updated datetime
        select @last_updated = bg_last_updated_date from bugs where bg_id = @id
        if @last_updated > @snapshot_datetime
        begin
            -- signal that we did NOT do the update
            set @now = @snapshot_datetime
        end
        else
        begin
            -- signal that we DID do the update
            set @now = getdate()

            update bugs set
            bg_short_desc = @sd,
            bg_tags = @tags,
            bg_project = @pj,
            bg_org = @og,
            bg_category = @ct,
            bg_priority = @pr,
            bg_assigned_to_user = @au,
            bg_status = @st,
            bg_last_updated_user = @lu,
            bg_last_updated_date = @now,
            bg_user_defined_attribute = @udf
            where bg_id = @id
        end
        select @now");

            sql = sql.AddParameterWithValue("sd", short_desc.Value);
            sql = sql.AddParameterWithValue("tags", tags.Value);
            sql = sql.AddParameterWithValue("lu", Convert.ToString(User.Identity.GetUserId()));
            sql = sql.AddParameterWithValue("id", Convert.ToString(id));
            sql = sql.AddParameterWithValue("pj", new_project);
            sql = sql.AddParameterWithValue("og", org.SelectedItem.Value);
            sql = sql.AddParameterWithValue("ct", category.SelectedItem.Value);
            sql = sql.AddParameterWithValue("pr", priority.SelectedItem.Value);
            sql = sql.AddParameterWithValue("au", assigned_to.SelectedItem.Value);
            sql = sql.AddParameterWithValue("st", status.SelectedItem.Value);
            sql = sql.AddParameterWithValue("udf", udf.SelectedItem.Value);
            sql = sql.AddParameterWithValue("snapshot_datetime", snapshot_timestamp.Value);

            DateTime last_update_date = (DateTime)DbUtil.execute_scalar(sql);

            WhatsNew.add_news(id, short_desc.Value, "updated", User.Identity);

            string date_from_db = last_update_date.ToString("yyyyMMdd HH\\:mm\\:ss\\:fff");
            string date_from_webpage = snapshot_timestamp.Value;

            if (date_from_db != date_from_webpage)
            {
                snapshot_timestamp.Value = date_from_db;
                Bug.auto_subscribe(id);
                format_subcribe_cancel_link();
                bug_fields_have_changed = record_changes();
            }
            else
            {
                set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug"))
                    + " was NOT updated.<br>"
                    + " Somebody changed it while you were editing it.<br>"
                    + " Click <a href=edit_bug.aspx?id="
                    + Convert.ToString(id)
                    + ">[here]</a> to refresh the page and discard your changes.<br>");
                return;
            }


            bugpost_fields_have_changed = (Bug.insert_comment(
                id,
                User.Identity.GetUserId(),
                comment_formated,
                comment_search,
                null, // from
                null, // cc
                commentType,
                internal_only.Checked) != 0);

            if (bug_fields_have_changed || (bugpost_fields_have_changed && !internal_only.Checked))
            {
                Bug.send_notifications(Bug.UPDATE, id, User.Identity, 0,
                    status_changed,
                    assigned_to_changed,
                    Convert.ToInt32(assigned_to.SelectedItem.Value));

            }

            set_msg(Util.capitalize_first_letter(Util.get_setting("SingularBugLabel", "bug")) + " was updated.");

            comment.Value = "";

            set_controls_field_permission(permission_level);

            if (bug_fields_have_changed)
            {
                // Fetch again from database
                DataRow updated_bug = Bug.get_bug_datarow(id, User.Identity);

                // Allow for customization not written by me
                Workflow.custom_adjust_controls(updated_bug, User.Identity, this);
            }

            load_user_dropdown();

        }


        ///////////////////////////////////////////////////////////////////////
        void load_other_dropdowns_and_select_defaults(DataSet ds_defaults)
        {

            // org
            string default_value;

            default_value = Convert.ToString(User.Identity.GetOrganizationId());
            foreach (ListItem li in org.Items)
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


            // category
            if (ds_defaults.Tables[1].Rows.Count > 0)
            {
                default_value = Convert.ToString((int)ds_defaults.Tables[1].Rows[0][0]);
            }
            else
            {
                default_value = "0";
            }

            foreach (ListItem li in category.Items)
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


            // priority
            if (ds_defaults.Tables[2].Rows.Count > 0)
            {
                default_value = Convert.ToString((int)ds_defaults.Tables[2].Rows[0][0]);
            }
            else
            {
                default_value = "0";
            }
            foreach (ListItem li in priority.Items)
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


            // status
            if (ds_defaults.Tables[3].Rows.Count > 0)
            {
                default_value = Convert.ToString((int)ds_defaults.Tables[3].Rows[0][0]);
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


            // udf
            if (ds_defaults.Tables[4].Rows.Count > 0)
            {
                default_value = Convert.ToString((int)ds_defaults.Tables[4].Rows[0][0]);
            }
            else
            {
                default_value = "0";
            }
            foreach (ListItem li in udf.Items)
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


        ///////////////////////////////////////////////////////////////////////
        void load_project_and_user_dropdown_for_insert(DataTable project_default)
        {

            // get default values
            string initial_project = (string)Session["project"];

            // project
            int forcedProjectId = User.Identity.GetForcedProjectId();

            if (forcedProjectId != 0)
            {
                initial_project = Convert.ToString(forcedProjectId);
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
                string default_value;
                if (project_default.Rows.Count > 0)
                {
                    default_value = Convert.ToString((int)project_default.Rows[0][0]);
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


            load_user_dropdown();

        }

        ///////////////////////////////////////////////////////////////////////
        void load_project_and_user_dropdown_for_update()
        {
            // Project
            if (prev_project.Value != "0")
            {
                // see if already in the dropdown.
                bool already_in_dropdown = false;
                foreach (ListItem li in project.Items)
                {
                    if (li.Value == prev_project.Value)
                    {
                        already_in_dropdown = true;
                        break;
                    }
                }

                // Add to the list, even if permissions don't allow it now, because, in the past, they did allow it.
                if (!already_in_dropdown)
                {
                    project.Items.Add(
                        new ListItem(
                            prev_project_name.Value,
                            prev_project.Value));

                }

            }

            foreach (ListItem li in project.Items)
            {
                if (li.Value == prev_project.Value)
                {
                    li.Selected = true;
                }
                else
                {
                    li.Selected = false;
                }
            }

            load_user_dropdown();

        }


        ///////////////////////////////////////////////////////////////////////
        void load_user_dropdown()
        {
            // What's selected now?   Save it before we refresh the dropdown.
            string current_value = "";

            if (IsPostBack)
            {
                current_value = assigned_to.SelectedItem.Value;
            }


            // Load the user dropdown, which changes per project
            // Only users explicitly allowed will be listed
            if (Util.get_setting("DefaultPermissionLevel", "2") == "0")
            {
                sql = new SQLString(@"
/* users this project */ select us_id, case when 1 = @fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
from users 
inner join orgs on us_org = og_id 
where us_active = 1 
and og_can_be_assigned_to = 1 
and (@og_other_orgs_permission_level <> 0 or @og_id = og_id or (og_external_user = 0 and 1 = @og_can_assign_to_internal_users))
and us_id in
    (select pu_user from project_user_xref
        where pu_project = @pj
        and pu_permission_level <> 0)
order by us_username; ");
            }
            // Only users explictly DISallowed will be omitted
            else
            {
                sql = new SQLString(@"
/* users this project */ select us_id, case when 1 = @fullnames then us_lastname + ', ' + us_firstname else us_username end us_username
from users 
inner join orgs on us_org = og_id
where us_active = 1
and og_can_be_assigned_to = 1
and (@og_other_orgs_permission_level <> 0 or @og_id = og_id or (og_external_user = 0 and 1 = @og_can_assign_to_internal_users))
and us_id not in
    (select pu_user from project_user_xref
        where pu_project = @pj
        and pu_permission_level = 0)
order by us_username; ");
            }

            if (Util.get_setting("UseFullNames", "0") == "0")
            {
                // false condition
                sql = sql.AddParameterWithValue("fullnames", "0");
            }
            else
            {
                // true condition
                sql = sql.AddParameterWithValue("fullnames", "1");
            }

            if (project.SelectedItem != null)
            {
                sql = sql.AddParameterWithValue("@pj", project.SelectedItem.Value);
            }
            else
            {
                sql = sql.AddParameterWithValue("@pj", "0");
            }


            sql = sql.AddParameterWithValue("@og_id", User.Identity.GetOrganizationId());
            sql = sql.AddParameterWithValue("@og_other_orgs_permission_level", User.Identity.GetOtherOrgsPermissionLevels());

            if (User.Identity.GetCanAssignToInternalUsers())
            {
                sql = sql.AddParameterWithValue("@og_can_assign_to_internal_users", 1);
            }
            else
            {
                sql = sql.AddParameterWithValue("@og_can_assign_to_internal_users", 0);
            }

            dt_users = DbUtil.get_dataset(sql).Tables[0];

            assigned_to.DataSource = new DataView((DataTable)dt_users);
            assigned_to.DataTextField = "us_username";
            assigned_to.DataValueField = "us_id";
            assigned_to.DataBind();
            assigned_to.Items.Insert(0, new ListItem("[not assigned]", "0"));

            // It can happen that the user in the db is not listed in the dropdown, because of a subsequent change in permissions.
            // Since that user IS the user associated with the bug, let's force it into the dropdown.
            if (id != 0) // if existing bug
            {
                if (prev_assigned_to.Value != "0")
                {
                    // see if already in the dropdown.
                    bool user_in_dropdown = false;
                    foreach (ListItem li in assigned_to.Items)
                    {
                        if (li.Value == prev_assigned_to.Value)
                        {
                            user_in_dropdown = true;
                            break;
                        }
                    }

                    // Add to the list, even if permissions don't allow it now, because, in the past, they did allow it.
                    if (!user_in_dropdown)
                    {
                        assigned_to.Items.Insert(1,
                            new ListItem(
                                prev_assigned_to_username.Value,
                                prev_assigned_to.Value));

                    }
                }
            }

            // At this point, all the users we need are in the dropdown.
            // Now selected the selected.
            if (current_value == "")
            {
                current_value = prev_assigned_to.Value;
            }


            // Select the user.  We are either restoring the previous selection
            // or selecting what was in the database.
            if (current_value != "0")
            {
                foreach (ListItem li in assigned_to.Items)
                {
                    if (li.Value == current_value)
                    {
                        li.Selected = true;
                    }
                    else
                    {
                        li.Selected = false;
                    }
                }
            }

            // if nothing else is selected. select the default user for the project
            if (assigned_to.SelectedItem.Value == "0")
            {
                int project_default_user = 0;
                if (project.SelectedItem != null)
                {
                    // get the default user of the project
                    project_default_user = Util.get_default_user(Convert.ToInt32(project.SelectedItem.Value));

                    if (project_default_user != 0)
                    {
                        foreach (ListItem li in assigned_to.Items)
                        {
                            if (Convert.ToInt32(li.Value) == project_default_user)
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
            }
        }


        ///////////////////////////////////////////////////////////////////////
        string get_custom_col_default_value(object o)
        {
            string defaultval = Convert.ToString(o);

            // populate the sql default value of a custom field
            if (defaultval.Length > 2)
            {
                if (defaultval[0] == '('
                && defaultval[defaultval.Length - 1] == ')')
                {
                    var defaultval_sql = new SQLString("select " + defaultval.Substring(1, defaultval.Length - 2));
                    defaultval = Convert.ToString(DbUtil.execute_scalar(defaultval_sql));
                }
            }

            return defaultval;
        }




        ///////////////////////////////////////////////////////////////////////
        void format_subcribe_cancel_link()
        {

            bool notification_email_enabled = (Util.get_setting("NotificationEmailEnabled", "1") == "1");
            if (notification_email_enabled)
            {
                int subscribed;
                if (!IsPostBack)
                {
                    subscribed = (int)dr_bug["subscribed"];
                }
                else
                {
                    // User might have changed bug to a project where we automatically subscribe
                    // so be prepared to format the link even if this isn't the first time in.
                    sql = new SQLString("select count(1) from bug_subscriptions where bs_bug = @bg and bs_user = @us");
                    sql = sql.AddParameterWithValue("@bg", Convert.ToString(id));
                    sql = sql.AddParameterWithValue("@us", Convert.ToString(User.Identity.GetUserId()));
                    subscribed = (int)DbUtil.execute_scalar(sql);
                }

                if (User.IsInRole(BtnetRoles.Guest)) // wouldn't make sense to share an email address
                {
                    subscriptions.InnerHtml = "";
                }
                else
                {

                    string subscription_link = "<a id='notifications' title='Get or stop getting email notifications about changes to this item.'"
                        + " href='javascript:toggle_notifications("
                        + Convert.ToString(id)
                        + ")'><img src=telephone.png border=0 align=top>&nbsp;<span id='get_stop_notifications'>";

                    if (subscribed > 0)
                    {
                        subscription_link += "stop notifications</span></a>";
                    }
                    else
                    {
                        subscription_link += "get notifications</span></a>";
                    }

                    subscriptions.InnerHtml = subscription_link;
                }
            }

        }


        ///////////////////////////////////////////////////////////////////////
        void set_org_field_permission(int bug_permission_level)
        {
            // pick the most restrictive permission
            int orgFieldPermissionLevel = User.Identity.GetOrgFieldPermissionLevel();
            int perm_level = bug_permission_level < orgFieldPermissionLevel
                ? bug_permission_level : orgFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                org_label.Visible = false;
                org.Visible = false;
                prev_org.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                org.Visible = false;
                static_org.InnerText = org.SelectedItem.Text;
            }
            else // editable
            {
                static_org.Visible = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void set_shortdesc_field_permission()
        {
            // turn on the spans to hold the data
            if (id != 0)
            {
                static_short_desc.Style["display"] = "block";
                short_desc.Visible = false;
            }

            static_short_desc.InnerText = short_desc.Value;

        }


        ///////////////////////////////////////////////////////////////////////
        void set_tags_field_permission(int bug_permission_level)
        {

            /// JUNK testing using cat permission
            // pick the most restrictive permission
            int tagsFieldPermissionLevel = User.Identity.GetTagsFieldPermissionLevel();
            int perm_level = bug_permission_level < tagsFieldPermissionLevel
                ? bug_permission_level : tagsFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                static_tags.Visible = false;
                tags_label.Visible = false;
                tags.Visible = false;
                tags_link.Visible = false;
                prev_tags.Visible = false;
                //tags_row.Style.display = "none";
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                if (id != 0)
                {
                    tags.Visible = false;
                    tags_link.Visible = false;
                    static_tags.Visible = true;
                    static_tags.InnerText = tags.Value;
                }
                else
                {
                    tags_label.Visible = false;
                    tags.Visible = false;
                    tags_link.Visible = false;
                }
            }
            else // editable
            {
                static_tags.Visible = false;
            }

        }


        ///////////////////////////////////////////////////////////////////////
        void set_category_field_permission(int bug_permission_level)
        {
            // pick the most restrictive permission
            var categoryFieldPermissionLevel = User.Identity.GetCategoryFieldPermissionLevel();
            int perm_level = bug_permission_level < categoryFieldPermissionLevel
                ? bug_permission_level : categoryFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                category_label.Visible = false;
                category.Visible = false;
                prev_category.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                category.Visible = false;
                static_category.InnerText = category.SelectedItem.Text;
            }
            else // editable
            {
                static_category.Visible = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void set_priority_field_permission(int bug_permission_level)
        {
            // pick the most restrictive permission
            int priorityFieldPermissionLevel = User.Identity.GetPriorityFieldPermissionLevel();
            int perm_level = bug_permission_level < priorityFieldPermissionLevel
                ? bug_permission_level : priorityFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                priority_label.Visible = false;
                priority.Visible = false;
                prev_priority.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                priority.Visible = false;
                static_priority.InnerText = priority.SelectedItem.Text;
            }
            else // editable
            {
                static_priority.Visible = false;
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void set_status_field_permission(int bug_permission_level)
        {
            // pick the most restrictive permission
            int statusFieldPermissionLevel = User.Identity.GetStatusFieldPermissionLevel();
            int perm_level = bug_permission_level < statusFieldPermissionLevel
                ? bug_permission_level : statusFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                status_label.Visible = false;
                status.Visible = false;
                prev_status.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                status.Visible = false;
                static_status.InnerText = status.SelectedItem.Text;
            }
            else // editable
            {
                static_status.Visible = false;
            }
        }


        ///////////////////////////////////////////////////////////////////////
        void set_project_field_permission(int bug_permission_level)
        {
            int projectFieldPermissionLevel = User.Identity.GetProjectFieldPermissionLevel();
            int perm_level = bug_permission_level < projectFieldPermissionLevel
                ? bug_permission_level : projectFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                project_label.Visible = false;
                project.Visible = false;
                prev_project.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                project.Visible = false;
                static_project.InnerText = project.SelectedItem.Text;
            }
            else
            {
                static_project.Visible = false;
            }
        }


        ///////////////////////////////////////////////////////////////////////
        void set_assigned_field_permission(int bug_permission_level)
        {
            int assignedToFieldPermissionLevel = User.Identity.GetAssignedToFieldPermissionLevel();
            int perm_level = bug_permission_level < assignedToFieldPermissionLevel
                ? bug_permission_level : assignedToFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                assigned_to_label.Visible = false;
                assigned_to.Visible = false;
                prev_assigned_to.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                assigned_to.Visible = false;
                static_assigned_to.InnerText = assigned_to.SelectedItem.Text;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void set_udf_field_permission(int bug_permission_level)
        {
            // pick the most restrictive permission
            int udfFieldPermissionLevel = User.Identity.GetUdfFieldPermissionLevel();
            int perm_level = bug_permission_level < udfFieldPermissionLevel
                ? bug_permission_level : udfFieldPermissionLevel;

            if (perm_level == PermissionLevel.None)
            {
                udf_label.Visible = false;
                udf.Visible = false;
                prev_udf.Visible = false;
            }
            else if (perm_level == PermissionLevel.ReadOnly)
            {
                udf.Visible = false;
                static_udf.InnerText = udf.SelectedItem.Text;
            }
            else // editable
            {
                static_udf.Visible = false;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void set_controls_field_permission(int bug_permission_level)
        {

            if (bug_permission_level == PermissionLevel.ReadOnly
            || bug_permission_level == PermissionLevel.Reporter)
            {
                // even turn off commenting updating for read only
                if (permission_level == PermissionLevel.ReadOnly)
                {
                    submit_button.Disabled = true;
                    submit_button.Visible = false;
                    if (Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
                    {
                        submit_button2.Disabled = true;
                        submit_button2.Visible = false;
                    }

                    comment_label.Visible = false;
                    comment.Visible = false;
                }

                set_project_field_permission(PermissionLevel.ReadOnly);
                set_org_field_permission(PermissionLevel.ReadOnly);
                set_category_field_permission(PermissionLevel.ReadOnly);
                set_tags_field_permission(PermissionLevel.ReadOnly);
                set_priority_field_permission(PermissionLevel.ReadOnly);
                set_status_field_permission(PermissionLevel.ReadOnly);
                set_assigned_field_permission(PermissionLevel.ReadOnly);
                set_udf_field_permission(PermissionLevel.ReadOnly);
                set_shortdesc_field_permission();

                internal_only_label.Visible = false;
                internal_only.Visible = false;
            }
            else
            {
                // Call these functions so that the field level permissions can kick in
                if (User.Identity.GetForcedProjectId() != 0)
                {
                    set_project_field_permission(PermissionLevel.ReadOnly);
                }
                else
                {
                    set_project_field_permission(PermissionLevel.All);
                }

                if (User.Identity.GetOtherOrgsPermissionLevels() == 0)
                {
                    set_org_field_permission(PermissionLevel.ReadOnly);
                }
                else
                {
                    set_org_field_permission(PermissionLevel.All);
                }
                set_category_field_permission(PermissionLevel.All);
                set_tags_field_permission(PermissionLevel.All);
                set_priority_field_permission(PermissionLevel.All);
                set_status_field_permission(PermissionLevel.All);
                set_assigned_field_permission(PermissionLevel.All);
                set_udf_field_permission(PermissionLevel.All);
            }

        }



        ///////////////////////////////////////////////////////////////////////
        void format_prev_next_bug()
        {
            // for next/prev bug links
            DataView dv_bugs = (DataView)Session["bugs"];

            if (dv_bugs != null)
            {
                int prev_bug = 0;
                int next_bug = 0;
                bool this_bug_found = false;

                // read through the list of bugs looking for the one that matches this one
                int position_in_list = 0;
                int save_position_in_list = 0;
                foreach (DataRowView drv in dv_bugs)
                {
                    position_in_list++;
                    if (this_bug_found)
                    {
                        // step 3 - get the next bug - we're done
                        next_bug = (int)drv[1];
                        break;
                    }
                    else if (id == (int)drv[1])
                    {
                        // step 2 - we found this - set switch
                        save_position_in_list = position_in_list;
                        this_bug_found = true;
                    }
                    else
                    {
                        // step 1 - save the previous just in case the next one IS this bug
                        prev_bug = (int)drv[1];
                    }
                }

                string prev_next_link = "";

                if (this_bug_found)
                {
                    if (prev_bug != 0)
                    {
                        prev_next_link =
                            "&nbsp;&nbsp;&nbsp;&nbsp;<a class=warn href=edit_bug.aspx?id="
                            + Convert.ToString(prev_bug)
                            + "><img src=arrow_up.png border=0 align=top>prev</a>";
                    }
                    else
                    {
                        prev_next_link = "&nbsp;&nbsp;&nbsp;&nbsp;<span class=gray_link>prev</span>";
                    }

                    if (next_bug != 0)
                    {
                        prev_next_link +=
                            "&nbsp;&nbsp;&nbsp;&nbsp;<a class=warn href=edit_bug.aspx?id="
                            + Convert.ToString(next_bug)
                            + ">next<img src=arrow_down.png border=0 align=top></a>";

                    }
                    else
                    {
                        prev_next_link += "&nbsp;&nbsp;&nbsp;&nbsp;<span class=gray_link>next</span>";
                    }

                    prev_next_link += "&nbsp;&nbsp;&nbsp;<span class=smallnote>"
                        + Convert.ToString(save_position_in_list)
                        + " of "
                        + Convert.ToString(dv_bugs.Count)
                        + "</span>";

                    prev_next.InnerHtml = prev_next_link;
                }

            }

        }


        ///////////////////////////////////////////////////////////////////////
        void load_dropdowns()
        {

            // only show projects where user has permissions
            // 0
            sql = new SQLString(@"/* drop downs */ select pj_id, pj_name
        from projects
        left outer join project_user_xref on pj_id = pu_project
        and pu_user = @us
        where pj_active = 1
        and isnull(pu_permission_level,@dpl) not in (0, 1)
        order by pj_name;");

            sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
            sql = sql.AddParameterWithValue("dpl", Util.get_setting("DefaultPermissionLevel", "2"));

            // 1
            sql.Append("\nselect og_id, og_name from orgs where og_active = 1 order by og_name;");

            // 2
            sql.Append("\nselect ct_id, ct_name from categories order by ct_sort_seq, ct_name;");

            // 3
            sql.Append("\nselect pr_id, pr_name from priorities order by pr_sort_seq, pr_name;");

            // 4
            sql.Append("\nselect st_id, st_name from statuses order by st_sort_seq, st_name;");

            // 5
            sql.Append("\nselect udf_id, udf_name from user_defined_attribute order by udf_sort_seq, udf_name;");

            // do a batch of sql statements
            DataSet ds_dropdowns = DbUtil.get_dataset(sql);

            project.DataSource = ds_dropdowns.Tables[0];
            project.DataTextField = "pj_name";
            project.DataValueField = "pj_id";
            project.DataBind();

            if (Util.get_setting("DefaultPermissionLevel", "2") == "2")
            {
                project.Items.Insert(0, new ListItem("[no project]", "0"));
            }

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

            udf.DataSource = ds_dropdowns.Tables[5];
            udf.DataTextField = "udf_name";
            udf.DataValueField = "udf_id";
            udf.DataBind();
            udf.Items.Insert(0, new ListItem("[none]", "0"));


        }

        ///////////////////////////////////////////////////////////////////////
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


        ///////////////////////////////////////////////////////////////////////
        bool did_something_change()
        {

            bool something_changed = false;

            if (prev_short_desc.Value != short_desc.Value
            || prev_tags.Value != tags.Value
            || comment.Value.Length > 0
            || clone_ignore_bugid.Value == "1"
            || prev_project.Value != project.SelectedItem.Value
            || prev_org.Value != org.SelectedItem.Value
            || prev_category.Value != category.SelectedItem.Value
            || prev_priority.Value != priority.SelectedItem.Value
            || prev_assigned_to.Value != assigned_to.SelectedItem.Value
            || prev_status.Value != status.SelectedItem.Value
            || (Util.get_setting("ShowUserDefinedBugAttribute", "1") == "1" &&
                prev_udf.Value != udf.SelectedItem.Value))
            {
                clone_ignore_bugid.Value = "0";
                something_changed = true;
            }


            // Now look to see if custom fields changed
            if (!something_changed)
            {
                foreach (string column_name in hash_custom_cols.Keys)
                {
                    string after = hash_custom_cols[column_name];
                    if (after == null)
                    {
                        continue; // because there's no control, nothing for user to edit
                    }

                    string before = Util.format_db_value(dr_bug[column_name]);

                    if (before != after.Trim())
                    {
                        something_changed = true;
                        break;
                    }

                }
            }

            if (!something_changed)
            {
                if ((Request["pcd1"] != null && prev_pcd1.Value != Request["pcd1"])
                || (Request["pcd2"] != null && prev_pcd2.Value != Request["pcd2"])
                || (Request["pcd3"] != null && prev_pcd3.Value != Request["pcd3"]))
                {
                    something_changed = true;
                }
            }

            return something_changed;

        }

        void AddChange(string field, string oldValue, string newValue)
        {
            var sql = new SQLString(@"
        insert into bug_posts
        (bp_bug, bp_user, bp_date, bp_comment, bp_type)
        values(@id, @us, getdate(), 'Changed ' + @field + ' from ' + @oldValue + ' to ' + @newValue, 'update'");

            sql.AddParameterWithValue("id", Convert.ToString(id));
            sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
            sql.AddParameterWithValue("field", field);
            sql.AddParameterWithValue("oldValue", oldValue);
            sql.AddParameterWithValue("newValue", newValue);
            DbUtil.execute_nonquery(sql);
        }

        ///////////////////////////////////////////////////////////////////////
        // returns true if there was a change
        bool record_changes()
        {
            bool areChanges = false;
            if (Util.get_setting("TrackBugHistory", "1") != "1")
                return true;

            string from;
            sql = new SQLString("");

            if (prev_short_desc.Value != short_desc.Value)
            {
                AddChange("desc", prev_short_desc.Value, short_desc.Value);
                prev_short_desc.Value = short_desc.Value;
                areChanges = true;
            }

            if (prev_tags.Value != tags.Value)
            {
                AddChange("tags", prev_tags.Value, tags.Value);

                prev_tags.Value = tags.Value;

                if (Util.get_setting("EnableTags", "0") == "1")
                {
                    Tags.build_tag_index(Application);
                }
                areChanges = true;
            }

            if (project.SelectedItem.Value != prev_project.Value)
            {
                AddChange("project", prev_project_name.Value, project.SelectedItem.Text);
                prev_project.Value = project.SelectedItem.Value;
                prev_project_name.Value = project.SelectedItem.Text;
                areChanges = true;
            }

            if (prev_org.Value != org.SelectedItem.Value)
            {
                from = get_dropdown_text_from_value(org, prev_org.Value);
                AddChange("organization", from, org.SelectedItem.Text);
                prev_org.Value = org.SelectedItem.Value;
                areChanges = true;
            }

            if (prev_category.Value != category.SelectedItem.Value)
            {
                from = get_dropdown_text_from_value(category, prev_category.Value);
                AddChange("category", from, category.SelectedItem.Text);
                prev_category.Value = category.SelectedItem.Value;
                areChanges = true;
            }

            if (prev_priority.Value != priority.SelectedItem.Value)
            {

                from = get_dropdown_text_from_value(priority, prev_priority.Value);
                AddChange("priority", from, priority.SelectedItem.Text);
                prev_priority.Value = priority.SelectedItem.Value;
                areChanges = true;
            }


            if (prev_assigned_to.Value != assigned_to.SelectedItem.Value)
            {

                assigned_to_changed = true; // for notifications

                AddChange("assigned to", prev_assigned_to_username.Value, assigned_to.SelectedItem.Text);

                prev_assigned_to.Value = assigned_to.SelectedItem.Value;
                prev_assigned_to_username.Value = assigned_to.SelectedItem.Text;
                areChanges = true;
            }


            if (prev_status.Value != status.SelectedItem.Value)
            {
                status_changed = true; // for notifications

                from = get_dropdown_text_from_value(status, prev_status.Value);

                AddChange("status", from, status.SelectedItem.Text);
                prev_status.Value = status.SelectedItem.Value;
                areChanges = true;
            }

            if (Util.get_setting("ShowUserDefinedBugAttribute", "1") == "1")
            {
                if (prev_udf.Value != udf.SelectedItem.Value)
                {

                    from = get_dropdown_text_from_value(udf, prev_udf.Value);

                    AddChange(Util.get_setting("UserDefinedBugAttributeName", "YOUR ATTRIBUTE"), from, udf.SelectedItem.Text);
                    prev_udf.Value = udf.SelectedItem.Value;
                    areChanges = true;
                }
            }




            // Handle project custom dropdowns
            if (Request["label_pcd1"] != null && Request["pcd1"] != null && prev_pcd1.Value != Request["pcd1"])
            {
                AddChange(Request["label_pcd1"], prev_pcd1.Value, Request["pcd1"]);
                prev_pcd1.Value = Request["pcd1"];
                areChanges = true;
            }
            if (Request["label_pcd2"] != null && Request["pcd2"] != null && prev_pcd2.Value != Request["pcd2"].Replace("'", "''"))
            {
                AddChange(Request["label_pcd2"], prev_pcd2.Value, Request["pcd2"]);
                areChanges = true;
                prev_pcd2.Value = Request["pcd2"];
            }
            if (Request["label_pcd3"] != null && Request["pcd3"] != null && prev_pcd3.Value != Request["pcd3"])
            {
                AddChange(Request["label_pcd3"], prev_pcd3.Value, Request["pcd3"]);
                areChanges = true;
                prev_pcd3.Value = Request["pcd3"];
            }

            if (project.SelectedItem.Value != prev_project.Value)
            {
                permission_level = fetch_permission_level(project.SelectedItem.Value);
            }

            // return true if something did change
            return areChanges;
        }

        ///////////////////////////////////////////////////////////////////////
        int fetch_permission_level(string projectToCheck)
        {

            // fetch the revised permission level
            sql = new SQLString(@"declare @permission_level int
        set @permission_level = -1
        select @permission_level = isnull(pu_permission_level,@dpl)
        from project_user_xref
        where pu_project = @pj
        and pu_user = @us
        if @permission_level = -1 set @permission_level = @dpl
        select @permission_level");

            sql = sql.AddParameterWithValue("dpl", Util.get_setting("DefaultPermissionLevel", "2"));
            sql = sql.AddParameterWithValue("pj", projectToCheck);
            sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
            int pl = (int)DbUtil.execute_scalar(sql);

            // reduce permissions for guest
            //if (security.user.is_guest && permission_level == PermissionLevel.All)
            //{
            //	pl = PermissionLevel.Reporter;
            //}

            return pl;

        }
        ///////////////////////////////////////////////////////////////////////
        bool validate()
        {

            bool good = true;
            custom_validation_err_msg.InnerText = "";

            if (short_desc.Value == "")
            {
                good = false;
                short_desc_err.InnerText = "Short Description is required.";
            }
            else
            {
                short_desc_err.InnerText = "";
            }


            if (!did_something_change())
            {
                return false;
            }

            // validate assigned to user versus 

            if (!does_assigned_to_have_permission_for_org(
                Convert.ToInt32(assigned_to.SelectedValue),
                Convert.ToInt32(org.SelectedValue)))
            {
                assigned_to_err.InnerText = "User does not have permission for the Organization";
                good = false;
            }
            else
            {
                assigned_to_err.InnerText = "";
            }


            // custom validations go here
            if (!Workflow.custom_validations(
                dr_bug,
                User.Identity,
                this,
                custom_validation_err_msg))
            {
                good = false;
            }

            return good;
        }

        ///////////////////////////////////////////////////////////////////////
        bool does_assigned_to_have_permission_for_org(int assigned_to, int org)
        {
            if (assigned_to < 1)
            {
                return true;
            }

            var sql = new SQLString(@"
/* validate org versus assigned_to */
select case when og_other_orgs_permission_level <> 0
or @bg_org = og_id then 1
else 0 end as [answer]
from users
inner join orgs on us_org = og_id
where us_id = @us_id");

            sql = sql.AddParameterWithValue("us_id", Convert.ToString(assigned_to));
            sql = sql.AddParameterWithValue("bg_org", Convert.ToString(org));

            object allowed = DbUtil.execute_scalar(sql);

            if (allowed != null && Convert.ToInt32(allowed) == 1)
                return true;
            else
                return false;
        }


        ///////////////////////////////////////////////////////////////////////
        void set_msg(string s)
        {
            msg.InnerHtml = s;
            if (Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
            {
                msg2.InnerHtml = s;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void set_custom_field_msg(string s)
        {
            custom_field_msg.InnerHtml = s;
            if (Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
            {
                custom_field_msg2.InnerHtml = s;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        void append_custom_field_msg(string s)
        {
            custom_field_msg.InnerHtml += s;
            if (Util.get_setting("DisplayAnotherButtonInEditBugPage", "0") == "1")
            {
                custom_field_msg2.InnerHtml += s;
            }
        }

        ///////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        protected void display_project_specific_custom_fields()
        {

            // create project custom dropdowns
            if (project.SelectedItem != null
            && project.SelectedItem.Value != null
            && project.SelectedItem.Value != "0")
            {
                sql = new SQLString(@"select
            isnull(pj_enable_custom_dropdown1,0) [pj_enable_custom_dropdown1],
            isnull(pj_enable_custom_dropdown2,0) [pj_enable_custom_dropdown2],
            isnull(pj_enable_custom_dropdown3,0) [pj_enable_custom_dropdown3],
            isnull(pj_custom_dropdown_label1,'') [pj_custom_dropdown_label1],
            isnull(pj_custom_dropdown_label2,'') [pj_custom_dropdown_label2],
            isnull(pj_custom_dropdown_label3,'') [pj_custom_dropdown_label3],
            isnull(pj_custom_dropdown_values1,'') [pj_custom_dropdown_values1],
            isnull(pj_custom_dropdown_values2,'') [pj_custom_dropdown_values2],
            isnull(pj_custom_dropdown_values3,'') [pj_custom_dropdown_values3]
            from projects where pj_id = @pj");

                sql = sql.AddParameterWithValue("pj", project.SelectedItem.Value);

                DataRow project_dr = DbUtil.get_datarow(sql);


                if (project_dr != null)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        if ((int)project_dr["pj_enable_custom_dropdown" + Convert.ToString(i)] == 1)
                        {
                            // GC: 20-Feb-08: Modified to add an ID to each custom row for CSS customisation
                            Response.Write("\n<tr id=\"pcdrow" + Convert.ToString(i) + "\"><td nowrap>");

                            Response.Write("<span id=label_pcd" + Convert.ToString(i) + ">");
                            Response.Write(project_dr["pj_custom_dropdown_label" + Convert.ToString(i)]);
                            Response.Write("</span>");
                            // End GC
                            Response.Write("<td nowrap>");


                            int permission_on_original = permission_level;
                            if ((prev_project.Value != string.Empty) && (project.SelectedItem.Value != prev_project.Value))
                            {
                                permission_on_original = fetch_permission_level(prev_project.Value);
                            }

                            if (permission_on_original == PermissionLevel.ReadOnly
                            || permission_on_original == PermissionLevel.Reporter)
                            {
                                // GC: 20-Feb-08: Modified to add an ID to the SPAN as well for easier CSS customisation
                                //Response.Write ("<span class="stat">");
                                Response.Write("<span class='stat' id=span_pcd" + Convert.ToString(i) + ">");

                                if (IsPostBack)
                                {
                                    string val = HttpUtility.HtmlEncode(Request["pcd" + Convert.ToString(i)]);

                                    Response.Write(val);
                                    Response.Write("</span>");

                                    Response.Write("<input type=hidden name=pcd"
                                        + Convert.ToString(i)
                                        + " value=\""
                                        + val
                                        + "\">");

                                }
                                else
                                {
                                    if (id != 0)
                                    {
                                        string val = (string)dr_bug["bg_project_custom_dropdown_value" + Convert.ToString(i)];
                                        Response.Write(val);
                                        Response.Write("</span>");

                                        Response.Write("<input type=hidden name=pcd"
                                            + Convert.ToString(i)
                                            + " value=\""
                                            + val
                                            + "\">");

                                    }
                                    else
                                    {
                                        Response.Write("</span>");
                                    }
                                }


                            }
                            else
                            {

                                // create a hidden area to carry the label

                                Response.Write("<input type=hidden");
                                Response.Write(" name=label_pcd" + Convert.ToString(i));
                                Response.Write(" value=\"");
                                Response.Write(project_dr["pj_custom_dropdown_label" + Convert.ToString(i)]);
                                Response.Write("\">");

                                // create a dropdown

                                Response.Write("<select");
                                // GC: 20-Feb-08: Added an ID as well for easier CSS customisation
                                Response.Write(" name=pcd" + Convert.ToString(i));
                                Response.Write(" id=pcd" + Convert.ToString(i) + ">");
                                string[] options = Util.split_dropdown_vals(
                                    (string)project_dr["pj_custom_dropdown_values" + Convert.ToString(i)]);

                                string selected_value = "";

                                if (IsPostBack)
                                {
                                    selected_value = Request["pcd" + Convert.ToString(i)];
                                }
                                else
                                {
                                    // first time viewing existing
                                    if (id != 0)
                                    {
                                        selected_value = (string)dr_bug["bg_project_custom_dropdown_value" + Convert.ToString(i)];
                                    }
                                }

                                for (int j = 0; j < options.Length; j++)
                                {
                                    Response.Write("<option value=\"" + options[j] + "\"");

                                    //if (options[j] == selected_value)
                                    if (HttpUtility.HtmlDecode(options[j]) == selected_value)
                                    {
                                        Response.Write(" selected ");
                                    }
                                    Response.Write(">");
                                    Response.Write(options[j]);
                                }

                                Response.Write("</select>");
                            }
                        }
                    }
                } // if result set not null
            }
        }

        ///////////////////////////////////////////////////////////////////////
        protected void display_bug_relationships()
        {

            ds_posts = PrintBug.get_bug_posts(id, User.Identity.GetIsExternalUser(), history_inline);
            string link_marker = Util.get_setting("BugLinkMarker", "bugid#");
            Regex reLinkMarker = new Regex(link_marker + "([0-9]+)");
            SortedDictionary<int, int> dict_linked_bugs = new SortedDictionary<int, int>();

            // fish out bug links
            foreach (DataRow dr_post in ds_posts.Tables[0].Rows)
            {
                if ((string)dr_post["bp_type"] == "comment")
                {
                    MatchCollection match_collection = reLinkMarker.Matches((string)dr_post["bp_comment"]);

                    foreach (Match match in match_collection)
                    {
                        int other_bugid = Convert.ToInt32(match.Groups[1].ToString());
                        if (other_bugid != id)
                        {
                            dict_linked_bugs[other_bugid] = 1;
                        }
                    }
                }
            }

            if (dict_linked_bugs.Count > 0)
            {
                Response.Write("Linked to:");
                foreach (int int_other_bugid in dict_linked_bugs.Keys)
                {
                    string string_other_bugid = Convert.ToString(int_other_bugid);

                    Response.Write("&nbsp;<a href=edit_bug.aspx?id=");
                    Response.Write(string_other_bugid);
                    Response.Write(">");
                    Response.Write(string_other_bugid);
                    Response.Write("</a>");
                }
            }
        }

    }
}
