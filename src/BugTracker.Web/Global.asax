<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Caching" %>
<%@ Import Namespace="btnet" %>
<script RunAt="server" Language="C#">

    /*
Copyright 2002 Corey Trager 
Distributed under the terms of the GNU General Public License
*/

    string prev_day = DateTime.Now.ToString("yyyy-MM-dd");

    public void Application_Error(Object sender, EventArgs e)
    {
        // Put the server vars into a string
        
        StringBuilder server_vars_string = new StringBuilder();
/*
        var varnames = Request.ServerVariables.AllKeys.Where(x => !x.StartsWith("AUTH_PASSWORD"));

        foreach (string varname in varnames)
        {
            string varval = Request.ServerVariables[varname];
            if (!string.IsNullOrEmpty(varval))
            {
                server_vars_string.Append("\n");
                server_vars_string.Append(varname);
                server_vars_string.Append("=");
                server_vars_string.Append(varval);
            }
        }
*/


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

        bool log_enabled = (Util.get_setting("LogEnabled", "1") == "1");
        if (log_enabled)
        {

            string path = Util.get_log_file_path();

            // open file
            StreamWriter w = File.AppendText(path);

            w.WriteLine("\nTIME: " + DateTime.Now.ToLongTimeString());
            w.WriteLine("MSG: " + exc.Message.ToString());
            w.WriteLine("URL: " + Request.Url.ToString());
            w.WriteLine("EXCEPTION: " + exc.ToString());
            w.WriteLine(server_vars_string.ToString());
            w.Close();
        }

        bool error_email_enabled = (btnet.Util.get_setting("ErrorEmailEnabled", "1") == "1");
        if (error_email_enabled)
        {

            if (exc.Message.ToString() == "Expected integer.  Possible SQL injection attempt?")
            {
                // don't bother sending email.  Too many automated attackers
            }
            else
            {
                string to = Util.get_setting("ErrorEmailTo", "");
                string from = Util.get_setting("ErrorEmailFrom", "");
                string subject = "Error: " + exc.Message.ToString();

                StringBuilder body = new StringBuilder();


                body.Append("\nTIME: ");
                body.Append(DateTime.Now.ToLongTimeString());
                body.Append("\nURL: ");
                body.Append(Request.Url.ToString());
                body.Append("\nException: ");
                body.Append(exc.ToString());
                body.Append(server_vars_string.ToString());

                btnet.Email.send_email(to, from, "", subject, body.ToString()); // 5 args				
            }
        }
    }

    /*     
    static void my_threadproc(object obj)     
    {
        for (int i = 0; i < 50; i++)
        {
            System.Threading.Thread.Sleep(1000);
            System.Console.Beep(440,10);
        }
	
    }
    */


    public void Application_OnStart(Object sender, EventArgs e)
    {
        /*
            System.Threading.Thread thread = new System.Threading.Thread(my_threadproc);
            thread.Start(null);
        */

        string path = HttpContext.Current.Server.MapPath(null);
        //    HttpRuntime.Cache["MapPath"] = path;
        //    HttpRuntime.Cache["Application"] = Application;
        HttpRuntime.Cache.Add("MapPath", path, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
        HttpRuntime.Cache.Add("Application", Application, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);

        string dir = path + "\\App_Data";
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        dir = path + "\\App_Data\\logs";
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        dir = path + "\\App_Data\\uploads";
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        dir = path + "\\App_Data\\lucene_index";
        if (!System.IO.Directory.Exists(dir))
        {
            System.IO.Directory.CreateDirectory(dir);
        }

        btnet.Util.set_context(HttpContext.Current); // required for map path calls to work in util.cs

        System.IO.StreamReader sr = System.IO.File.OpenText(path + "\\custom\\custom_header.html");
        Application["custom_header"] = sr.ReadToEnd();
        sr.Close();

        sr = System.IO.File.OpenText(path + "\\custom\\custom_footer.html");
        Application["custom_footer"] = sr.ReadToEnd();
        sr.Close();

        sr = System.IO.File.OpenText(path + "\\custom\\custom_logo.html");
        Application["custom_logo"] = sr.ReadToEnd();
        sr.Close();

        sr = System.IO.File.OpenText(path + "\\custom\\custom_welcome.html");
        Application["custom_welcome"] = sr.ReadToEnd();
        sr.Close();

        if (btnet.Util.get_setting("EnableVotes", "0") == "1")
        {
            btnet.Tags.count_votes(this.Application); // in tags file for convenience for me....
        }

        if (btnet.Util.get_setting("EnableTags", "0") == "1")
        {
            btnet.Tags.build_tag_index(this.Application);
        }

        if (btnet.Util.get_setting("EnableLucene", "1") == "1")
        {
            btnet.MyLucene.build_lucene_index(this.Application);
        }

        if (btnet.Util.get_setting("EnablePop3", "0") == "1")
        {
            btnet.MyPop3.start_pop3(this.Application);
        }
    }

  
/*
public void Application_BeginRequest(Object sender, EventArgs e)
{

	string day = DateTime.Now.ToString("yyyy-MM-dd");
	
	if (day != prev_day)
	{
		prev_day = day;
		Util.write_to_log("Global.asax detected first page hit of the day");
	}

	
}
*/
</script>
