<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;

string left_out = "";
string right_out = "";
string unified_diff_text = "";
string left_title = "";
string right_title = "";
string path = "";

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);


	// get info about revision

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

	string error = "";

	string commit0 = Request["rev_0"];
	
	if (string.IsNullOrEmpty(commit0)) 
	{
		string commit = (string) dr["gitcom_commit"];
		
		unified_diff_text = VersionControl.git_get_unified_diff_one_commit(repo, commit, path);
	
		// get the source code for both the left and right
		string left_text = VersionControl.git_get_file_contents(repo, commit + "^", path);
	    string right_text = VersionControl.git_get_file_contents(repo, commit, path);
		left_title = commit + "^";	
		right_title = commit;
		
	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);
	    
	}
	else
	{

		string commit1 = Request["rev_1"];
		
		unified_diff_text = VersionControl.git_get_unified_diff_two_commits(repo, commit0, commit1, path);
	
		// get the source code for both the left and right
		string left_text = VersionControl.git_get_file_contents(repo, commit0, path);
	    string right_text = VersionControl.git_get_file_contents(repo, commit1, path);
		left_title = commit0;	
		right_title = commit1;

	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);

	}

	if (error != "")
	{
		Response.Write(HttpUtility.HtmlEncode(error));
		Response.End();
	}
}




</script>

<html>
<title>git diff <% Response.Write(HttpUtility.HtmlEncode(path));%></title>

<!-- #include file = "inc_diff.inc" -->