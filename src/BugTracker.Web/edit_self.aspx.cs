using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using btnet.Security;

namespace btnet
{
    public partial class edit_self : BasePage
    {
        int id;
        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit your settings";

            msg.InnerText = "";

            id = User.Identity.GetUserId();

            if (!IsPostBack)
            {


                sql = new SQLString(@"declare @org int
			select @org = us_org from users where us_id = @us

			select qu_id, qu_desc
			from queries
			where (isnull(qu_user,0) = 0 and isnull(qu_org,0) = 0)
			or isnull(qu_user,0) = @us
			or isnull(qu_org,0) = @org
			order by qu_desc");

                sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));

                query.DataSource = btnet.DbUtil.get_dataview(sql);
                query.DataTextField = "qu_desc";
                query.DataValueField = "qu_id";
                query.DataBind();


                sql = new SQLString(@"select pj_id, pj_name, isnull(pu_auto_subscribe,0) [pu_auto_subscribe]
			from projects
			left outer join project_user_xref on pj_id = pu_project and @us = pu_user
			where isnull(pu_permission_level,@dpl) <> 0
			order by pj_name");

                sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));
                sql = sql.AddParameterWithValue("dpl", Util.get_setting("DefaultPermissionLevel", "2"));

                DataView projects_dv = btnet.DbUtil.get_dataview(sql);

                project_auto_subscribe.DataSource = projects_dv;
                project_auto_subscribe.DataTextField = "pj_name";
                project_auto_subscribe.DataValueField = "pj_id";
                project_auto_subscribe.DataBind();


                // Get this entry's data from the db and fill in the form
                // MAW -- 2006/01/27 -- Converted to use new notification columns

                sql = new SQLString(@"select
			us_username [username],
			isnull(us_firstname,'') [firstname],
			isnull(us_lastname,'') [lastname],
			isnull(us_bugs_per_page,10) [us_bugs_per_page],
			us_use_fckeditor,
			us_enable_bug_list_popups,
			isnull(us_email,'') [email],
			us_enable_notifications,
			us_send_notifications_to_self,
            us_reported_notifications,
            us_assigned_notifications,
            us_subscribed_notifications,
			us_auto_subscribe,
			us_auto_subscribe_own_bugs,
			us_auto_subscribe_reported_bugs,
			us_default_query,
			isnull(us_signature,'') [signature]
			from users
			where us_id = @id");

                sql = sql.AddParameterWithValue("id", Convert.ToString(id));

                DataRow dr = btnet.DbUtil.get_datarow(sql);

                // Fill in this form
                firstname.Value = (string)dr["firstname"];
                lastname.Value = (string)dr["lastname"];
                bugs_per_page.Value = Convert.ToString(dr["us_bugs_per_page"]);

                if (Util.get_setting("DisableFCKEditor", "0") == "1")
                {
                    use_fckeditor.Visible = false;
                    use_fckeditor_label.Visible = false;
                }

                use_fckeditor.Checked = Convert.ToBoolean((int)dr["us_use_fckeditor"]);
                enable_popups.Checked = Convert.ToBoolean((int)dr["us_enable_bug_list_popups"]);
                email.Value = (string)dr["email"];
                enable_notifications.Checked = Convert.ToBoolean((int)dr["us_enable_notifications"]);
                reported_notifications.Items[(int)dr["us_reported_notifications"]].Selected = true;
                assigned_notifications.Items[(int)dr["us_assigned_notifications"]].Selected = true;
                subscribed_notifications.Items[(int)dr["us_subscribed_notifications"]].Selected = true;
                send_to_self.Checked = Convert.ToBoolean((int)dr["us_send_notifications_to_self"]);
                auto_subscribe.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe"]);
                auto_subscribe_own.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_own_bugs"]);
                auto_subscribe_reported.Checked = Convert.ToBoolean((int)dr["us_auto_subscribe_reported_bugs"]);
                signature.InnerText = (string)dr["signature"];

                foreach (ListItem li in query.Items)
                {
                    if (Convert.ToInt32(li.Value) == (int)dr["us_default_query"])
                    {
                        li.Selected = true;
                        break;
                    }
                }


                // select projects
                foreach (DataRowView drv in projects_dv)
                {
                    foreach (ListItem li in project_auto_subscribe.Items)
                    {
                        if (Convert.ToInt32(li.Value) == (int)drv["pj_id"])
                        {
                            if ((int)drv["pu_auto_subscribe"] == 1)
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
            else
            {
                on_update();
            }
        }



        ///////////////////////////////////////////////////////////////////////
        Boolean validate()
        {

            Boolean good = true;

            pw_err.InnerText = "";

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

            if (!Util.is_int(bugs_per_page.Value))
            {
                good = false;
                bugs_per_page_err.InnerText = Util.get_setting("PluralBugLabel", "Bugs") + " Per Page must be a number.";
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
        void on_update()
        {

            Boolean good = validate();

            if (good)
            {

                sql = new SQLString(@"update users set
			us_firstname = @fn,
			us_lastname = @ln,
			us_bugs_per_page = @bp,
			us_use_fckeditor = @fk,
			us_enable_bug_list_popups = @pp,
			us_email = @em,
			us_enable_notifications = @en,
			us_send_notifications_to_self = @ss,
            us_reported_notifications = @rn,
            us_assigned_notifications = @an,
            us_subscribed_notifications = @sn,
			us_auto_subscribe = @as,
			us_auto_subscribe_own_bugs = @ao,
			us_auto_subscribe_reported_bugs = @ar,
			us_default_query = @dq,
			us_signature = @sg
			where us_id = @id");

                sql = sql.AddParameterWithValue("fn", firstname.Value);
                sql = sql.AddParameterWithValue("ln", lastname.Value);
                sql = sql.AddParameterWithValue("bp", bugs_per_page.Value);
                sql = sql.AddParameterWithValue("fk", Util.bool_to_string(use_fckeditor.Checked));
                sql = sql.AddParameterWithValue("pp", Util.bool_to_string(enable_popups.Checked));
                sql = sql.AddParameterWithValue("em", email.Value);
                sql = sql.AddParameterWithValue("en", Util.bool_to_string(enable_notifications.Checked));
                sql = sql.AddParameterWithValue("ss", Util.bool_to_string(send_to_self.Checked));
                sql = sql.AddParameterWithValue("rn", reported_notifications.SelectedItem.Value);
                sql = sql.AddParameterWithValue("an", assigned_notifications.SelectedItem.Value);
                sql = sql.AddParameterWithValue("sn", subscribed_notifications.SelectedItem.Value);
                sql = sql.AddParameterWithValue("as", Util.bool_to_string(auto_subscribe.Checked));
                sql = sql.AddParameterWithValue("ao", Util.bool_to_string(auto_subscribe_own.Checked));
                sql = sql.AddParameterWithValue("ar", Util.bool_to_string(auto_subscribe_reported.Checked));
                sql = sql.AddParameterWithValue("dq", query.SelectedItem.Value);
                sql = sql.AddParameterWithValue("sg", signature.InnerText);
                sql = sql.AddParameterWithValue("id", Convert.ToString(id));

                // update user
                btnet.DbUtil.execute_nonquery(sql);

                // update the password
                if (pw.Value != "")
                {
                    btnet.Util.update_user_password(id, pw.Value);
                }

                // Now update project_user_xref

                // First turn everything off, then turn selected ones on.
                sql = new SQLString(@"update project_user_xref
				set pu_auto_subscribe = 0 where pu_user = @id");
                sql = sql.AddParameterWithValue("id", Convert.ToString(id));
                btnet.DbUtil.execute_nonquery(sql);

                // Second see what to turn back on
                string projects = "";
                foreach (ListItem li in project_auto_subscribe.Items)
                {
                    if (li.Selected)
                    {
                        if (projects != "")
                        {
                            projects += ",";
                        }
                        projects += Convert.ToInt32(li.Value);
                    }
                }

                // If we need to turn anything back on
                if (projects != "")
                {

                    sql = new SQLString(@"update project_user_xref
				set pu_auto_subscribe = 1 where pu_user = @id and pu_project in ($projects)

			insert into project_user_xref (pu_project, pu_user, pu_auto_subscribe)
				select pj_id, @id, 1
				from projects
				where pj_id in (projects)
				and pj_id not in (select pu_project from project_user_xref where pu_user = @id)");

                    sql = sql.AddParameterWithValue("id", Convert.ToString(id));
                    sql = sql.AddParameterWithValue("projects", projects);
                    btnet.DbUtil.execute_nonquery(sql);
                }


                // apply subscriptions retroactively
                if (retroactive.Checked)
                {
                    sql = new SQLString(@"delete from bug_subscriptions where bs_user = @id;");
                    if (auto_subscribe.Checked)
                    {
                        sql.Append(@"insert into bug_subscriptions (bs_bug, bs_user)
					select bg_id, @id from bugs;");
                    }
                    else
                    {
                        if (auto_subscribe_reported.Checked)
                        {
                            sql.Append(@"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, @id from bugs where bg_reported_user = @id
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @id);");
                        }

                        if (auto_subscribe_own.Checked)
                        {
                            sql.Append(@"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, @id from bugs where bg_assigned_to_user = @id
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @id);");
                        }

                        if (projects != "")
                        {
                            sql.Append(@"insert into bug_subscriptions (bs_bug, bs_user)
						select bg_id, @id from bugs where bg_project in (@projects)
						and bg_id not in (select bs_bug from bug_subscriptions where bs_user = @id);");
                        }
                    }

                    sql = sql.AddParameterWithValue("id", Convert.ToString(id));
                    sql = sql.AddParameterWithValue("projects", projects);
                    btnet.DbUtil.execute_nonquery(sql);

                }

                msg.InnerText = "Your settings have been updated.";
            }
            else
            {
                msg.InnerText = "Your settings have not been updated.";
            }

        }

    }
}
