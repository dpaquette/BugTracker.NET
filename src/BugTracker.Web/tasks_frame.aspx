<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

string string_bugid;

void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
			+ "tasks";
	
	string_bugid = Util.sanitize_integer(Request["bugid"]);
	
}
</script>

<html>
<head>
<title id="titl" runat="server">btnet tasks</title>

<script>
function set_task_cnt(cnt)
{
	opener.set_task_cnt(<% Response.Write(string_bugid);	%>,cnt)
}
</script>
</head>
<iframe width=100% height=100% frameborder=0 scrolling=yes src=tasks.aspx?bugid=<%Response.Write(string_bugid);%>>
</html>