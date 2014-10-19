<%@ Page language="C#" CodeBehind="query.aspx.cs" Inherits="btnet.queryPage" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

DataSet ds;

Security security;

string exception_message;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	

	// If there is a users table, then authenticate this page
	try
	{
		btnet.DbUtil.execute_nonquery(new SQLString("select count(1) from users"));
		security = new Security();
		security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);
	}
	catch (Exception)
	{
	}

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "run query";


    if (IsPostBack)
    {
        if (query.Value != "")
        {
            try
            {
                ds = btnet.DbUtil.get_dataset(new SQLString(Server.HtmlDecode(query.Value)));
            }
            catch (Exception e2)
            {
                exception_message = e2.Message;
                //exception_message = e2.ToString();  // uncomment this if you need more error info.
            }
        }
    }
    else
    {
        DataSet ds = btnet.DbUtil.get_dataset(new SQLString("select name from sysobjects where type = 'u' order by 1"));
        dbtables_select.Items.Add("Select Table");
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            dbtables_select.Items.Add((string)dr[0]);
        }
    }
}


</script>

<html>
<head>
<title id="titl" runat="server">btnet query</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>

<script>

var shown = true;

function showhide_form()
{
	var frm =  document.getElementById("<% Response.Write(Util.get_form_name()); %>");
	if (shown)
	{
		frm.style.display = "none";
		shown = false;
		showhide.firstChild.nodeValue = "show form";
	}
	else
	{
		frm.style.display = "block";
		shown = true;
		showhide.firstChild.nodeValue = "hide form";
	}
}

function on_dbtables_changed()
{
    var tables_sel = document.getElementById("dbtables_select")
    selected_text = tables_sel.options[tables_sel.selectedIndex].text;
    if (selected_text != "Select Table") 
    {
        document.getElementById("query").value = "select * from " + selected_text;
    }
}
</script>
</head>

<body>

<div class=align>
<table border=0>

<tr>

	<td style="border: red solid 2px;
		background: yellow; color: #ff0000;
		font-weight: bold;
		font-size: 8pt;
		padding: 4px;">

This page is not safe on a public web server.
After you install BugTracker.NET on a public web server, please delete it.

<tr>
	<td>
		<select id="dbtables_select" runat="server" onchange="on_dbtables_changed()"/>
		<div style="float:right;"><a href='javascript:showhide_form()' id='showhide'>hide form</a></span>

<tr>
	<td>

		<form class=frm action="query.aspx" method="POST" runat="server">
			Or enter SQL:
			<br>
			<textarea rows=15 cols=70 runat="server" id="query"></textarea>
			<p>
			<input runat="server" type=submit value="Execute SQL">
		</form>

</table></div>

<%

if (exception_message != "")
{
    Response.Write("<span class=err>" + exception_message + "</span><br>");
}

if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "");

}
else
{
	Response.Write ("No Rows");
}
%>
</body>
</html>


