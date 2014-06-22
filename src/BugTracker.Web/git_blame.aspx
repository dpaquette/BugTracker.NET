<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;
string blame_text;
string path;
string commit;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	string sql = @"
select gitcom_commit, gitcom_bug, gitcom_repository, gitap_path 
from git_commits
inner join git_affected_paths on gitap_gitcom_id = gitcom_id
where gitap_id = $id";

    int gitap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	sql = sql.Replace("$id", Convert.ToString(gitap_id));

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	// check if user has permission for this bug
	int permission_level = Bug.get_bug_permission_level((int) dr["gitcom_bug"], security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}
	
	string repo = (string) dr["gitcom_repository"];
	path = (string) dr["gitap_path"];
	commit = Request["commit"];
	
    blame_text = VersionControl.git_blame(repo, path, commit);

}


///////////////////////////////////////////////////////////////////////
void write_blame(string blame_text)
{
/*
f36d6c45 (corey 2009-10-04 19:44:42 -0500  1) asdfasdf
f36d6c45 (corey 2009-10-04 19:44:42 -0500  2) asdf
9f3ac5e7 (corey 2009-10-04 19:46:05 -0500  3) asdfab
*/

	Regex regex = new Regex("\n");
	string[] lines = regex.Split(blame_text);
	
	
	
	for (int i = 0; i < lines.Length; i++)
	{
		
		if (lines[i].Length > 40)
		{
			string commit;
			string author;
			string text;
			string date;

			commit = lines[i].Substring(0,8);
			int pos = lines[i].IndexOf(" ",11); // position of space after author
			author = lines[i].Substring(10,pos-10);
			date = lines[i].Substring(pos + 1, 19);
			pos = lines[i].IndexOf(")",40);
			text = lines[i].Substring(pos+2);

			Response.Write("<tr><td>");
			Response.Write(commit);
			Response.Write("&nbsp;<td nowrap>" + author);
			Response.Write("<td nowrap style='background: #ddffdd'><pre style='display:inline;'> " + HttpUtility.HtmlEncode(text));
			Response.Write(" </pre><td nowrap>" + date);
		}
	}
}



</script>

<html>
<title>git blame <% Response.Write(commit + " -- " + HttpUtility.HtmlEncode(path));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<body>
<p>
<pre>
<table border=0 class=datat cellspacing=0 cellpadding=0>
<tr>
<td class=datah>commit
<td class=datah>author
<td class=datah>text
<td class=datah>date
<% write_blame(blame_text); %>
</table>
</pre>
</body>
</html>