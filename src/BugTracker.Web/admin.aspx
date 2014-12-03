<%@ Page language="C#" CodeBehind="admin.aspx.cs" Inherits="btnet.admin" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<html>
<head>
<title id="titl" runat="server">btnet admin</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script>

var nagspan
var color
var hex_chars = "0123456789ABCDEF"

function decimal_to_hex(dec)
{
	var result = 
		hex_chars.charAt(Math.floor(dec / 16))
		+ hex_chars.charAt(dec % 16)
	return result
}

function RGB2HTML(red, green, blue)
{
	var rgb = "#"
	rgb += String(decimal_to_hex(red));
	rgb += String(decimal_to_hex(green));
	rgb += String(decimal_to_hex(blue));
	return rgb
}

function start_animation()
{
	nagspan = document.getElementById("nagspan")
// cc = 204, 66 = 102
	color = 1
	timer = setInterval(timer_callback,5)
}

function timer_callback()
{
	color += 1
	
	new_color = RGB2HTML(255, color * 2, color)
	
	nagspan.style.background = new_color
	
	if (color == 102) // if the color is now orange
	{
		clearInterval(timer)
	}
}

</script>
</head>
<body  <% if (nag) Response.Write("onload='start_animation()'"); %>>
<% security.write_menu(Response, "admin"); %>

<% if (nag) { %>

<script>
function donate()
{
	pp.submit()
}
</script>
<form name=pp id=pp action="https://www.paypal.com/cgi-bin/webscr" method="post">
<span id="nagspan" style='background: #ff0000; border: dotted 1px blue; padding: 5px;'>
Is BugTracker.NET helping you or your company?  Please consider <a href=javascript:donate()>donating</a>.  Thanks in advance.</span>&nbsp;<span class=smallnote>Change if(false) to if(true) near line 27 in admin.aspx to remove this.</span>
<input type="hidden" name="cmd" value="_s-xclick">
<input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----
">
</form>

<% } %>

<div class=align><table border=0><tr><td>
<ul>

<li class=listitem><a href=users.aspx>Users</a></li>
<li class=listitem><a href=orgs.aspx>Organizations</a></li>
<li class=listitem><a href=projects.aspx>Projects</a></li>
<li class=listitem><a href=categories.aspx>Categories</a></li>

<li class=listitem><a href=priorities.aspx>Priorities</a></li>

<li class=listitem><a href=statuses.aspx>Statuses</a></li>

<li class=listitem><a href=udfs.aspx>User Defined Attribute</a>
&nbsp;&nbsp;<span class=smallnote>(see "ShowUserDefinedBugAttribute" and "UserDefinedBugAttributeName" in Web.config)</span></li>
<li class=listitem><a href=customfields.aspx>Custom Fields</a>
&nbsp;&nbsp;<span class=smallnote>(add custom fields to the bug page)</span></li>

<li class=listitem><a href=notifications.aspx>Queued Email Notifications</a></li>

<li class=listitem><a href=edit_custom_html.aspx>Edit Custom Html</a></li>

<li class=listitem><a href=backup_db.aspx>Backup Database</a></li>
<li class=listitem><a href=manage_logs.aspx>Manage Logs</a></li>
    <li>
        <form runat="server" ID="SearchSettingsForm"><asp:Button ID="reindexLink" runat="server" OnClick="ReindexAllBugs" Text="Recreate Search Index"></asp:Button></form>
    </li>
</ul>
</td></tr></table>
<p>&nbsp;<p>
<p>Server Info:
<%
Response.Write ("<br>Path=");
Response.Write (HttpContext.Current.Server.MapPath(null));
Response.Write ("<br>MachineName=");
Response.Write (HttpContext.Current.Server.MachineName);
Response.Write ("<br>ScriptTimeout=");
Response.Write (HttpContext.Current.Server.ScriptTimeout);
Response.Write ("<br>.NET Version=");
Response.Write(Environment.Version.ToString());
Response.Write ("<br>CurrentCulture=");
Response.Write(System.Threading.Thread.CurrentThread.CurrentCulture.Name);

%>

</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>
