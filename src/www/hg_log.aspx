<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="System.Xml" %>
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


	// get info about revision

	string sql = @"
select hgrev_repository, hgrev_revision, hgap_path, hgrev_bug
from hg_revisions
inner join hg_affected_paths on hgap_hgrev_id = hgrev_id
where hgap_id = $id
order by hgrev_revision desc, hgap_path";

    int hgap_id = Convert.ToInt32(Util.sanitize_integer(Request["revpathid"]));
	
	string_affected_path_id = Convert.ToString(hgap_id);
	sql = sql.Replace("$id", string_affected_path_id);

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	
	// check if user has permission for this bug
	int bugid = (int) dr["hgrev_bug"];
	
	int permission_level = Bug.get_bug_permission_level(bugid, security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}

    revpathid.Value = string_affected_path_id;

	repo = (string) dr["hgrev_repository"];
	string revision = Convert.ToString((int) dr["hgrev_revision"]);
	file_path = (string) dr["hgap_path"];
	
	log_result = VersionControl.hg_log(repo, revision, file_path);

}

///////////////////////////////////////////////////////////////////////
void fetch_and_write_history()
{


    XmlDocument doc = new XmlDocument();
    doc.LoadXml("<log>" + log_result + "</log>");
    
    XmlNodeList revisions = doc.GetElementsByTagName("changeset");

	int row = 0;

	// read backwards
    for (int i = revisions.Count - 1; i > -1;  i--)
    {
        XmlElement changeset = (XmlElement)revisions[i];
        
		string revision = changeset.GetAttribute("node");
		string author = changeset.GetElementsByTagName("auth")[0].InnerText;
		string date = changeset.GetElementsByTagName("date")[0].InnerText;
        string desc = changeset.GetElementsByTagName("desc")[0].InnerText;
        string path = changeset.GetElementsByTagName("file")[0].InnerText;

        Response.Write("<tr><td class=datad>" + revision);
        Response.Write("<td class=datad>" + author);
        Response.Write("<td class=datad>" + date);
        Response.Write("<td class=datad>" + path);
//        Response.Write("<td class=datad>" + action);
//        Response.Write("<td class=datad>" + copy_from);
//        Response.Write("<td class=datad>" + copy_from_rev);

        Response.Write("<td class=datad>" + desc.Replace(Environment.NewLine, "<br/>"));


        Response.Write("<td class=datad><a target=_blank href=hg_view.aspx?revpathid=" + string_affected_path_id 
			+ "&rev=" + revision
			+ ">");

		Response.Write("view</a>");

        Response.Write("<td class=datad><a target=_blank href=hg_blame.aspx?revpathid=" + string_affected_path_id 
			+ "&rev=" + revision
			+ ">");

		Response.Write("annotated</a>");

		Response.Write("<td class=datad><a id=" + revision 
			+ " href='javascript:sel_for_diff("
			+ Convert.ToString(++row)
			+ ",\"" 
			+ revision 
			+ "\",\"\")'>select for diff</a>");


    }

}


</script>

<html>
<title>hg log <% Response.Write(HttpUtility.HtmlEncode(file_path));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="version_control_sel_rev.js"></script>
<body>
<p>
<form id="frm" target=_blank action="hg_diff.aspx" method="GET">

<input type=hidden name="rev_0" id="rev_0" value="0"></input>
<input type=hidden name="rev_1" id="rev_1" value="0"></input>
<input type=hidden name="revpathid" id="revpathid" value="" runat="server"></input>

</form>

<p>

<table border=1 class=datat>
<tr>
<td class=datah>revision
<td class=datah>author
<td class=datah>date
<td class=datah>path
<!--<td class=datah>action<br>-->
<td class=datah>msg
<td class=datah>view
<td class=datah>view<br>annotated<br>(hg blame)
<td class=datah>
<span></span><a
style="display: none; background: yellow;
border-top: 1px silver solid;
border-left: 1px silver solid;
border-bottom: 2px black solid;
border-right: 2px black solid;
"
id="do_diff_enabled" href="javascript:on_do_diff()">click<br>to<br>diff</a>
<a style="color: red;"   id="do_diff_disabled" href="javascript:on_do_diff()">select<br>two<br>revisions</a></span>
<% fetch_and_write_history(); %>

</table>

</body>

</html>
