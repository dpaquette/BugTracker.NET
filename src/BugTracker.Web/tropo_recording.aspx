<%@ Page language="C#" CodeBehind="tropo_recording.aspx.cs" Inherits="btnet.tropo_recording"  validateRequest="false" AutoEventWireup="True" %>
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">

void Page_Load(Object sender, EventArgs e)
{

// to do - have a lock so that there's no race condition between wave and transcript
        
    if (Request.RequestType == "POST")
    {

        string username = btnet.Util.get_setting("TropoUsername", "admin");
        Security security = MyMime.get_synthesized_security(null, null, username);
        
        // new or existing bug?
        string key = Request.QueryString["time"];
        int bugid = 0;
        object obj = Application["tropo-" + key];
        if (obj != null)
        {
            bugid = (int)obj;
        }

        string which = Request.QueryString["which"];

        if (bugid == 0)
        {
            // create a new bug from either the recording or transcript, which comes in first
            bugid = create_new_bug(security);
            Application["tropo-" + key] = bugid;            
        }
       
        if (which == "recording")
        {
            // append the recording
                            
            HttpPostedFile file = Request.Files[0];
            int len = file.ContentLength;

            string attachment_desc = "recording";
            string attachment_filename = file.FileName;
            string attachment_content_type = file.ContentType;  // doesn't seem to work, so..

            if (attachment_filename.EndsWith(".wav"))
                attachment_content_type = "audio/wav";
            else
                attachment_content_type = "audio/mpeg";

            Bug.insert_post_attachment(
                security,
                bugid,
                file.InputStream,
                file.ContentLength,
                attachment_filename,
                attachment_desc,
                attachment_content_type,
                -1, // parent
                false, // internal_only
                false); // don't send notification yet

        }
        else // transcript
        {

            System.IO.Stream stream = Request.InputStream;
            System.Text.Encoding encoding = Request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(stream, encoding);
            string json = reader.ReadToEnd();

            //  {"result":{"transcription":"One 23 testing bye 3.","guid":"8e9d71a0-f8e4-012d-6648-12313d064a99","identifier":"1294003437"}}
            
            string transcript_start = "transcription\":";
            string transcript_end = ",\"guid";
            
            int pos_start = json.IndexOf(transcript_start);
            pos_start += transcript_start.Length;
            int pos_end = json.IndexOf(transcript_end);

            if (pos_start > -1 && pos_end > pos_start)
            {

                string comment = json.Substring(pos_start, pos_end - pos_start);
                                
                // Add a comment to existing bug.
                int postid = btnet.Bug.insert_comment(
                    bugid,
                    security.user.usid, // (int) dr["us_id"],
                    comment,
                    comment,
                    null, //from_addr,
                    "", //cc,
                    "text/plain",
                    false); // internal only
            }
            else
            {
                btnet.Util.write_to_log("empty transcript");
            }        
        }

        Response.AddHeader("BTNET", "OK:" + Convert.ToString(bugid));
        Response.Write("OK:" + Convert.ToString(bugid));
        Response.End();

    }
}
        
int create_new_bug(Security security)
{

    DataRow defaults = Bug.get_bug_defaults();

    // If you didn't set these from the query string, we'll give them default values
    int projectid = (int)defaults["pj"];
    int orgid = security.user.org;
    int categoryid = (int)defaults["ct"];
    int priorityid = (int)defaults["pr"];
    int statusid = (int)defaults["st"];
    int udfid = (int)defaults["udf"];
    int assignedid = 0;

    string short_desc = "Call from " + Request["from"];
    btnet.Util.write_to_log(short_desc);

    btnet.Bug.NewIds new_ids = btnet.Bug.insert_bug(
        short_desc,
        security,
        "", // tags
        projectid,
        orgid,
        categoryid,
        priorityid,
        statusid,
        assignedid,
        udfid,
        "", "", "", // project specific dropdown values
        "", // comment
        "", // comment
        "", // from_addr,
        "", // cc,
        "text/plain",
        false, // internal only
        null, // custom columns
        false);  // suppress notifications for now - wait till after the attachments           

    // your customizations
    Bug.apply_post_insert_rules(new_ids.bugid);

    btnet.Bug.send_notifications(btnet.Bug.INSERT, new_ids.bugid, security);
    btnet.WhatsNew.add_news(new_ids.bugid, short_desc, "added", security);        
    
    return new_ids.bugid;    
}

/*
<form method="POST" action="tropo_recording.aspx?time=999&from=1112223333" enctype="multipart/form-data">
<input type="file" name="filename"><br>
<input type="submit">
</form>
*/   
    
</script>
<form method="POST" action="tropo_recording.aspx?time=999&from=1112223333" enctype="multipart/form-data">
<input type="file" name="filename"><br>
<input type="submit">
</form>
