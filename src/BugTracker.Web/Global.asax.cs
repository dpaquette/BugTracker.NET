using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using btnet.App_Start;
using NLog;

namespace btnet
{
    public class Global : HttpApplication
    {
        /*
    Copyright 2002 Corey Trager 
    Distributed under the terms of the GNU General Public License
    */

        public void Application_Error(Object sender, EventArgs e)
        {
            // Put the server vars into a string

            var server_vars_string = new StringBuilder();

            int loop1, loop2;
            NameValueCollection coll;

            // Load ServerVariable collection into NameValueCollection object.
            coll = Request.ServerVariables;
            // Get names of all keys into a string array.
            String[] arr1 = coll.AllKeys;
            for (loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                string key = arr1[loop1];
                if (key.StartsWith("AUTH_PASSWORD"))
                    continue;

                String[] arr2 = coll.GetValues(key);

                for (loop2 = 0; loop2 < 1; loop2++)
                {
                    string val = arr2[loop2];
                    if (string.IsNullOrEmpty(val))
                        break;
                    server_vars_string.Append("\n");
                    server_vars_string.Append(key);
                    server_vars_string.Append("=");
                    server_vars_string.Append(val);
                }
            }


            Exception exc = Server.GetLastError().GetBaseException();

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Fatal(exc);

            //TODO: This can probably be replaced with a new NLog target or maybe Elmah
            bool error_email_enabled = (Util.get_setting("ErrorEmailEnabled", "1") == "1");
            if (error_email_enabled)
            {
                if (exc.Message == "Expected integer.  Possible SQL injection attempt?")
                {
                    // don't bother sending email.  Too many automated attackers
                }
                else
                {
                    string to = Util.get_setting("ErrorEmailTo", "");
                    string from = Util.get_setting("ErrorEmailFrom", "");
                    string subject = "Error: " + exc.Message;

                    var body = new StringBuilder();


                    body.Append("\nTIME: ");
                    body.Append(DateTime.Now.ToLongTimeString());
                    body.Append("\nURL: ");
                    body.Append(Request.Url);
                    body.Append("\nException: ");
                    body.Append(exc);
                    body.Append(server_vars_string);

                    Email.send_email(to, from, "", subject, body.ToString()); // 5 args				
                }
            }
        }


        public void Application_OnStart(Object sender, EventArgs e)
        {
            LoggingConfig.Configure();
            HttpRuntime.Cache.Add("Application", Application, null, Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

            string dir = Util.GetAbsolutePath("App_Data");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Util.GetAbsolutePath("App_Data\\logs");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Util.GetAbsolutePath("App_Data\\uploads");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Util.GetAbsolutePath("App_Data\\lucene_index");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Util.set_context(HttpContext.Current); // required for map path calls to work in util.cs

            StreamReader sr = File.OpenText(Util.GetAbsolutePath("custom\\custom_header.html"));
            Application["custom_header"] = sr.ReadToEnd();
            sr.Close();

            sr = File.OpenText(Util.GetAbsolutePath("custom\\custom_footer.html"));
            Application["custom_footer"] = sr.ReadToEnd();
            sr.Close();

            sr = File.OpenText(Util.GetAbsolutePath("custom\\custom_logo.html"));
            Application["custom_logo"] = sr.ReadToEnd();
            sr.Close();

            sr = File.OpenText(Util.GetAbsolutePath("custom\\custom_welcome.html"));
            Application["custom_welcome"] = sr.ReadToEnd();
            sr.Close();

            if (Util.get_setting("EnableVotes", "0") == "1")
            {
                Tags.count_votes(Application); // in tags file for convenience for me....
            }

            if (Util.get_setting("EnableTags", "0") == "1")
            {
                Tags.build_tag_index(Application);
            }

            if (Util.get_setting("EnableLucene", "1") == "1")
            {
                MyLucene.build_lucene_index(Application);
            }

            if (Util.get_setting("EnablePop3", "0") == "1")
            {
                MyPop3.start_pop3(Application);
            }
        }

    }
}