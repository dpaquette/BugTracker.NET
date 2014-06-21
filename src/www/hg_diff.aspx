<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="System.Xml" %>
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
select hgrev_revision, hgrev_bug, hgrev_repository, hgap_path 
from hg_revisions
inner join hg_affected_paths on hgap_hgrev_id = hgrev_id
where hgap_id = $id";

    int hgap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	sql = sql.Replace("$id", Convert.ToString(hgap_id));

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	// check if user has permission for this bug
	int permission_level = Bug.get_bug_permission_level((int) dr["hgrev_bug"], security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}


	string repo = (string) dr["hgrev_repository"];
	path = (string) dr["hgap_path"];

	string error = "";

	string revision0 = Request["rev_0"];
	
	if (string.IsNullOrEmpty(revision0)) 
	{
		string revision = Convert.ToString((int) dr["hgrev_revision"]);
		
		// we need to find the previous revision
		string log = VersionControl.hg_log(repo, revision, path);
		string prev_revision = get_previous_revision(log, revision);
		
		if (prev_revision == "")
		{
			Response.Write ("unable to determine previous revision from log");
			Response.End();
		}
		
		unified_diff_text = VersionControl.hg_get_unified_diff_two_revisions(repo, prev_revision, revision, path);
	
		// get the source code for both the left and right
		string left_text = VersionControl.hg_get_file_contents(repo, prev_revision, path);

	    string right_text = VersionControl.hg_get_file_contents(repo, revision, path);
		left_title = prev_revision;	
		right_title = revision;
		
	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);
	    
	}
	else
	{

		string revision1 = Request["rev_1"];
		
		unified_diff_text = VersionControl.hg_get_unified_diff_two_revisions(repo, revision0, revision1, path);
	
		// get the source code for both the left and right
		string left_text = VersionControl.hg_get_file_contents(repo, revision0, path);
	    string right_text = VersionControl.hg_get_file_contents(repo, revision1, path);
		left_title = revision0;	
		right_title = revision1;

	    error = VersionControl.visual_diff(unified_diff_text, left_text, right_text, ref left_out, ref right_out);

	}

	if (error != "")
	{
		Response.Write(HttpUtility.HtmlEncode(error));
		Response.End();
	}
}


///////////////////////////////////////////////////////////////////////
string get_previous_revision(string log_result, string this_revision)
{

    XmlDocument doc = new XmlDocument();
    doc.LoadXml("<log>" + log_result + "</log>");
    XmlNodeList revisions = doc.GetElementsByTagName("changeset");

	// read backwards
    if (revisions.Count > 1)
    {
        XmlElement changeset = (XmlElement)revisions[revisions.Count-2];
        
		return changeset.GetAttribute("rev");
    }
    else
    {
	    return "";
	}

}


</script>

<html>
<title>hg diff <% Response.Write(HttpUtility.HtmlEncode(path));%></title>

<!-- #include file = "inc_diff.inc" -->
