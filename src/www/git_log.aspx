<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;

string log_result;
string repo;
string file_path;
string string_affected_path_id;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);


	// get info about commit

	string sql = @"
select gitcom_repository, gitcom_commit, gitap_path, gitcom_bug
from git_commits
inner join git_affected_paths on gitap_gitcom_id = gitcom_id
where gitap_id = $id
order by gitcom_commit desc, gitap_path";

    int gitap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	
	string_affected_path_id = Convert.ToString(gitap_id);
	sql = sql.Replace("$id", string_affected_path_id);

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	
	// check if user has permission for this bug
	int bugid = (int) dr["gitcom_bug"];
	
	int permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}

    revpathid.Value = string_affected_path_id;

	repo = (string) dr["gitcom_repository"];
	string commit = (string) dr["gitcom_commit"];
	file_path = (string) dr["gitap_path"];
	
	log_result = VersionControl.git_log(repo, commit, file_path);
	
}


///////////////////////////////////////////////////////////////////////
void write_line(int row, string commit, string author, string date, string path, string action, string msg)
{
	Response.Write("<tr><td class=datad>" + commit);
	Response.Write("<td class=datad>" + author);
	Response.Write("<td class=datad>" + date);
	Response.Write("<td class=datad>" + path);
	Response.Write("<td class=datad>" + action);
	Response.Write("<td class=datad>" + msg.Replace(Environment.NewLine, "<br/>"));

    Response.Write("<td class=datad><a target=_blank href=git_view.aspx?revpathid=" + string_affected_path_id 
		+ "&commit=" + commit
		+ ">");
		
	Response.Write("view</a>");

    Response.Write("<td class=datad><a target=_blank href=git_blame.aspx?revpathid=" + string_affected_path_id 
		+ "&commit=" + commit
		+ ">");
		
	Response.Write("annotated</a>");

	Response.Write("<td class=datad><a id=" + commit
		+ " href='javascript:sel_for_diff("
		+ Convert.ToString(row)
		+ ",\"" 
		+ commit
        + "\",\"\")'>select for diff</a>");
}


///////////////////////////////////////////////////////////////////////
void fetch_and_write_history()
{

/*
commit 789e948bce733dab9605bf8eb51584e3b9a2eba3
Author: corey <ctrager@yahoo.com>
Date:   2009-10-11 21:54:14 -0500

    123 just 8 lines

M	dir1/file3.txt

commit 0b77adbedfab04185a3c1d33afe25aa330e91518
Author: corey <ctrager@yahoo.com>
Date:   2009-10-11 21:24:12 -0500

    123 just 8 lines

M	dir1/file3.txt

*/
	Regex regex = new Regex("\n");
	string[] lines = regex.Split(log_result);

	string commit = "";
	string author = "";
	string date = "";
	string path = "";
	string action = "";
	string msg = "";
	int row = 0;
	
	for (int i = 0; i < lines.Length; i++)
	{
		if (lines[i].StartsWith("commit "))
		{
			if (commit != "")
			{
				write_line(++row, commit, author, date, path, action, msg);
				commit = "";
				author = "";
				date = "";
				path = "";
				action = "";
				msg = "";
			}
		
			commit = lines[i].Substring(7);
		}
		else if (lines[i].StartsWith("Author: "))
		{
			author = Server.HtmlEncode(lines[i].Substring(8));
		}
		else if (lines[i].StartsWith("Date: "))
		{
			date = lines[i].Substring(8,19);
		}
		else if (lines[i].StartsWith("    "))
		{
			if (msg != "")
			{
				msg += Environment.NewLine;
			}
			msg += lines[i].Substring(4);
		}
		else if (lines[i].Length > 1 && lines[i][1] == '\t')
		{
			action = lines[i].Substring(0,1);
			path = lines[i].Substring(2);
		}

    }

	if (commit != "")
	{
		write_line(++row, commit, author, date, path, action, msg);
	}
}



</script>

<html>
<title>git log <% Response.Write(HttpUtility.HtmlEncode(file_path));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="version_control_sel_rev.js"></script>
<body>
<p>
<form id="frm" target=_blank action="git_diff.aspx" method="GET">

<input type=hidden name="rev_0" id="rev_0" value="0"></input>
<input type=hidden name="rev_1" id="rev_1" value="0"></input>
<input type=hidden name="revpathid" id="revpathid" value="" runat="server"></input>

</form>

<p>

<table border=1 class=datat>
<tr>
<td class=datah>commit
<td class=datah>author
<td class=datah>date
<td class=datah>path
<td class=datah>action<br>
<td class=datah>msg
<td class=datah>view
<td class=datah>view<br>annotated<br>(git blame)
<td class=datah>
<span></span><a
style="display: none; background: yellow;
border-top: 1px silver solid;
border-left: 1px silver solid;
border-bottom: 2px black solid;
border-right: 2px black solid;
"
id="do_diff_enabled" href="javascript:on_do_diff()">click<br>to<br>diff</a>
<a style="color: red;"   id="do_diff_disabled" href="javascript:on_do_diff()">select<br>two<br>commits</a></span>
<% fetch_and_write_history(); %>

</table>

</body>

</html>
