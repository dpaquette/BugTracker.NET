<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

String sql;

Security security;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	if (Request.QueryString["ses"] != (string) Session["session_cookie"])
	{
		Response.Write ("session in URL doesn't match session cookie");
		Response.End();
	}

	if (Request.QueryString["actn"] == "delete")
	{
		sql = @"delete from queued_notifications where qn_status = N'not sent'";
		btnet.DbUtil.execute_nonquery(sql);
	}
	else if (Request.QueryString["actn"] == "reset")
	{
		sql = @"update queued_notifications set qn_retries = 0 where qn_status = N'not sent'";
		btnet.DbUtil.execute_nonquery(sql);
	}
	else if (Request.QueryString["actn"] == "resend")
	{
		// spawn a worker thread to send the emails
        System.Threading.Thread thread = new System.Threading.Thread(btnet.Bug.threadproc_notifications);
		thread.Start();
	}


	Response.Redirect("notifications.aspx");
}



</script>