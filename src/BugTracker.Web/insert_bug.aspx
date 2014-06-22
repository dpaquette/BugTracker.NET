<%@ Page language="C#" validateRequest="false"%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<%@ Import Namespace="anmar.SharpMimeTools" %>
<%@ Assembly Name="SharpMimeTools" %>

<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">
        
void Page_Load(Object sender, EventArgs e)
{

	Util.set_context(HttpContext.Current);
    Util.do_not_cache(Response);

	string username = Request["username"];
	string password = Request["password"];
	string projectid_string = Request["projectid"];
	string comment = Request["comment"];
	string from_addr = Request["from"];
	string cc = "";
	string message = Request["message"];
	string attachment_as_base64 = Request["attachment"];
	string attachment_content_type = Request["attachment_content_type"];
	string attachment_filename = Request["attachment_filename"];
	string attachment_desc = Request["attachment_desc"];
	string bugid_string = Request["bugid"];
	string short_desc = Request["short_desc"];

	// this could also be the email subject
	if (short_desc == null)
	{
		short_desc = "";
	}
	else if (short_desc.Length > 200)
	{
		short_desc = short_desc.Substring(0,200);
	}

    SharpMimeMessage mime_message = null;  

	if (message != null && message.Length > 0)
	{
        mime_message = MyMime.get_sharp_mime_message(message);
     
        comment = MyMime.get_comment(mime_message);

        string headers = MyMime.get_headers_for_comment(mime_message);
        if (headers != "")
        {
            comment = headers + "\n" + comment;
        }
               
        from_addr = MyMime.get_from_addr(mime_message);

    }
	else
	{
		if (comment == null)
		{
			comment = "";
		}
	}


	if (username == null
	|| username == "")
	{
		Response.AddHeader("BTNET","ERROR: username required");
		Response.Write("ERROR: username required");
		Response.End();
	}

	if (password == null
	|| password == "")
	{
		Response.AddHeader("BTNET","ERROR: password required");
		Response.Write("ERROR: password required");
		Response.End();
	}

	// authenticate user

    bool authenticated = btnet.Authenticate.check_password(username, password);

    if (!authenticated)
    {
		Response.AddHeader("BTNET","ERROR: invalid username or password");
		Response.Write("ERROR: invalid username or password");
		Response.End();
    }

    Security security = MyMime.get_synthesized_security(mime_message, from_addr, username);
    
	int projectid = 0;
	if (Util.is_int(projectid_string))
	{
		projectid = Convert.ToInt32(projectid_string);
	}

	int bugid = 0;

	if (Util.is_int(bugid_string))
	{
		bugid = Convert.ToInt32(bugid_string);
	}


	// Even though btnet_service.exe has already parsed out the bugid,
	// we can do a better job here with SharpMimeTools.dll
	string subject = "";

	if (mime_message != null)
	{
        subject = MyMime.get_subject(mime_message);

        if (subject != "[No Subject]")
        {
            bugid = MyMime.get_bugid_from_subject(ref subject);
        }

        cc = MyMime.get_cc(mime_message);
	}
    
    string sql = "";
    
	if (bugid != 0)
	{
		// Check if the bug is still in the database
		// No comment can be added to merged or deleted bugids
		// In this case a new bug is created, this to prevent possible loss of information

		sql = @"select count(bg_id)
			from bugs
			where bg_id = $id";

		sql = sql.Replace("$id", Convert.ToString(bugid));

		if (Convert.ToInt32(btnet.DbUtil.execute_scalar(sql)) == 0)
		{
			bugid = 0;
		}
	}


	// Either insert a new bug or append a commment to existing bug
	// based on presence, absence of bugid
	if (bugid == 0)
	{
		// insert a new bug

		if (mime_message != null)
		{

			// in case somebody is replying to a bug that has been deleted or merged
			subject = subject.Replace(Util.get_setting("TrackingIdString","DO NOT EDIT THIS:"), "PREVIOUS:");

			short_desc = subject;
			if (short_desc.Length > 200)
			{
				short_desc = short_desc.Substring(0,200);
			}

		}

		int orgid = 0;
		int categoryid = 0;
		int priorityid = 0;
		int assignedid = 0;
		int statusid = 0;
		int udfid = 0;

        // You can control some more things from the query string
        if (Request["$ORGANIZATION$"] != null && Request["$ORGANIZATION$"] != "") { orgid = Convert.ToInt32(Request["$ORGANIZATION$"]); }
        if (Request["$CATEGORY$"] != null && Request["$CATEGORY$"] != "") { categoryid = Convert.ToInt32(Request["$CATEGORY$"]); }
        if (Request["$PROJECT$"] != null && Request["$PROJECT$"] != "") { projectid = Convert.ToInt32(Request["$PROJECT$"]); }
        if (Request["$PRIORITY$"] != null && Request["$PRIORITY$"] != "") { priorityid = Convert.ToInt32(Request["$PRIORITY$"]); }
        if (Request["$ASSIGNEDTO$"] != null && Request["$ASSIGNEDTO$"] != "") { assignedid = Convert.ToInt32(Request["$ASSIGNEDTO$"]); }
        if (Request["$STATUS$"] != null && Request["$STATUS$"] != "") { statusid = Convert.ToInt32(Request["$STATUS$"]); }
        if (Request["$UDF$"] != null && Request["$UDF$"] != "") { udfid = Convert.ToInt32(Request["$UDF$"]); }

        DataRow defaults = Bug.get_bug_defaults();

        // If you didn't set these from the query string, we'll give them default values
		if (projectid == 0) {projectid = (int) defaults["pj"];}
		if (orgid == 0) {orgid = security.user.org;}
		if (categoryid == 0) {categoryid = (int) defaults["ct"];}
		if (priorityid == 0) {priorityid = (int) defaults["pr"];}
		if (statusid == 0) {statusid = (int) defaults["st"];}
		if (udfid == 0) {udfid = (int) defaults["udf"];}

        // but forced project always wins
        if (security.user.forced_project != 0)
        {
            projectid = security.user.forced_project;
        }            
        
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
			"","","", // project specific dropdown values
			comment,
            comment,
			from_addr,
			cc,
			"text/plain",
			false, // internal only
			null, // custom columns
            false);  // suppress notifications for now - wait till after the attachments

		if (mime_message != null)
		{
			MyMime.add_attachments(mime_message, new_ids.bugid, new_ids.postid, security);

            MyPop3.auto_reply(new_ids.bugid, from_addr, short_desc, projectid);
            
		}
		else if (attachment_as_base64 != null && attachment_as_base64.Length > 0)
		{

			if (attachment_desc == null) attachment_desc = "";
			if (attachment_content_type == null) attachment_content_type = "";
			if (attachment_filename == null) attachment_filename = "";

			System.Byte[] byte_array = System.Convert.FromBase64String(attachment_as_base64);
			Stream stream = new MemoryStream(byte_array);

			Bug.insert_post_attachment(
				security,
				new_ids.bugid,
				stream,
				byte_array.Length,
				attachment_filename,
				attachment_desc,
				attachment_content_type,
				-1, // parent
				false, // internal_only
				false); // don't send notification yet
		}

        // your customizations
        Bug.apply_post_insert_rules(new_ids.bugid);
        
        btnet.Bug.send_notifications(btnet.Bug.INSERT, new_ids.bugid, security);
        btnet.WhatsNew.add_news(new_ids.bugid, short_desc, "added", security);

		Response.AddHeader("BTNET","OK:" + Convert.ToString(new_ids.bugid));
		Response.Write ("OK:" + Convert.ToString(new_ids.bugid));
		Response.End();

	}
	else // update existing bug
	{

		string StatusResultingFromIncomingEmail = Util.get_setting("StatusResultingFromIncomingEmail","0");

		sql = "";

		if (StatusResultingFromIncomingEmail != "0")
		{

			sql = @"update bugs
				set bg_status = $st
				where bg_id = $bg
				";

				sql = sql.Replace("$st", StatusResultingFromIncomingEmail);

		}

		sql += "select bg_short_desc from bugs where bg_id = $bg";

		sql = sql.Replace("$bg", Convert.ToString(bugid));
		DataRow dr2 = btnet.DbUtil.get_datarow(sql);


		// Add a comment to existing bug.
		int postid = btnet.Bug.insert_comment(
            bugid,
			security.user.usid, // (int) dr["us_id"],
		 	comment,
            comment,
		 	from_addr,
		 	cc,
		 	"text/plain",
		 	false); // internal only

		if (mime_message != null)
		{
            MyMime.add_attachments(mime_message, bugid, postid, security);
		}
		else if (attachment_as_base64 != null && attachment_as_base64.Length > 0)
		{

			if (attachment_desc == null) attachment_desc = "";
			if (attachment_content_type == null) attachment_content_type = "";
			if (attachment_filename == null) attachment_filename = "";

			System.Byte[] byte_array = System.Convert.FromBase64String(attachment_as_base64);
			Stream stream = new MemoryStream(byte_array);

			Bug.insert_post_attachment(
				security,
				bugid,
				stream,
				byte_array.Length,
				attachment_filename,
				attachment_desc,
				attachment_content_type,
				-1, // parent
				false, // internal_only
				false); // don't send notification yet
		}

		btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security);
		btnet.WhatsNew.add_news(bugid, (string) dr2["bg_short_desc"], "updated", security);

		Response.AddHeader("BTNET","OK:" + Convert.ToString(bugid));
		Response.Write ("OK:" + Convert.ToString(bugid));

		Response.End();
	}
}

</script>