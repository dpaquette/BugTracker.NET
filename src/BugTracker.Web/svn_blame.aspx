<%@ Page language="C#"%>
<%@ Import Namespace="System.Xml" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;

string blame_text;
string raw_text;
string path;
int revision;
string repo;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
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
	
	revision = Convert.ToInt32(Request["rev"]);
	

	repo = (string) dr["svnrev_repository"];

    if (btnet.Util.get_setting("SvnTrustPathsInUrls", "0") == "1")
    {
        path = Request["path"];
    }
    else
    {
        path = (string)dr["svnap_path"];
    }
	
    raw_text = VersionControl.svn_cat(repo, path, revision);

	if (raw_text.StartsWith("ERROR:"))
	{
        Response.Write(HttpUtility.HtmlEncode(raw_text));
		Response.End();
	}

    blame_text = VersionControl.svn_blame(repo, path, revision);

    if (blame_text.StartsWith("ERROR:"))
    {
        Response.Write(HttpUtility.HtmlEncode(blame_text));
        Response.End();
    }

}


///////////////////////////////////////////////////////////////////////
void write_blame()
{

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(blame_text);
    XmlNodeList commits = doc.GetElementsByTagName("commit");

	// split the source text into lines
	Regex regex = new Regex("\n");
	string[] lines = regex.Split(raw_text.Replace("\r\n","\n"));

    for (int i = 0; i < commits.Count; i++)
    {
        XmlElement commit = (XmlElement)commits[i];
        Response.Write("<tr><td nowrap>" + commit.GetAttribute("revision"));

        string author = "";
        string date = "";

        foreach (XmlNode node in commit.ChildNodes)
        {
            if (node.Name == "author") author = node.InnerText;
            else if (node.Name == "date") date = btnet.Util.format_db_date_and_time(XmlConvert.ToDateTime(node.InnerText, XmlDateTimeSerializationMode.Local));
        }

        Response.Write("<td nowrap>" + author);
        Response.Write("<td nowrap style='background: #ddffdd'><pre style='display:inline;'> " + HttpUtility.HtmlEncode(lines[i]));
        Response.Write(" </pre><td nowrap>" + date);

    }

}



</script>

<html>
<title>svn blame <% Response.Write(HttpUtility.HtmlEncode(path) + "@" + Convert.ToString(revision));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<body>
<p>
<pre>
<table border=0 class=datat cellspacing=0 cellpadding=0>
<tr>
<td class=datah>revision
<td class=datah>author
<td class=datah>text
<td class=datah>date
<% write_blame(); %>
</table>
</pre>
</body>
</html>