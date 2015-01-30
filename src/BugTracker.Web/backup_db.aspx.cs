using System;
using System.Web;
using System.Linq;
using System.Data;
using btnet.Security;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class backup_db : BasePage
    {

        string app_data_folder;

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "backup db";

            app_data_folder = HttpContext.Current.Server.MapPath(null);
            app_data_folder += "\\App_Data\\";

            if (!IsPostBack)
            {
                get_files();
            }

        }

        protected void get_files()
        {
            string[] backup_files = System.IO.Directory.GetFiles(app_data_folder, "*.bak");

            if (backup_files.Length == 0)
            {
                MyDataGrid.Visible = false;
                return;
            }

            MyDataGrid.Visible = true;

            // sort the files
            ArrayList list = new ArrayList();
            list.AddRange(backup_files);
            list.Sort();

            DataTable dt = new DataTable();
            DataRow dr;

            dt.Columns.Add(new DataColumn("file", typeof(String)));
            dt.Columns.Add(new DataColumn("url", typeof(String)));

            for (int i = 0; i < list.Count; i++)
            {
                dr = dt.NewRow();

                string just_file = System.IO.Path.GetFileName((string)list[i]);
                dr[0] = just_file;
                dr[1] = "download_file.aspx?which=backup&filename=" + just_file;

                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);

            MyDataGrid.DataSource = dv;
            MyDataGrid.DataBind();
        }


        protected void on_backup(Object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string db = (string)btnet.DbUtil.execute_scalar(new SQLString("select db_name()"));
            string backup_file = app_data_folder + "db_backup_" + date + ".bak";
            var sql = new SQLString("backup database " + db + " to disk = '" + backup_file + "'");
            btnet.DbUtil.execute_nonquery(sql);
            get_files();
        }


        protected void my_button_click(object sender, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "dlt")
            {
                int i = e.Item.ItemIndex;
                string file = MyDataGrid.Items[i].Cells[0].Text;
                System.IO.File.Delete(app_data_folder + file);
                get_files();
            }

        }

 

    }
}
