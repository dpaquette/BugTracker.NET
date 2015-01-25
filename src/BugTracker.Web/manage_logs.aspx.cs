using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace btnet
{
    public partial class manage_logs : BasePage
    {
        string app_data_folder;

        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Master.Menu.SelectedItem = "admin";
            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "manage logs";

            app_data_folder = HttpContext.Current.Server.MapPath(null);
            app_data_folder += "\\App_Data\\logs\\";

            if (!IsPostBack)
            {
                get_files();
            }

        }

        void get_files()
        {
            string[] backup_files = System.IO.Directory.GetFiles(app_data_folder, "*.txt");

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

            for (int i = list.Count - 1; i != -1; i--)
            {
                dr = dt.NewRow();

                string just_file = System.IO.Path.GetFileName((string)list[i]);
                dr[0] = just_file;
                dr[1] = "download_file.aspx?which=log&filename=" + just_file;

                dt.Rows.Add(dr);
            }

            DataView dv = new DataView(dt);

            MyDataGrid.DataSource = dv;
            MyDataGrid.DataBind();
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
