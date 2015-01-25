using System;
using System.Data;
using System.Text;
using btnet.Security;

namespace btnet
{
    public partial class merge_bug : BasePage
    {

        SQLString sql;

        protected DataRow dr;

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);


            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanMergeBugs())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "merge " + Util.get_setting("SingularBugLabel", "bug");


            if (!IsPostBack)
            {
                string orig_id_string = Util.sanitize_integer(Request["id"]);
                orig_id.Value = orig_id_string;
                back_href.HRef = "edit_bug.aspx?id=" + orig_id_string;
                from_bug.Value = orig_id_string;
            }
            else
            {
                from_err.InnerText = "";
                into_err.InnerText = "";
                on_update();
            }

        }


        ///////////////////////////////////////////////////////////////////////
        bool validate()
        {

            bool good = true;

            // validate FROM

            if (from_bug.Value == "")
            {
                from_err.InnerText = "\"From\" bug is required.";
                good = false;
            }
            else
            {
                if (!Util.is_int(from_bug.Value))
                {
                    from_err.InnerText = "\"From\" bug must be an integer.";
                    good = false;

                }
            }

            // validate INTO

            if (into_bug.Value == "")
            {
                into_err.InnerText = "\"Into\" bug is required.";
                good = false;
            }
            else
            {
                if (!Util.is_int(into_bug.Value))
                {
                    into_err.InnerText = "\"Into\" bug must be an integer.";
                    good = false;

                }
            }


            if (!good)
            {
                return false;
            }


            if (from_bug.Value == into_bug.Value)
            {
                from_err.InnerText = "\"From\" bug cannot be the same as \"Into\" bug.";
                return false;
            }

            // Continue and see if from and to exist in db

            sql = new SQLString(@"
	declare @from_desc nvarchar(200)
	declare @into_desc nvarchar(200)
	declare @from_id int
	declare @into_id int
	set @from_id = -1
	set @into_id = -1
	select @from_desc = bg_short_desc, @from_id = bg_id from bugs where bg_id = @from
	select @into_desc = bg_short_desc, @into_id = bg_id from bugs where bg_id = @into
	select @from_desc, @into_desc, @from_id, @into_id	");

            sql = sql.AddParameterWithValue("from", from_bug.Value);
            sql = sql.AddParameterWithValue("into", into_bug.Value);

            dr = btnet.DbUtil.get_datarow(sql);

            if ((int)dr[2] == -1)
            {
                from_err.InnerText = "\"From\" bug not found.";
                good = false;
            }


            if ((int)dr[3] == -1)
            {
                into_err.InnerText = "\"Into\" bug not found.";
                good = false;
            }


            if (!good)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        ///////////////////////////////////////////////////////////////////////
        void on_update()
        {

            // does it say "Merge" or "Confirm Merge"?

            if (submit.Value == "Merge")
            {
                if (!validate())
                {
                    prev_from_bug.Value = "";
                    prev_into_bug.Value = "";
                    return;
                }
            }


            if (prev_from_bug.Value == from_bug.Value
            && prev_into_bug.Value == into_bug.Value)
            {

                prev_from_bug.Value = btnet.Util.sanitize_integer(prev_from_bug.Value);
                prev_into_bug.Value = btnet.Util.sanitize_integer(prev_into_bug.Value);

                // rename the attachments

                string upload_folder = Util.get_upload_folder();
                if (upload_folder != null)
                {

                    sql = new SQLString(@"select bp_id, bp_file from bug_posts
			where bp_type = 'file' and bp_bug = @from");

                    sql = sql.AddParameterWithValue("from", prev_from_bug.Value);
                    DataSet ds = btnet.DbUtil.get_dataset(sql);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {

                        // create path
                        StringBuilder path = new StringBuilder(upload_folder);
                        path.Append("\\");
                        path.Append(prev_from_bug.Value);
                        path.Append("_");
                        path.Append(Convert.ToString(dr["bp_id"]));
                        path.Append("_");
                        path.Append(Convert.ToString(dr["bp_file"]));
                        if (System.IO.File.Exists(path.ToString()))
                        {

                            StringBuilder path2 = new StringBuilder(upload_folder);
                            path2.Append("\\");
                            path2.Append(prev_into_bug.Value);
                            path2.Append("_");
                            path2.Append(Convert.ToString(dr["bp_id"]));
                            path2.Append("_");
                            path2.Append(Convert.ToString(dr["bp_file"]));

                            System.IO.File.Move(path.ToString(), path2.ToString());
                        }

                    }
                }


                // copy the from db entries to the to
                sql = new SQLString(@"
insert into bug_subscriptions
(bs_bug, bs_user)
select @into, bs_user
from bug_subscriptions
where bs_bug = @from
and bs_user not in (select bs_user from bug_subscriptions where bs_bug = @into)

insert into bug_user
(bu_bug, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime)
select @into, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime
from bug_user
where bu_bug = @from
and bu_user not in (select bu_user from bug_user where bu_bug = @into)

update bug_posts     set bp_bug     = @into	where bp_bug = @from
update bug_tasks     set tsk_bug    = @into where tsk_bug = @from
update svn_revisions set svnrev_bug = @into where svnrev_bug = @from
update hg_revisions  set hgrev_bug  = @into where hgrev_bug = @from
update git_commits   set gitcom_bug = @into where gitcom_bug = @from
");

                sql = sql.AddParameterWithValue("from", prev_from_bug.Value);
                sql = sql.AddParameterWithValue("into", prev_into_bug.Value);

                btnet.DbUtil.execute_nonquery(sql);

                // record the merge itself

                sql = new SQLString(@"insert into bug_posts
			(bp_bug, bp_user, bp_date, bp_type, bp_comment, bp_comment_search)
			values(@into, @us,getdate(), 'comment', 'merged bug @from into this bug:', 'merged bug @from into this bug:')
			select scope_identity()");

                sql = sql.AddParameterWithValue("@from", prev_from_bug.Value);
                sql = sql.AddParameterWithValue("@into", prev_into_bug.Value);
                sql = sql.AddParameterWithValue("@us", Convert.ToString(User.Identity.GetUserId()));

                int comment_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

                // update bug comments with info from old bug
                sql = new SQLString(@"update bug_posts
			set bp_comment = convert(nvarchar,bp_comment) + char(10) + bg_short_desc
			from bugs where bg_id = @from
			and bp_id = @bc");

                sql = sql.AddParameterWithValue("from", prev_from_bug.Value);
                sql = sql.AddParameterWithValue("bc", Convert.ToString(comment_id));
                btnet.DbUtil.execute_nonquery(sql);


                // delete the from bug
                int from_bugid = Convert.ToInt32(prev_from_bug.Value);
                Bug.delete_bug(from_bugid);

                // delete the from bug from the list, if there is a list
                DataView dv_bugs = (DataView)Session["bugs"];
                if (dv_bugs != null)
                {
                    // read through the list of bugs looking for the one that matches the from
                    int index = 0;
                    foreach (DataRowView drv in dv_bugs)
                    {
                        if (from_bugid == (int)drv[1])
                        {
                            dv_bugs.Delete(index);
                            break;
                        }
                        index++;
                    }
                }

                btnet.Bug.send_notifications(btnet.Bug.UPDATE, Convert.ToInt32(prev_into_bug.Value), User.Identity);

                Response.Redirect("edit_bug.aspx?id=" + prev_into_bug.Value);

            }
            else
            {
                prev_from_bug.Value = from_bug.Value;
                prev_into_bug.Value = into_bug.Value;
                static_from_bug.InnerText = from_bug.Value;
                static_into_bug.InnerText = into_bug.Value;
                static_from_desc.InnerText = (string)dr[0];
                static_into_desc.InnerText = (string)dr[1];
                from_bug.Style["display"] = "none";
                into_bug.Style["display"] = "none";
                static_from_bug.Style["display"] = "";
                static_into_bug.Style["display"] = "";
                static_from_desc.Style["display"] = "";
                static_into_desc.Style["display"] = "";
                submit.Value = "Confirm Merge";
            }

        }


    }
}
