<%@ Page language="C#"%>
<%@ Import Namespace="System.IO" %>

<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">


DataSet ds;

Security security;

void Page_Init (object sender, EventArgs e) {ViewStateUserKey = Session.SessionID;}


void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.MUST_BE_ADMIN);

	ds = btnet.DbUtil.get_dataset(
		@"select
			'<a target=_blank href=edit_priority.aspx?id=' + convert(varchar,pr_id) + '>' + pr_name + '</a>' [priority],
			'<a target=_blank href=edit_status.aspx?id=' + convert(varchar,st_id) + '>' + st_name + '</a>' [status],
			isnull(pr_style,'') [priority CSS class],
			isnull(st_style,'') [status CSS class],
			isnull(pr_style + st_style,'datad') [combo CSS class - priority + status ],
			'<span class=''' + isnull(pr_style,'') + isnull(st_style,'')  +'''>The quick brown fox</span>' [text sample]
			from priorities, statuses /* intentioanl cartesian join */
			order by pr_sort_seq, st_sort_seq;

			select distinct isnull(pr_style + st_style,'datad')
			from priorities, statuses;");

	ArrayList classes_list = new ArrayList();
	foreach (DataRow dr_styles in ds.Tables[1].Rows)
	{

		classes_list.Add("." + (string) dr_styles[0]);
	}

	// create path
    string map_path = (string)HttpRuntime.Cache["MapPath"];
	string path = map_path + "\\custom\\btnet_custom.css";

	StringBuilder relevant_css_lines = new StringBuilder();

	ArrayList lines = new ArrayList();
	if (System.IO.File.Exists(path))
	{
		string line;
		StreamReader stream=File.OpenText(path);
		while((line = stream.ReadLine()) != null)
		{
			for (int i = 0; i < classes_list.Count; i++)
			{
				if (line.IndexOf((string)classes_list[i]) > -1)
				{
					relevant_css_lines.Append(line);
					relevant_css_lines.Append("<br>");
					lines.Add(line);
					break;
				}
			}
		}
		stream.Close();
	}

	relevant_lines.InnerHtml = relevant_css_lines.ToString();
}


</script>

<html>
<head>
<title id="titl" runat="server">btnet styles</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="sortable.js"></script>
</head>

<body>
<% security.write_menu(Response, "admin"); %>


<div class=align>

<div class="lbl" style="width: 600;">
The query "demo use of css classes" has as its first column a CSS class name that is
 composed of the priority's CSS class name concatenated with the status's CSS
 class name.  The SQL looks like this:
</div>
<p>
<div style="font-family: courier; font-weight: bold;">
	select <span style="color: red;">isnull(pr_style + st_style,'datad')</span>, bg_id [id], bg_short_desc [desc], .... etc
</div>
<p>
<div  class="lbl"  style="width: 600;">
Note that in the sql, where there isn't both a priority CSS class and a status CSS class
 available, the default CSS class name of "datad" is used.    The following list lets you see
 how all the different priority/status combinations will look.   Click on a link to edit
 a priority or a status.

</div>

<%

if (ds.Tables[0].Rows.Count > 0)
{
	SortableHtmlTable.create_from_dataset(
		Response, ds, "", "",false);

}
else
{
	Response.Write ("No priority/status combos in the database.");
}

%>

<div cls="lbl">Relevant lines from btnet_custom.css:</div>
<div  class="frm" style="width: 600px;" id="relevant_lines" runat="server">
</div>

</div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>