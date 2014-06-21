using System;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Web;
//using anmar.SharpMimeTools;


public class POP3Main
{

    protected enum service_state { STARTED, PAUSED, STOPPED };
    protected service_state state = service_state.STARTED;

    protected string config_file;
    public static bool verbose = true;
    public static string LogFileFolder;
    public static int LogEnabled = 1;
    protected bool suspended = false;

    static object dummy = new object();

    protected Timer timer;
    protected int FetchIntervalInMinutes = 15;
    protected System.Collections.ArrayList websites;
    protected string MessageInputFile;
    protected string MessageOutputFile;
    protected string ConnectionString;
    protected string Pop3Server;
    protected string Pop3Port;
    protected string Pop3UseSSL;

    protected string SubjectMustContain;
    protected string SubjectCannotContain;
    protected string[] SubjectCannotContainStrings;

    protected string FromMustContain;
    protected string FromCannotContain;
    protected string[] FromCannotContainStrings;

    protected string DeleteMessagesOnServer;
    protected string InsertBugUrl;
    protected string ServiceUsername;
    protected string ServicePassword;
    protected string TrackingIdString;
    protected int TotalErrorsAllowed = 999999;
    protected int total_error_count = 0;
    protected int ReadInputStreamCharByChar = 0;
    protected int EnableWatchdogThread = 1;
    protected int RespawnFetchingThreadAfterNSecondsOfInactivity = 60 * 60 * 2; // 6 hours

    static Regex rePipes = new Regex("\\|");

    System.Threading.Thread fetching_thread;
    System.Threading.Thread watchdog_thread;

    public static DateTime heartbeat_datetime = DateTime.Now;

    ///////////////////////////////////////////////////////////////////
    public POP3Main(string config_file, bool verbose)
    {
        string this_exe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
        LogFileFolder = System.IO.Path.GetDirectoryName(this_exe);

        this.config_file = config_file;
        POP3Main.verbose = verbose;

        ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();

        get_settings();
        write_line("creating");

        fetching_thread = new System.Threading.Thread(new System.Threading.ThreadStart(fetching_thread_proc));
        fetching_thread.Start();

        if (EnableWatchdogThread == 1)
        {
            watchdog_thread = new System.Threading.Thread(new System.Threading.ThreadStart(watchdog_thread_proc));
            watchdog_thread.Start();
        }

    }

    ///////////////////////////////////////////////////////////////////
    public void start()
    {

        // call do_work()
        write_line("starting");
        state = service_state.STARTED;
    }

    ///////////////////////////////////////////////////////////////////
    public void pause()
    {
        write_line("pausing");
        state = service_state.PAUSED;
    }

    ///////////////////////////////////////////////////////////////////
    public void stop()
    {
        write_line("stopping");
        state = service_state.STOPPED;
    }


    ///////////////////////////////////////////////////////////////////////
    public static string get_log_file_path()
    {

        // determine log file name

        DateTime now = DateTime.Now;
        string now_string =
            (now.Year).ToString()
            + "_" +
            (now.Month).ToString("0#")
            + "_" +
            (now.Day).ToString("0#");

        string path = LogFileFolder
            + "\\"
            + "btnet_service_log_"
            + now_string
            + ".txt";

        return path;

    }

    ///////////////////////////////////////////////////////////////////////
    public static void write_to_log(string s)
    {

        string path = get_log_file_path();

        lock (dummy)
        {
            System.IO.StreamWriter w = System.IO.File.AppendText(path);

            w.WriteLine(DateTime.Now.ToLongTimeString()
                + " "
                + s);

            w.Close();
        }
    }

    ///////////////////////////////////////////////////////////////////
    public static void write_line(object o)
    {
        if (LogEnabled == 1)
        {
            write_to_log(Convert.ToString(o));
        }

        if (verbose)
        {
            Console.WriteLine(o);
        }
    }


    ///////////////////////////////////////////////////////////////////
    public void fetching_thread_proc()
    {

        write_line("entering fetching thread");

        do_work(null, null);

        while (true)
        {
            System.Threading.Thread.Sleep(2000);
            if (state == service_state.STOPPED)
            {
                timer.Enabled = false;
                break;
            }
        }

        write_line("exiting fetching thread");

    }


    ///////////////////////////////////////////////////////////////////
    public void watchdog_thread_proc()
    {
        POP3Main.write_line("entering watchdog thread");

        while (true)
        {
            System.Threading.Thread.Sleep(2000);
    
            if (state == service_state.STOPPED)
            {
                break;
            }
            else
            {

                TimeSpan timespan = DateTime.Now.Subtract(heartbeat_datetime);

                if (timespan.TotalSeconds > RespawnFetchingThreadAfterNSecondsOfInactivity)
                {
                    POP3Main.write_line("WARNING - watchdog thread is killing fetching thread");
                    fetching_thread.Abort();
                    fetching_thread = new System.Threading.Thread(new System.Threading.ThreadStart(fetching_thread_proc));
                    POP3Main.write_line("WARNING - watchdog thread is starting new fetching thread");
                    fetching_thread.Start();
                }
            }
        }

        POP3Main.write_line("exiting watchdog thread");

    }


    ///////////////////////////////////////////////////////////////////
    public void do_work(object source, ElapsedEventArgs eea)
    {

        heartbeat_datetime = DateTime.Now;
        write_line("doing work, updating heartbeat to " + heartbeat_datetime.ToString("yyyy-MM-dd h:mm tt"));

        if (state != service_state.STARTED)
        {

            write_line("not in STARTED state");
        }
        else
        {
            get_settings();


            for (int i = 0; i < websites.Count; i++)
            {
                if (state != service_state.STARTED)
                {
                    break;
                }

                StringDictionary settings = (StringDictionary)websites[i];

                MessageInputFile = settings["MessageInputFile"];
                MessageOutputFile = settings["MessageOutputFile"];
                ConnectionString = settings["ConnectionString"];
                Pop3Server = settings["Pop3Server"];
                Pop3Port = settings["Pop3Port"];
                Pop3UseSSL = settings["Pop3UseSSL"];
                SubjectMustContain = settings["SubjectMustContain"];

                SubjectCannotContain = settings["SubjectCannotContain"];
                SubjectCannotContainStrings = rePipes.Split(SubjectCannotContain);

                FromMustContain = settings["FromMustContain"];

                FromCannotContain = settings["FromCannotContain"];
                FromCannotContainStrings = rePipes.Split(FromCannotContain);

                DeleteMessagesOnServer = settings["DeleteMessagesOnServer"];
                InsertBugUrl = settings["InsertBugUrl"];
                ServiceUsername = settings["ServiceUsername"];
                ServicePassword = settings["ServicePassword"];
                TrackingIdString = settings["TrackingIdString"];

                write_line("*** fetching messages for website " + Convert.ToString(i + 1) + " " + InsertBugUrl);

                fetch_messages_for_projects();

            }
        }

        resume(); // reset the timer

    }


    ///////////////////////////////////////////////////////////////////
    public void resume()
    {
        // Set up a timer so that we keep fetching messages
        if (timer != null)
        {
            timer.Stop();
            timer.Dispose();
        }

        timer = new System.Timers.Timer();
        timer.AutoReset = false;
        timer.Elapsed += new ElapsedEventHandler(do_work);

        // Set the timer interval
        timer.Interval = 60 * 1000 * FetchIntervalInMinutes;
        timer.Enabled = true;
    }

    ///////////////////////////////////////////////////////////////////
    protected void get_settings()
    {

        write_line("get_settings");

        websites = new ArrayList();
        StringDictionary settings = null;

        string filename = config_file;
        XmlTextReader tr = null;

        try
        {
            tr = new XmlTextReader(filename);
            while (tr.Read())
            {
                //continue;
                if (tr.Name == "add")
                {
                    string key = tr["key"];

                    if (key == "FetchIntervalInMinutes")
                    {
                        write_line(key + "=" + tr["value"]);
                        FetchIntervalInMinutes = Convert.ToInt32(tr["value"]);
                    }
                    else if (key == "TotalErrorsAllowed")
                    {
                        write_line(key + "=" + tr["value"]);
                        TotalErrorsAllowed = Convert.ToInt32(tr["value"]);
                    }
                    else if (key == "ReadInputStreamCharByChar")
                    {
                        write_line(key + "=" + tr["value"]);
                        ReadInputStreamCharByChar = Convert.ToInt32(tr["value"]);
                    }
                    else if (key == "LogFileFolder")
                    {
                        write_line(key + "=" + tr["value"]);
                        LogFileFolder = Convert.ToString(tr["value"]);
                    }
                    else if (key == "LogEnabled")
                    {
                        write_line(key + "=" + tr["value"]);
                        LogEnabled = Convert.ToInt32(tr["value"]);
                    }
                    else if (key == "EnableWatchdogThread")
                    {
                        write_line(key + "=" + tr["value"]);
                        EnableWatchdogThread = Convert.ToInt32(tr["value"]);
                    }
                    else if (key == "RespawnFetchingThreadAfterNSecondsOfInactivity")
                    {
                        write_line(key + "=" + tr["value"]);
                        RespawnFetchingThreadAfterNSecondsOfInactivity = Convert.ToInt32(tr["value"]);
                    }
                    else
                    {
                        if (key == "ConnectionString"
                        || key == "Pop3Server"
                        || key == "Pop3Port"
                        || key == "Pop3UseSSL"
                        || key == "SubjectMustContain"
                        || key == "SubjectCannotContain"
                        || key == "FromMustContain"
                        || key == "FromCannotContain"
                        || key == "DeleteMessagesOnServer"
                        || key == "FetchIntervalInMinutes"
                        || key == "InsertBugUrl"
                        || key == "ServiceUsername"
                        || key == "ServicePassword"
                        || key == "TrackingIdString"
                        || key == "MessageInputFile"
                        || key == "MessageOutputFile")
                        {
                            write_line(key + "=" + tr["value"]);
                            if (settings != null)
                            {
                                settings[key] = tr["value"];
                            }
                        }
                    }
                    // else an uninteresting setting
                }
                else
                {
                    // create a new dictionary of settings each time we encounter a new Website section
                    if (tr.Name.ToLower() == "website" && tr.NodeType == XmlNodeType.Element)
                    {
                        settings = new System.Collections.Specialized.StringDictionary();
                        settings["MessageInputFile"] = "";
                        settings["MessageOutputFile"] = "";
                        settings["ConnectionString"] = "";
                        settings["Pop3Server"] = "";
                        settings["Pop3Port"] = "";
                        settings["Pop3UseSSL"] = "";
                        settings["SubjectMustContain"] = "";
                        settings["SubjectCannotContain"] = "";
                        settings["FromMustContain"] = "";
                        settings["FromCannotContain"] = "";
                        settings["DeleteMessagesOnServer"] = "";
                        settings["InsertBugUrl"] = "";
                        settings["ServiceUsername"] = "";
                        settings["ServicePassword"] = "";
                        settings["TrackingIdString"] = "";
                        websites.Add(settings);
                        write_line("*** loading settings for website " + Convert.ToString(websites.Count));
                    }
                }
            }
        }
        catch (Exception e)
        {
            write_line("Error trying to read file: " + filename);
            write_line(e);
        }

        tr.Close();

    }

    ///////////////////////////////////////////////////////////////////////
    protected void fetch_messages_for_projects()
    {

        // Get the list of accounts to read

        try
        {
            string sql = @"select
				pj_id, pj_pop3_username, pj_pop3_password
				from projects
				where pj_enable_pop3 = 1";

            DataSet ds = get_dataset(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                if (state != service_state.STARTED)
                {
                    break;
                }


                write_line("processing project " + Convert.ToString(dr["pj_id"]) + " using account " + dr["pj_pop3_username"]);

                fetch_messages(
                    (string)dr["pj_pop3_username"],
                    (string)dr["pj_pop3_password"],
                    (int)dr["pj_id"]);
            }
        }
        catch (Exception e)
        {
            write_line("Error trying to process messages");
            write_line(e);
            return;
        }

    }

    ///////////////////////////////////////////////////////////////////////
    protected string maybe_append_next_line(string[] lines, int j)
    {
        string s = "";
        if (j + 1 < lines.Length)
        {
            int pos = -1;

            // find first non space, non tab
            for (int i = 0; i < lines[j + 1].Length; i++)
            {
                String c = lines[j + 1].Substring(i, 1);
                if (c == "\t" || c == " ")
                {
                    continue;
                }
                else
                {
                    pos = i;
                    break;
                }
            }

            // this line is part of the previous header, so return it
            if (pos > 0)
            {
                s = " ";
                s = lines[j + 1].Substring(pos);
            }
        }
        return s;
    }

    ///////////////////////////////////////////////////////////////////////
    protected void fetch_messages(string user, string password, int projectid)
    {

        string[] messages = null;
        Regex regex = new Regex("\r\n");
        string[] test_message_text = new string[100];
        POP3Client.POP3client client = null;

        if (MessageInputFile == "")
        {

            try
            {
                client = new POP3Client.POP3client(ReadInputStreamCharByChar);

                write_line("****connecting to server:");
                int port = 110;
                if (Pop3Port != "")
                {
                    port = Convert.ToInt32(Pop3Port);
                }

                bool use_ssl = false;
                if (Pop3UseSSL != "")
                {
                    use_ssl = Pop3UseSSL == "1" ? true : false;
                }

                write_line(client.connect(Pop3Server, port, use_ssl));

                write_line("sending POP3 command USER");
                write_line(client.USER(user));

                write_line("sending POP3 command PASS");
                write_line(client.PASS(password));

                write_line("sending POP3 command STAT");
                write_line(client.STAT());

                write_line("sending POP3 command LIST");
                string list;
                list = client.LIST();
                write_line("list follows:");
                write_line(list);
                messages = regex.Split(list);
            }
            catch (Exception e)
            {
                write_line("Exception trying to talk to pop3server");
                write_line(e);
                return;
            }

        }
        else
        {
            StringBuilder builder = new StringBuilder(4096);
            write_line("opening test input file " + MessageInputFile);
            using (FileStream fs = File.OpenRead(MessageInputFile))
            {
                byte[] b = new byte[4096];
                //UTF8Encoding encoding = new UTF8Encoding(true);  // Does not work...

                int bytes_read = fs.Read(b, 0, b.Length);

                while (bytes_read > 0)
                {
                    //test_messages += encoding.GetString(b); // Does not work....

                    for (int i = 0; i < bytes_read; i++)
                    {
                        builder.Append(Convert.ToChar(b[i])); // Does work
                    }

                    bytes_read = fs.Read(b, 0, b.Length);

                }

            }

            string test_messages = builder.ToString();
            Regex test_regex = new Regex("Q6Q6\r\n");
            test_message_text = test_regex.Split(test_messages);
        }


        string message;
        int message_number = 0;
        int start;
        int end;

        if (MessageInputFile == "")
        {
            start = 1;
            end = messages.Length - 1;
        }
        else
        {
            start = 0;
            end = test_message_text.Length;
            if (end > 99) end = 99;
        }

        // loop through the messages
        for (int i = start; i < end; i++)
        {
            heartbeat_datetime = DateTime.Now; // because the watchdog is watching

            if (state != service_state.STARTED)
            {
                break;
            }

            // fetch the message

            write_line("i:" + Convert.ToString(i));
            if (MessageInputFile == "")
            {
                int space_pos = messages[i].IndexOf(" ");
                message_number = Convert.ToInt32(messages[i].Substring(0, space_pos));
                message = client.RETR(message_number);
            }
            else
            {
                message = test_message_text[message_number++];
            }

            // for diagnosing problems
            if (MessageOutputFile != "")
            {
                System.IO.StreamWriter w = System.IO.File.AppendText(MessageOutputFile);
                w.WriteLine(message);
                w.Flush();
                w.Close();
            }

            // break the message up into lines
            string[] lines = regex.Split(message);

            string from = "";
            string subject = "";

            bool encountered_subject = false;
            bool encountered_from = false;


            // Loop through the lines of a message.
            // Pick out the subject and body
            for (int j = 0; j < lines.Length; j++)
            {

                if (state != service_state.STARTED)
                {
                    break;
                }

                // We know from
                // http://www.devnewsgroups.net/group/microsoft.public.dotnet.framework/topic62515.aspx
                // that headers can be lowercase too.

                if ((lines[j].IndexOf("Subject: ") == 0 || lines[j].IndexOf("subject: ") == 0)
                && !encountered_subject)
                {
                    subject = lines[j].Replace("Subject: ", "");
                    subject = subject.Replace("subject: ", ""); // try lowercase too
                    subject += maybe_append_next_line(lines, j);

                    encountered_subject = true;
                }
                else if (lines[j].IndexOf("From: ") == 0 && !encountered_from)
                {
                    from = lines[j].Replace("From: ", "");
                    encountered_from = true;
                    from += maybe_append_next_line(lines, j);

                }
                else if (lines[j].IndexOf("from: ") == 0 && !encountered_from)
                {
                    from = lines[j].Replace("from: ", "");
                    encountered_from = true;
                    from += maybe_append_next_line(lines, j);
                }

            } // end for each line

            write_line("\nFrom: " + from);

            write_line("Subject: " + subject);

            if (SubjectMustContain != "" && subject.IndexOf(SubjectMustContain) < 0)
            {
                write_line("skipping because subject does not contain: " + SubjectMustContain);
                continue;
            }

            bool bSkip = false;
            for (int k = 0; k < SubjectCannotContainStrings.Length; k++)
            {
                if (SubjectCannotContainStrings[k] != "")
                {
                    if (subject.IndexOf(SubjectCannotContainStrings[k]) >= 0)
                    {
                        write_line("skipping because subject cannot contain: " + SubjectCannotContainStrings[k]);
                        bSkip = true;
                        break;  // done checking, skip this message
                    }
                }
            }

            if (bSkip)
            {
                continue;
            }

            if (FromMustContain != "" && from.IndexOf(FromMustContain) < 0)
            {
                write_line("skipping because from does not contain: " + FromMustContain);
                continue; // that is, skip to next message
            }

            for (int k = 0; k < FromCannotContainStrings.Length; k++)
            {
                if (FromCannotContainStrings[k] != "")
                {
                    if (from.IndexOf(FromCannotContainStrings[k]) >= 0)
                    {
                        write_line("skipping because from cannot contain: " + FromCannotContainStrings[k]);
                        bSkip = true;
                        break; // done checking, skip this message
                    }
                }
            }

            if (bSkip)
            {
                continue;
            }

            write_line("calling insert_bug.aspx");
            string Url = InsertBugUrl;

            // Try to parse out the bugid from the subject line
            string bugidString = TrackingIdString;
            if (TrackingIdString == "")
            {
                bugidString = "DO NOT EDIT THIS:";
            }

            int pos = subject.IndexOf(bugidString);

            if (pos >= 0)
            {
                // position of colon
                pos = subject.IndexOf(":", pos);
                pos++;
                // position of close paren
                int pos2 = subject.IndexOf(")", pos);
                if (pos2 > pos)
                {
                    string bugid_string = subject.Substring(pos, pos2 - pos);
                    write_line("BUGID=" + bugid_string);
                    try
                    {
                        int bugid = Int32.Parse(bugid_string);
                        Url += "?bugid=" + Convert.ToString(bugid);
                        write_line("updating existing bug " + Convert.ToString(bugid));
                    }
                    catch (Exception e)
                    {
                        write_line("bugid not numeric " + e.Message);
                    }
                }
            }


            string post_data = "username=" + HttpUtility.UrlEncode(ServiceUsername)
                + "&password=" + HttpUtility.UrlEncode(ServicePassword)
                + "&projectid=" + Convert.ToString(projectid)
                + "&from=" + HttpUtility.UrlEncode(from)
                + "&short_desc=" + HttpUtility.UrlEncode(subject)
                + "&message=" + HttpUtility.UrlEncode(message);

            byte[] bytes = Encoding.UTF8.GetBytes(post_data);


            // send request to web server
            HttpWebResponse res = null;
            try
            {
                HttpWebRequest req = (HttpWebRequest)System.Net.WebRequest.Create(Url);


                req.Credentials = CredentialCache.DefaultCredentials;
                req.PreAuthenticate = true;

                //req.Timeout = 200; // maybe?
                //req.KeepAlive = false; // maybe?

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bytes.Length;
                Stream request_stream = req.GetRequestStream();
                request_stream.Write(bytes, 0, bytes.Length);
                request_stream.Close();
                res = (HttpWebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                write_line("HttpWebRequest error url=" + Url);
                write_line(e);
            }

            // examine response

            if (res != null)
            {

                int http_status = (int)res.StatusCode;
                write_line(Convert.ToString(http_status));

                string http_response_header = res.Headers["BTNET"];
                res.Close();

                if (http_response_header != null)
                {
                    write_line(http_response_header);

                    // only delete message from pop3 server if we
                    // know we stored in on the web server ok
                    if (MessageInputFile == ""
                    && http_status == 200
                    && DeleteMessagesOnServer == "1"
                    && http_response_header.IndexOf("OK") == 0)
                    {
                        write_line("sending POP3 command DELE");
                        write_line(client.DELE(message_number));
                    }
                }
                else
                {
                    write_line("BTNET HTTP header not found.  Skipping the delete of the email from the server.");
                    write_line("Incrementing total error count");
                    total_error_count++;
                }
            }
            else
            {
                write_line("No response from web server.  Skipping the delete of the email from the server.");
                write_line("Incrementing total error count");
                total_error_count++;
            }

            if (total_error_count > TotalErrorsAllowed)
            {
                write_line("Stopping because total error count > TotalErrorsAllowed");
                stop();
            }


        }  // end for each message


        if (MessageInputFile == "")
        {
            write_line("\nsending POP3 command QUIT");
            write_line(client.QUIT());
        }
        else
        {
            write_line("\nclosing input file " + MessageInputFile);
        }

    }

    ///////////////////////////////////////////////////////////////////////
    protected DataSet get_dataset(string sql)
    {

        DataSet ds = new DataSet();
        SqlConnection conn = new SqlConnection(ConnectionString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(sql, conn);
        da.Fill(ds);
        return ds;
    }
};


class AcceptAllCertificatePolicy : ICertificatePolicy
{
    public AcceptAllCertificatePolicy()
    {
    }

    public bool CheckValidationResult(
    ServicePoint service_point,
    System.Security.Cryptography.X509Certificates.X509Certificate cert,
    WebRequest web_request,
    int certificate_problem)
    {
        // Always accept
        return true;
    }
}

