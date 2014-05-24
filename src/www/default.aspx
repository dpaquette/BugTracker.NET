<%@ Page language="C#" validateRequest="false"%>
<%@ Import Namespace="System.Data.SqlClient" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">



string sql;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.set_context(HttpContext.Current);

	Util.do_not_cache(Response);

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "logon";

	msg.InnerText = "";

	// see if the connection string works
	try
	{
		// Intentionally getting an extra connection here so that we fall into the right "catch"
		SqlConnection conn = btnet.DbUtil.get_sqlconnection();
        conn.Close();

		try
		{
			btnet.DbUtil.execute_nonquery("select count(1) from users");

		}
		catch (SqlException e1)
		{
			Util.write_to_log (e1.Message);
			Util.write_to_log (Util.get_setting("ConnectionString","?"));
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
	string auth_mode = Util.get_setting("WindowsAuthentication","0");
	HttpCookie username_cookie = Request.Cookies["user"];
	string previous_auth_mode = "0";
	if (username_cookie!=null) {
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
    	if ( previous_auth_mode == "1" && auth_mode == "2" )
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
		if ( previous_auth_mode == "0" )
		{
			if ((Request.QueryString["user"] == null) || (Request.QueryString["password"]  == null))
			{
				//	User name and password are not on the querystring.

				if (username_cookie != null)
				{
					//	Set the user name from the last logon.

				user.Value = username_cookie["name"];
				}
			}
			else
			{
				//	User name and password have been passed on the querystring.

				user.Value = Request.QueryString["user"];
				pw.Value = Request.QueryString["password"];

				on_logon();
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

	string auth_mode = Util.get_setting("WindowsAuthentication","0");
	if ( auth_mode != "0" ) {
		if (user.Value.Trim() == "") {
			btnet.Util.redirect("loginNT.aspx", Request, Response);
		}
	}

    bool authenticated = btnet.Authenticate.check_password(user.Value, pw.Value);

    if (authenticated)
    {
        sql = "select us_id from users where us_username = N'$us'";
    	sql = sql.Replace("$us", user.Value.Replace("'","''"));
    	DataRow dr = btnet.DbUtil.get_datarow(sql);
        if (dr != null)
        {
            int us_id = (int)dr["us_id"];

            btnet.Security.create_session(
            	Request,
            	Response,
                us_id,
                user.Value,
                "0");

            btnet.Util.redirect(Request, Response);
        }
        else
        {
            // How could this happen?  If someday the authentication
            // method uses, say LDAP, then check_password could return
            // true, even though there's no user in the database";
            msg.InnerText = "User not found in database";
        }
	}
	else
	{
		msg.InnerText = "Invalid User or Password.";
	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet logon</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<link rel="shortcut icon" href="favicon.ico">
</head>
<body onload="document.forms[0].user.focus()">
<div style="float: right;">
<span>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href=http://ifdefined.com/bugtrackernet.html>BugTracker.NET</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href=http://ifdefined.com/README.html>Help</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href=mailto:ctrager@yahoo.com>Feedback</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href=about.html>About</a>
	<br>
	<a target=_blank style=" font-size: 7pt; font-family: arial; letter-spacing: 1px;" href=http://ifdefined.com/README.html>Donate</a>
</span>
</div>
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>
<form class=frm runat="server">
	<table border=0>

	<% if (Util.get_setting("WindowsAuthentication", "0") != "0") { %>
		<tr><td colspan="2" class="smallnote">To login using your Windows ID, leave User blank</td></tr>
	<% } %>
	<tr>
	<td class=lbl>User:</td>
	<td><input runat="server" type=text class=txt id="user"></td>
	</tr>

	<tr>
	<td class=lbl>Password:</td>
	<td><input runat="server" type=password class=txt id="pw"></td>
	</tr>

	<tr><td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr><td colspan=2 align=center>
	<input class=btn type=submit value="Logon" runat="server">
	</td></tr>

	</table>
</form>

<span>

<% if (Util.get_setting("AllowGuestWithoutLogin","0") == "1") { %>
<p>
<a style="font-size: 8pt;"href="bugs.aspx">Continue as "guest" without logging in</a>
<p>
<% } %>

<% if (Util.get_setting("AllowSelfRegistration","0") == "1") { %>
<p>
<a style="font-size: 8pt;"href="register.aspx">Register</a>
<p>
<% } %>

<% if (Util.get_setting("ShowForgotPasswordLink","1") == "1") { %>
<p>
<a style="font-size: 8pt;"href="forgot.aspx">Forgot your username or password?</a>
<p>
<% } %>


</span>

</td></tr></table>

<% Response.Write (Application["custom_welcome"]); %>
</div>
</body>
</html>