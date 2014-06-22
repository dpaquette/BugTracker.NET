<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">

/*
The following is an explanation of Unified Diff Format from this web page, from Guido van Rossum, the Python guy:
http://www.artima.com/weblogs/viewpost.jsp?thread=164293

The header lines look like this:

indicator ' ' filename '\t' date ' ' time ' ' timezone

where:

		* indicator is '---' for the old file and '+++' for the new
		* date has the form YYYY-MM-DD
		* time has the form hh:mm:ss.nnnnnnnnn on a 24-hour clock
		* timezone is has the form ('+'|'-') hhmm where hhmm is hours and minutes east
		(if the sign is +) or west (if the sign is -) of GMT/UTC

Each chunk starts with a line that looks like this:

'@@ -' range ' +' range ' @@'

where range is either one unsigned decimal number or two separated by a comma.
The first number is the start line of the chunk in the old or new file.
The second number is chunk size in that file;
it and the comma are omitted if the chunk size is 1.
If the chunk size is 0, the first number is one lower than one would expect
(it is the line number after which the chunk should be inserted or deleted;
in all other cases it gives the first line number or the replaced range of lines).

A chunk then continues with lines starting with
' ' (common line),
'-' (only in old file), or
'+' (only in new file).

If the last line of a file doesn't end in a newline character,
it is displayed with a newline character, and the following line in the chunk has
the literal text (starting in the first column):

'\ No newline at end of file'

*/



Security security;

string left_out = "";
string right_out = "";
string unified_diff_text = "";
string left_title = "";
string right_title = "";
string path0 = "";
string path1 = "";


string repo;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	string sql = @"
select svnrev_revision, svnrev_repository, svnap_path, svnrev_bug
from svn_revisions
inner join svn_affected_paths on svnap_svnrev_id = svnrev_id
where svnap_id = $id";

    int svnap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	sql = sql.Replace("$id", Convert.ToString(svnap_id));

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	// check if user has permission for this bug
	int permission_level = Bug.get_bug_permission_level((int) dr["svnrev_bug"], security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}
	
	repo = (string) dr["svnrev_repository"];

    if (btnet.Util.get_setting("SvnTrustPathsInUrls", "0") == "1")
    {
        path0 = Request["path_0"];
        path1 = Request["path_1"];
    }
    else
    {
        path0 = path1 = (string)dr["svnap_path"];
    }

	string error = "";

	string string_revision0 = Request["rev_0"];
	
	if (string.IsNullOrEmpty(string_revision0)) 
	{
		int revision = (int) dr["svnrev_revision"];
		
		unified_diff_text = VersionControl.svn_diff(repo, path1, revision, 0);
        examine_diff(unified_diff_text);
        
		// get the old revision number
		Regex regex = new Regex("\n");
		string[] diff_lines = regex.Split(unified_diff_text.Replace("\r\n","\n"));
		
		string line = diff_lines[2];
		int old_rev_pos1 = line.ToLower().IndexOf("(revision ");  // 10 chars long
		int old_rev_pos_start_of_int = old_rev_pos1 + 10;
		int old_rev_after_int = line.IndexOf(")",old_rev_pos_start_of_int);
		string old_revision_string = line.Substring(old_rev_pos_start_of_int,
			old_rev_after_int - old_rev_pos_start_of_int);

		int old_revision = Convert.ToInt32(old_revision_string);

	
		// get the source code for both the left and right
        string left_text = VersionControl.svn_cat(repo, path0, old_revision);
        string right_text = VersionControl.svn_cat(repo, path1, revision);
		left_title = Convert.ToString(old_revision);	
		right_title = Convert.ToString(revision);
		
	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);
	    
	}
	else
	{


		int revision1 = Convert.ToInt32(Request["rev_1"]);
		int revision0 = Convert.ToInt32(string_revision0);

        unified_diff_text = VersionControl.svn_diff(repo, path1, revision1, revision0);
        examine_diff(unified_diff_text);
        
		// get the source code for both the left and right
        string left_text = VersionControl.svn_cat(repo, path0, revision0);
        string right_text = VersionControl.svn_cat(repo, path1, revision1);
		left_title = Convert.ToString(revision0);	
		right_title = Convert.ToString(revision1);

	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);

	}
    
	if (error != "")
	{
		Response.Write(HttpUtility.HtmlEncode(error));
		Response.End();
	}

}

///////////////////////////////////////////////////////////////////////
void examine_diff(string unified_diff_text)
{

    if (unified_diff_text == "")
    {
        Response.Write("No differences.");
        Response.End();
    }

    int error_pos = unified_diff_text.IndexOf("Cannot display: file marked as a binary type.");
    if (error_pos > -1)
    {
        Response.Write("<div style='color:red; font-weight: bold; font-size: 10pt;'>");
        Response.Write(unified_diff_text.Substring(error_pos));
        Response.Write("<br>Subversion thinks this is a binary file.</div>");
        Response.End();
    }
}
   

</script>

<html>
<title>svn diff <% Response.Write(HttpUtility.HtmlEncode(path1));%></title>

<!-- #include file = "inc_diff.inc" -->
