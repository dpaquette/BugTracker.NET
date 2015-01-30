using System;
using System.Data;
using System.Text;
using btnet.Security;

namespace btnet
{
    public partial class massedit : BasePage
    {

        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanMassEditBugs())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }


            string list = "";

            if (!IsPostBack)
            {
                Master.Menu.SelectedItem = "admin";
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "massedit";

                if (Request["mass_delete"] != null)
                {
                    update_or_delete.Value = "delete";
                }
                else
                {
                    update_or_delete.Value = "update";
                }

                // create list of bugs affected
                foreach (string var in Request.QueryString)
                {
                    if (Util.is_int(var))
                    {
                        if (list != "")
                        {
                            list += ",";
                        }
                        list += var;
                    };
                }

                bug_list.Value = list;

                if (update_or_delete.Value == "delete")
                {
                    update_or_delete.Value = "delete";

                    sql = new SQLString("delete bug_post_attachments from bug_post_attachments inner join bug_posts on bug_post_attachments.bpa_post = bug_posts.bp_id where bug_posts.bp_bug in (" + list + ")");
                    sql.Append("\ndelete from bug_posts where bp_bug in (" + list + ")");
                    sql.Append("\ndelete from bug_subscriptions where bs_bug in (" + list + ")");
                    sql.Append("\ndelete from bug_relationships where re_bug1 in (" + list + ")");
                    sql.Append("\ndelete from bug_relationships where re_bug2 in (" + list + ")");
                    sql.Append("\ndelete from bug_user where bu_bug in (" + list + ")");
                    sql.Append("\ndelete from bug_tasks where tsk_bug in (" + list + ")");
                    sql.Append("\ndelete from bugs where bg_id in (" + list + ")");

                    confirm_href.InnerText = "Confirm Delete";

                }
                else
                {
                    update_or_delete.Value = "update";

                    sql = new SQLString("update bugs \nset ");

                    string updates = "";

                    string val;

                    val = Request["mass_project"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_project = " + val;
                    }

                    val = Request["mass_org"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_org = " + val;
                    }

                    val = Request["mass_category"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_category = " + val;
                    }

                    val = Request["mass_priority"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_priority = " + val;
                    }

                    val = Request["mass_assigned_to"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_assigned_to_user = " + val;
                    }

                    val = Request["mass_reported_by"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_reported_user = " + val;
                    }

                    val = Request["mass_status"];
                    if (val != "-1" && Util.is_int(val))
                    {
                        if (updates != "") { updates += ",\n"; }
                        updates += "bg_status = " + val;
                    }


                    sql.Append(updates + "\nwhere bg_id in (" + list + ")");

                    confirm_href.InnerText = "Confirm Update";

                }

                sql_text.InnerText = sql.ToString();

            }
            else // postback
            {
                list = bug_list.Value;

                if (update_or_delete.Value == "delete")
                {
                    string upload_folder = Util.get_upload_folder();
                    if (upload_folder != null)
                    {
                        // double check the bug_list
                        string[] ints = bug_list.Value.Split(',');
                        for (int i = 0; i < ints.Length; i++)
                        {
                            if (!btnet.Util.is_int(ints[i]))
                            {
                                Response.End();
                            }
                        }

                        var sql2 = new SQLString(@"select bp_bug, bp_id, bp_file from bug_posts where bp_type = 'file' and bp_bug in (" + bug_list.Value + ")");
                        DataSet ds = btnet.DbUtil.get_dataset(sql2);
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            // create path
                            StringBuilder path = new StringBuilder(upload_folder);
                            path.Append("\\");
                            path.Append(Convert.ToString(dr["bp_bug"]));
                            path.Append("_");
                            path.Append(Convert.ToString(dr["bp_id"]));
                            path.Append("_");
                            path.Append(Convert.ToString(dr["bp_file"]));
                            if (System.IO.File.Exists(path.ToString()))
                            {
                                System.IO.File.Delete(path.ToString());
                            }
                        }
                    }
                }


                btnet.DbUtil.execute_nonquery(new SQLString(sql_text.InnerText));
                Response.Redirect("search.aspx");

            }
        }



    }
}
