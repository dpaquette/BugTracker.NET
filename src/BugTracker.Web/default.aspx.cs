using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using btnet.Security;

namespace btnet
{
    [PageAllowAnonymous]
    public partial class @default : BasePage
    {
        private SQLString sql;


        ///////////////////////////////////////////////////////////////////////
        public void Page_Load(Object sender, EventArgs e)
        {

            Util.set_context(HttpContext.Current);

            Util.do_not_cache(Response);

            Page.Header.Title = Util.get_setting("AppTitle", "BugTracker.NET") + " - "
                + "logon";

            msg.InnerText = "";

            // see if the connection string works
            try
            {
                // Intentionally getting an extra connection here so that we fall into the right "catch"
                SqlConnection conn = btnet.DbUtil.GetConnection();
                conn.Close();

                try
                {
                    btnet.DbUtil.execute_nonquery(new SQLString("select count(1) from users"));

                }
                catch (SqlException e1)
                {
                    Util.write_to_log(e1.Message);
                    Util.write_to_log(Util.get_setting("ConnectionString", "?"));
                    msg.InnerHtml = "Unable to find \"bugs\" table.<br>"
                    + "Click to <a href=install.aspx>setup database tables</a>";
                }

            }
            catch (SqlException e2)
            {
                msg.InnerHtml = "Unable to connect.<br>"
                + e2.Message + "<br>"
                + "Check Web.config file \"ConnectionString\" setting.<br>"
                + "Check also README.html<br>"
                + "Check also <a href=http://sourceforge.net/projects/btnet/forums/forum/226938>Help Forum</a> on Sourceforge.";
            }

            // Get authentication mode
            string auth_mode = Util.get_setting("WindowsAuthentication", "0");
            HttpCookie username_cookie = Request.Cookies["user"];
            string previous_auth_mode = "0";
            if (username_cookie != null)
            {
                previous_auth_mode = username_cookie["NTLM"];
            }

            // If an error occured, then force the authentication to manual
            if (Request.QueryString["msg"] == null)
            {
                // If windows authentication only, then redirect
                if (auth_mode == "1")
                {
                    btnet.Util.redirect("loginNT.aspx", Request, Response);
                }

                // If previous login was with windows authentication, then try it again
                if (previous_auth_mode == "1" && auth_mode == "2")
                {
                    Response.Cookies["user"]["name"] = "";
                    Response.Cookies["user"]["NTLM"] = "0";
                    btnet.Util.redirect("loginNT.aspx", Request, Response);
                }
            }
            else
            {
                if (Request.QueryString["msg"] != "logged off")
                {
                    msg.InnerHtml = "Error during windows authentication:<br>"
                        + HttpUtility.HtmlEncode(Request.QueryString["msg"]);
                }
            }


            // fill in the username first time in
            if (!IsPostBack)
            {
                if (previous_auth_mode == "0")
                {
                    //	User name and password are not on the querystring.
                    if (username_cookie != null)
                    {
                        //	Set the user name from the last logon.
                        user.Value = username_cookie["name"];
                    }
                }
            }
            else
            {
                on_logon();
            }

        }

        ///////////////////////////////////////////////////////////////////////
        void on_logon()
        {
            var username = user.Value;
            string auth_mode = Util.get_setting("WindowsAuthentication", "0");
            if (auth_mode != "0")
            {
                if (username.Trim() == "")
                {
                    Util.redirect("loginNT.aspx", Request, Response);
                }
            }
            LoginResult loginResult = Authenticate.AttemptLogin(Request.GetOwinContext(), username, pw.Value);

            if (loginResult.Success)
            {
                Util.redirect(Request, Response);
            }
            else
            {
                msg.InnerText = loginResult.ErrorMessage;
            }

        }

    }
}