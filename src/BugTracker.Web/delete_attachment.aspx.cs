using btnet.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace btnet
{
    public partial class delete_attachment : BasePage
    {

        SQLString sql;

        protected void Page_Init(object sender, EventArgs e) { ViewStateUserKey = Session.SessionID; }

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            this.Master.Menu.SelectedItem = Util.get_setting("PluralBugLabel", "bugs");

            if (User.IsInRole(BtnetRoles.Admin) || User.Identity.GetCanEditAndDeleteBugs())
            {
                //
            }
            else
            {
                Response.Write("You are not allowed to use this page.");
                Response.End();
            }

            string attachment_id_string = Util.sanitize_integer(Request["id"]);
            string bug_id_string = Util.sanitize_integer(Request["bug_id"]);

            int permission_level = Bug.get_bug_permission_level(Convert.ToInt32(bug_id_string), User.Identity);
            if (permission_level != PermissionLevel.All)
            {
                Response.Write("You are not allowed to edit this item");
                Response.End();
            }


            if (IsPostBack)
            {
                // save the filename before deleting the row
                sql = new SQLString(@"select bp_file from bug_posts where bp_id = @ba");
                sql = sql.AddParameterWithValue("ba", attachment_id_string);
                string filename = (string)DbUtil.execute_scalar(sql);

                // delete the row representing the attachment
                sql = new SQLString(@"delete bug_post_attachments where bpa_post = @ba
            delete bug_posts where bp_id = @ba");
                sql = sql.AddParameterWithValue("ba", attachment_id_string);
                DbUtil.execute_nonquery(sql);

                // delete the file too
                string upload_folder = Util.get_upload_folder();
                if (upload_folder != null)
                {
                    StringBuilder path = new StringBuilder(upload_folder);
                    path.Append("\\");
                    path.Append(bug_id_string);
                    path.Append("_");
                    path.Append(attachment_id_string);
                    path.Append("_");
                    path.Append(filename);
                    if (System.IO.File.Exists(path.ToString()))
                    {
                        System.IO.File.Delete(path.ToString());
                    }
                }


                Response.Redirect("edit_bug.aspx?id=" + bug_id_string);
            }
            else
            {
                Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                    + "delete attachment";

                back_href.HRef = "edit_bug.aspx?id=" + bug_id_string;

                sql = new SQLString(@"select bp_file from bug_posts where bp_id = @id");
                sql = sql.AddParameterWithValue("id", attachment_id_string);

                DataRow dr = DbUtil.get_datarow(sql);

                string s = Convert.ToString(dr["bp_file"]);

                confirm_href.InnerText = "confirm delete of attachment: " + s;

                row_id.Value = attachment_id_string;
            }

        }

    }
}
