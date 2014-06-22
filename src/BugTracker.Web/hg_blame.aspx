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
string revision;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
	security = new Security();
	security.check_security( HttpContext.Current, Security.ANY_USER_OK);

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
	revision = Request["rev"];
	
    blame_text = VersionControl.hg_blame(repo, path, revision);

}

///////////////////////////////////////////////////////////////////////
void write_blame(string blame_text)
{
    Response.Write (HttpUtility.HtmlEncode(blame_text));
}

</script>

<html>
<title>hg blame <% Response.Write(HttpUtility.HtmlEncode(revision) + " -- " + HttpUtility.HtmlEncode(path));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<body>
<p>
<pre>
<% write_blame(blame_text); %>
</pre>
</body>
</html>