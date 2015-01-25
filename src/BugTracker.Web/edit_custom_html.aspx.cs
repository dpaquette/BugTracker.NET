using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    [PageAuthorize(BtnetRoles.Admin)]
    public partial class edit_custom_html : BasePage
    {


        ///////////////////////////////////////////////////////////////////////
        protected void Page_Load(Object sender, EventArgs e)
        {

            Util.do_not_cache(Response);

            Page.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "edit web config";

            string which_file = "";
            string file_name = "";

            if (!IsPostBack)
            {
                which_file = Request["which"];

                // default to footer
                if (string.IsNullOrEmpty(which_file))
                {
                    which_file = "footer";
                }

                file_name = get_file_name(which_file);
                msg.InnerHtml = "&nbsp;";
            }
            else
            {
                which_file = which.Value;

                if (string.IsNullOrEmpty(which_file))
                {
                    Response.End();
                }

                file_name = get_file_name(which_file);

                if (file_name == "")
                    Response.End();

                // save to disk
                string path = HttpContext.Current.Server.MapPath(null);
                path += "\\custom\\";

                System.IO.StreamWriter sw = System.IO.File.CreateText(path + file_name);
                sw.Write(myedit.Value);
                sw.Close();
                sw.Dispose();

                // save in Application (memory)
                Application[System.IO.Path.GetFileNameWithoutExtension(file_name)] = myedit.Value;

                msg.InnerHtml = file_name + " was saved.";
            }

            load_file_into_control(file_name);

            which.Value = which_file;
        }

        void load_file_into_control(string file_name)
        {
            string path = HttpContext.Current.Server.MapPath(null);
            path += "\\custom\\" + file_name;

            System.IO.StreamReader sr = System.IO.File.OpenText(path);
            myedit.Value = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
        }

        string get_file_name(string which_file)
        {
            string file_name = "";

            if (which_file == "css")
            {
                file_name = "btnet_custom.css";
            }
            else if (which_file == "footer")
            {
                file_name = "custom_footer.html";
            }
            else if (which_file == "header")
            {
                file_name = "custom_header.html";
            }
            else if (which_file == "logo")
            {
                file_name = "custom_logo.html";
            }
            else if (which_file == "welcome")
            {
                file_name = "custom_welcome.html";
            }

            return file_name;

        }   
    }
}
