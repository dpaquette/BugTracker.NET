<%@ Page language="C#"%>
<%@ Import Namespace="System.IO" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

int bugid;
Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "add attachment";

	string string_id = Util.sanitize_integer(Request.QueryString["id"]);

	if (string_id == null || string_id == "0")
	{
		write_msg("Invalid id.", false);
		Response.End();
		return;
	}
	else
	{
		bugid = Convert.ToInt32(string_id);
		int permission_level = Bug.get_bug_permission_level(bugid, security);
		if (permission_level == Security.PERMISSION_NONE
		|| permission_level == Security.PERMISSION_READONLY)
		{
			write_msg("You are not allowed to edit this item", false);
			Response.End();
			return;
		}
	}


	if (security.user.external_user || Util.get_setting("EnableInternalOnlyPosts","0") == "0")
	{
		internal_only.Visible = false;
		internal_only_label.Visible = false;
	}
	
	if (IsPostBack)
	{
		on_update();
	}
}


///////////////////////////////////////////////////////////////////////
void write_msg(string msg, bool rewrite_posts)
{
	string script = "script"; // C# compiler doesn't like s c r i p t
	Response.Write("<" + script + ">");
	Response.Write("function foo() {");
	Response.Write("parent.set_msg('");
	Response.Write(msg);
	Response.Write("'); ");
	
	if (rewrite_posts)
	{
		Response.Write 	("parent.opener.rewrite_posts(" + Convert.ToString(bugid) + ")");
	}
	Response.Write("}</" + script + ">");
	Response.Write("<html><body onload='foo()'>");
	Response.Write("</body></html>");
	Response.End();
}

///////////////////////////////////////////////////////////////////////
void on_update()
{

	
	if (attached_file.PostedFile == null)
	{
		write_msg("Please select file", false);
		return;
	}

	string filename = System.IO.Path.GetFileName(attached_file.PostedFile.FileName);
	if (string.IsNullOrEmpty(filename))
	{
		write_msg("Please select file", false);
		return;
	}

	int max_upload_size = Convert.ToInt32(Util.get_setting("MaxUploadSize","100000"));
	int content_length = attached_file.PostedFile.ContentLength;
	if (content_length > max_upload_size)
	{
		write_msg("File exceeds maximum allowed length of "
			+ Convert.ToString(max_upload_size)
			+ ".", false);
		return;
	}

	if (content_length == 0)
	{
		write_msg("No data was uploaded.", false);
		return;
	}

	bool good = false;
	
	try
	{
        Bug.insert_post_attachment(
            security,
			bugid,
			attached_file.PostedFile.InputStream,
			content_length,
			filename,
			desc.Value,
			attached_file.PostedFile.ContentType,
			-1, // parent
			internal_only.Checked,
			true);
			
		good = true;			

	}
	catch (Exception ex)
	{
		write_msg("caught exception:" + ex.Message, false);
		return;
	}


	if (good)
	{
		write_msg(
			filename 
			+ " was successfully upload ("
			+ attached_file.PostedFile.ContentType
			+ "), "
			+ Convert.ToString(content_length)
			+ " bytes"
			,true);
	}
	else
	{
		// This should never happen....
		write_msg("Unexpected error with file upload.", false);
	}

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet add attachment</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">

<script>

function set_msg(s)
{
	document.getElementById("msg").innerHTML = s
	document.getElementById("file_input").innerHTML 
		='<input type=file class=txt name="attached_file" id="attached_file" maxlength=255 size=60>'
}

function waiting()
{
	document.getElementById("msg").innerHTML = "Uploading..."
	return true
}

</script>
</head>

<body>

<iframe name="hiddenframe" style="display:none" >x</iframe>

<div class=align>

Add attachment to <% Response.Write(Convert.ToString(bugid)); %>
<p>
	<table border=0><tr><td>
		<form target="hiddenframe" class=frm runat="server" enctype="multipart/form-data" onsubmit="return waiting()">
			<table border=0>

			<tr>
			<td class=lbl>Description:</td>
			<td><input runat="server" type=text class=txt id="desc" maxlength=80 size=80></td>
			<td runat="server" class=err id="desc_err">&nbsp;</td>
			</tr>

			<tr>
			<td class=lbl>File:</td>
			<td><div id="file_input">
			<input runat="server" type=file class=txt id="attached_file" maxlength=255 size=60>
			</div>
			</td>
			<td runat="server" class=err id="attached_file_err">&nbsp;</td>
			</tr>

			<tr>
			<td colspan=3>
			<asp:checkbox runat="server" class=cb id="internal_only"/>
			<span runat="server" id="internal_only_label">Visible to internal users only</span>
			</td>
			</tr>

			<tr><td colspan=3 align=left>
			<span runat="server" class=err id="msg">&nbsp;</span>
			</td></tr>

			<tr>
			<td colspan=2 align=center>
			<input runat="server" class=btn type=submit id="sub" value="Upload">
			</td>
			</tr>
			</table>
		</form>
	</td></tr></table>
</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>