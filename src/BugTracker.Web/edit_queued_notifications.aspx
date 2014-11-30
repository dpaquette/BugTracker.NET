<%@ Page language="C#" CodeBehind="edit_queued_notifications.aspx.cs" Inherits="btnet.edit_queued_notifications" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

SQLString sql;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	if (Request.QueryString["ses"] != (string) Session["session_cookie"])
	{
		Response.Write ("session in URL doesn't match session cookie");
		Response.End();
	}

	if (Request.QueryString["actn"] == "delete")
	{
		sql = new SQLString(@"delete from queued_notifications where qn_status = N'not sent'");
		btnet.DbUtil.execute_nonquery(sql);
	}
	else if (Request.QueryString["actn"] == "reset")
	{
		sql = new SQLString(@"update queued_notifications set qn_retries = 0 where qn_status = N'not sent'");
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
