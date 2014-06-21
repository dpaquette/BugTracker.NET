<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.set_context(HttpContext.Current);
	Util.do_not_cache(Response);

	if (Util.get_setting("ShowForgotPasswordLink","0") == "0")
	{
		Response.Write("Sorry, Web.config ShowForgotPasswordLink is set to 0");
		Response.End();
	}

	if (!IsPostBack)
	{
		titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "forgot password";
	}
	else
	{
		msg.InnerHtml = "";

		if (email.Value == "" && username.Value == "")
		{
			msg.InnerHtml = "Enter either your Username or your Email address.";
		}
		else if (email.Value != "" && !Util.validate_email(email.Value))
		{
			msg.InnerHtml = "Format of email address is invalid.";
		}
		else
		{

			int user_count = 0;
			int user_id = 0;
			
			if (email.Value != "" && username.Value == "")
			{

				// check if email exists
				user_count = (int) btnet.DbUtil.execute_scalar(
					"select count(1) from users where us_email = N'" + email.Value.Replace("'","''") + "'");
					
				if (user_count == 1)
				{
					user_id = (int) btnet.DbUtil.execute_scalar(
						"select us_id from users where us_email = N'" + email.Value.Replace("'","''") + "'");
				}
				
				
			}
			else if (email.Value == "" && username.Value != "")
			{
				// check if email exists
				user_count = (int) btnet.DbUtil.execute_scalar(
					"select count(1) from users where isnull(us_email,'') != '' and  us_username = N'" + username.Value.Replace("'","''") + "'");

				if (user_count == 1)
				{
					user_id = (int) btnet.DbUtil.execute_scalar(
						"select us_id from users where us_username = N'" + username.Value.Replace("'","''") + "'");
				}
			}			
			else if (email.Value != "" && username.Value != "")
			{
				// check if email exists
				user_count = (int) btnet.DbUtil.execute_scalar(
					"select count(1) from users where us_username = N'" + username.Value.Replace("'","''") + "' and us_email = N'"
					+ email.Value.Replace("'","''") + "'");

				if (user_count == 1)
				{
					user_id = (int) btnet.DbUtil.execute_scalar(
						"select us_id from users where us_username = N'" + username.Value.Replace("'","''") + "' and us_email = N'"
						+ email.Value.Replace("'","''") + "'");
				}
			}			


			if (user_count == 1)
			{
				string guid = Guid.NewGuid().ToString();
				string sql = @"
declare @username nvarchar(255)
declare @email nvarchar(255)

select @username = us_username, @email = us_email
	from users where us_id = $user_id

insert into emailed_links
	(el_id, el_date, el_email, el_action, el_user_id)
	values ('$guid', getdate(), @email, N'forgot', $user_id)

select @username us_username, @email us_email";

				sql = sql.Replace("$guid",guid);
				sql = sql.Replace("$user_id",Convert.ToString(user_id));

				DataRow dr = btnet.DbUtil.get_datarow(sql);

				string result = btnet.Email.send_email(
					(string) dr["us_email"],
					Util.get_setting("NotificationEmailFrom",""),
					"", // cc
					"reset password",

					"Click to <a href='"
						+ Util.get_setting("AbsoluteUrlPrefix","")
						+ "change_password.aspx?id="
						+ guid
						+ "'>reset password</a> for user \""
						+ (string) dr["us_username"]
						+ "\".",

                    BtnetMailFormat.Html);

				if (result == "")
				{
					msg.InnerHtml = "An email with password info has been sent to you.";
				}
				else
				{
					msg.InnerHtml = "There was a problem sending the email.";
					msg.InnerHtml += "<br>" + result;
				}
			}
			else
			{
				msg.InnerHtml = "Unknown username or email address.<br>Are you sure you spelled everything correctly?<br>Try just username, just email, or both.";
			}
		}
	}
}

</script>

<html>
<head>
<title id="titl" runat="server">btnet forgot password</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body onload="document.forms[0].email.focus()">
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>

<form class=frm runat="server">
	<table border=0>

	<tr>
	<td colspan=2 class=smallnote>Enter Username or Email or both</td>
	</tr>

	<tr>
	<td colspan=2>&nbsp;</td>
	</tr>

	<tr>
	<td class=lbl>Username:</td>
	<td><input runat="server" type=text class=txt id="username" size=40 maxlength=40></td>
	</tr>

	<tr>
	<td class=lbl>Email:</td>
	<td><input runat="server" type=text class=txt id="email" size=40 maxlength=40></td>
	</tr>

	<tr><td colspan=2 align=left>
	<span runat="server" class=err id="msg">&nbsp;</span>
	</td></tr>

	<tr><td colspan=2 align=center>
	<input class=btn type=submit value="Send password info to my email" runat="server">
	</td></tr>

	</table>
</form>

<a href="default.aspx">Return to login page</a>

</td></tr></table>

</div>
</body>
</html>