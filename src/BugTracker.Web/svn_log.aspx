<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<%@ Import Namespace="System.Xml" %>
<!-- #include file = "inc.aspx" -->
<script language="C#" runat="server">


Security security;

string file_path;
int rev;
string log;
string repo;

string string_affected_path_id;

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
	string_affected_path_id = Convert.ToString(svnap_id);

	sql = sql.Replace("$id", string_affected_path_id);

	DataRow dr = btnet.DbUtil.get_datarow(sql);

	// check if user has permission for this bug
	int permission_level = Bug.get_bug_permission_level((int) dr["svnrev_bug"], security);
	if (permission_level == Security.PERMISSION_NONE) {
		Response.Write("You are not allowed to view this item");
		Response.End();
	}

    revpathid.Value = string_affected_path_id;

	
	repo = (string) dr["svnrev_repository"];
	file_path = (string) dr["svnap_path"];
    rev = (int)dr["svnrev_revision"];


	log = VersionControl.svn_log(repo, file_path, rev);

	if (log.StartsWith("ERROR:"))
	{
		Response.Write(HttpUtility.HtmlEncode(log));
		Response.End();
	}

}


///////////////////////////////////////////////////////////////////////
void fetch_and_write_history(string file_path)
{

    XmlDocument doc = new XmlDocument();
    doc.LoadXml(log);
    XmlNode log_node = doc.ChildNodes[1];
    //string adjusted_file_path = "/" + file_path; // when/why did this stop working?
    string adjusted_file_path = file_path;
    
    int row = 0;
    foreach (XmlElement logentry in log_node)
    {

        string revision = logentry.GetAttribute("revision");
        string author = "";
        string date = "";
        string path = "";
        string action = "";
        //string copy_from = "";
        //string copy_from_rev = "";
        string msg = "";

        foreach (XmlNode node in logentry.ChildNodes)
        {
            if (node.Name == "author") author = node.InnerText;
            else if (node.Name == "date") date = btnet.Util.format_db_date_and_time(XmlConvert.ToDateTime(node.InnerText, XmlDateTimeSerializationMode.Local));
            else if (node.Name == "msg") msg = node.InnerText;
            else if (node.Name == "paths")
            {
                foreach (XmlNode path_node in node.ChildNodes)
                {
                    if (path_node.InnerText == adjusted_file_path)
                    {
                        XmlElement path_el = (XmlElement)path_node;
                        action = path_el.GetAttribute("action");
                        if (!action.Contains("D"))
                        {
                            path = path_node.InnerText;
                            path = adjusted_file_path;
                            if (path_el.GetAttribute("copyfrom-path") != "")
                            {
                                adjusted_file_path = path_el.GetAttribute("copyfrom-path");
                            }
                        }
                    }
                }
            }
        }

        Response.Write("<tr><td class=datad>" + revision);
        Response.Write("<td class=datad>" + author);
        Response.Write("<td class=datad>" + date);
        Response.Write("<td class=datad>" + path);
        Response.Write("<td class=datad>" + action);
//        Response.Write("<td class=datad>" + copy_from);
//        Response.Write("<td class=datad>" + copy_from_rev);
        Response.Write("<td class=datad>" + msg.Replace(Environment.NewLine, "<br/>"));


        Response.Write("<td class=datad><a target=_blank href=svn_view.aspx?revpathid=" + string_affected_path_id 
			+ "&rev=" + revision
            + "&path=" + HttpUtility.UrlEncode(path)
			+ ">");

		Response.Write("view</a>");

        Response.Write("<td class=datad><a target=_blank href=svn_blame.aspx?revpathid=" + string_affected_path_id 
			+ "&rev=" + revision
            + "&path=" + HttpUtility.UrlEncode(path)
			+ ">");

		Response.Write("annotated</a>");

		Response.Write("<td class=datad><a id=" + revision 
			+ " href='javascript:sel_for_diff("
			+ Convert.ToString(row)
			+ ",\"" 
			+ revision
            + "\",\""
            + path
			+ "\")'>select for diff</a>");


    }
}



</script>

<html>
<title>svn log <% Response.Write(HttpUtility.HtmlEncode(file_path));%></title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
<script type="text/javascript" language="JavaScript" src="version_control_sel_rev.js"></script>
<body>

<form id="frm" target=_blank action="svn_diff.aspx" method="GET">

<input type=hidden name="rev_0" id="rev_0" value="0"></input>
<input type=hidden name="rev_1" id="rev_1" value="0"></input>
<input type=hidden name="path_0" id="path_0" value=""></input>
<input type=hidden name="path_1" id="path_1" value=""></input>
<input type=hidden name="revpathid" id="revpathid" value="" runat="server"></input>

</form>

<p>

<table border=1 class=datat>
<tr>
<td class=datah>revision
<td class=datah>author
<td class=datah>date
<td class=datah>path
<td class=datah>action<br>
<td class=datah>msg
<td class=datah>view
<td class=datah>view<br>annotated<br>(svn blame)
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
<% fetch_and_write_history(file_path); %>

</table>

</body>

</html>
