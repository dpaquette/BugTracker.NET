<%@ Page language="C#"%>
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


// *****>>>>>> Intentionally not putting copyright in HTML comment, because of text/plain content type.
//Copyright 2002-2011 Corey Trager
//Distributed under the terms of the GNU General Public License


Security security;
string repo;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	Response.ContentType = "text/plain";
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

	// get info about revision

	string sql = @"
select svnrev_revision, svnrev_repository, svnap_path, svnrev_bug
from svn_revisions
inner join svn_affected_paths on svnap_svnrev_id = svnrev_id
where svnap_id = $id
order by svnrev_revision desc, svnap_path";

    int svnap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	string string_affected_path_id = Convert.ToString(svnap_id);

	sql = sql.Replace("$id", string_affected_path_id);

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	// check if user has permission for this bug
	int permission_level = Bug.get_bug_permission_level((int) dr["svnrev_bug"], security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}
	
	int revision = Convert.ToInt32(Request["rev"]);

	repo = (string) dr["svnrev_repository"];
    string path;
    if (btnet.Util.get_setting("SvnTrustPathsInUrls", "0") == "1")
    {
        path = Request["path"];
    }
    else
    {
        path = (string)dr["svnap_path"];
    }    
	
    string raw_text = VersionControl.svn_cat(repo, path, revision);

	Response.Write (raw_text);

}


</script>

