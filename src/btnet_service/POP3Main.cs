using System;
using System.Collections.Generic;
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
using OpenPop.Mime;
using OpenPop.Pop3;


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

        List<string> messages = null;
        Regex regex = new Regex("\r\n");
        using (Pop3Client client = new Pop3Client())
        {
            try
            {
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

                write_line("Connecting to pop3 server");
                client.Connect(Pop3Server, port, use_ssl);
                write_line("Autenticating");
                client.Authenticate(user, password);


                write_line("Getting list of documents");
                messages = client.GetMessageUids();
            }
            catch (Exception e)
            {
                write_line("Exception trying to talk to pop3 server");
                write_line(e);
                return;
            }


            int message_number = 0;


            // loop through the messages
            for (int i = 0; i < messages.Count - 1; i++)
            {
                heartbeat_datetime = DateTime.Now; // because the watchdog is watching

                if (state != service_state.STARTED)
                {
                    break;
                }

                // fetch the message

                write_line("Getting Message:" + messages[i]);
                message_number = Convert.ToInt32(messages[i]);
                Message mimeMessage = client.GetMessage(message_number);

                // for diagnosing problems
                if (MessageOutputFile != "")
                {
                    File.WriteAllBytes(MessageOutputFile, mimeMessage.RawMessage);
                }

                // break the message up into lines

                string from = mimeMessage.Headers.From.Address;
                string subject = mimeMessage.Headers.Subject;




                write_line("\nFrom: " + from);

                write_line("Subject: " + subject);

                if (!string.IsNullOrEmpty(SubjectMustContain) && subject.IndexOf(SubjectMustContain, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    write_line("skipping because subject does not contain: " + SubjectMustContain);
                    continue;
                }

                bool bSkip = false;
                foreach (string subjectCannotContainString in SubjectCannotContainStrings)
                {
                    if (!string.IsNullOrEmpty(subjectCannotContainString))
                    {
                        if (subject.IndexOf(subjectCannotContainString, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            write_line("skipping because subject cannot contain: " + subjectCannotContainString);
                            bSkip = true;
                            break;  // done checking, skip this message
                        }
                    }
                }

                if (bSkip)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(FromMustContain) && from.IndexOf(FromMustContain, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    write_line("skipping because from does not contain: " + FromMustContain);
                    continue; // that is, skip to next message
                }

                foreach (string fromCannotContainStrings in FromCannotContainStrings)
                {
                    if (!string.IsNullOrEmpty(fromCannotContainStrings))
                    {
                        if (from.IndexOf(fromCannotContainStrings, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            write_line("skipping because from cannot contain: " + fromCannotContainStrings);
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
                if (string.IsNullOrEmpty(TrackingIdString))
                {
                    bugidString = "DO NOT EDIT THIS:";
                }

                int pos = subject.IndexOf(bugidString, StringComparison.OrdinalIgnoreCase);

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

                string rawMessage = Encoding.Default.GetString(mimeMessage.RawMessage);
                string post_data = "username=" + HttpUtility.UrlEncode(ServiceUsername)
                                   + "&password=" + HttpUtility.UrlEncode(ServicePassword)
                                   + "&projectid=" + Convert.ToString(projectid)
                                   + "&from=" + HttpUtility.UrlEncode(from)
                                   + "&short_desc=" + HttpUtility.UrlEncode(subject)
                                   + "&message=" + HttpUtility.UrlEncode(rawMessage);

                byte[] bytes = Encoding.UTF8.GetBytes(post_data);


                // send request to web server
                HttpWebResponse res = null;
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);


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
                            client.DeleteMessage(message_number);
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
                client.Disconnect();
            }
            else
            {
                write_line("\nclosing input file " + MessageInputFile);
            }
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

