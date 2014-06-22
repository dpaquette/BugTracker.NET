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

	

	string guid = Request["id"];

	string sql = @"
declare @expiration datetime
set @expiration = dateadd(n,-$minutes,getdate())

select *,
	case when el_date < @expiration then 1 else 0 end [expired]
	from emailed_links
	where el_id = '$guid'

delete from emailed_links
	where el_date < dateadd(n,-240,getdate())";

	sql = sql.Replace("$minutes",Util.get_setting("RegistrationExpiration","20"));
	sql = sql.Replace("$guid",guid.Replace("'","''"));

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	if (dr == null)
	{
		msg.InnerHtml = "The link you clicked on is expired or invalid.<br>Please start over again.";
	}
	else if ((int) dr["expired"] == 1)
	{
		msg.InnerHtml = "The link you clicked has expired.<br>Please start over again.";
	}
	else
	{
		btnet.User.copy_user(
			(string) dr["el_username"],
			(string) dr["el_email"],
			(string) dr["el_firstname"],
			(string) dr["el_lastname"],
            "",
			(int) dr["el_salt"],
			(string) dr["el_password"],
            Util.get_setting("SelfRegisteredUserTemplate", "[error - missing user template]"),
            false);
		
		//  Delete the temp link
		sql = @"delete from emailed_links where el_id = '$guid'";
		sql = sql.Replace("$guid",guid.Replace("'","''"));
		btnet.DbUtil.execute_nonquery(sql);

		msg.InnerHtml = "Your registration is complete.";
	}

}


</script>

<html>
<head>
<title id="titl" runat="server">btnet change password</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<table border=0><tr>

<%

Response.Write (Application["custom_logo"]);

%>

</table>


<div align="center">
<table border=0><tr><td>

<div runat="server" class=err id="msg">&nbsp;</div>
<p>
<a href="default.aspx">Go to login page</a>

</td></tr></table>

</div>
</body>
</html>