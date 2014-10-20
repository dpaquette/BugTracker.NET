<%@ Page language="C#" CodeBehind="svn_hook.aspx.cs" Inherits="btnet.svn_hook" validateRequest="false" AutoEventWireup="True" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<%@ Import Namespace="System.Xml" %>

<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

        
///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.set_context(HttpContext.Current);
	Util.do_not_cache(Response);

	string username = Request["username"];
	string password = Request["password"];
	
	string svn_log = Request["svn_log"];
	string repo = Request["repo"];
	

	if (username == null
	|| username == "")
	{
		Response.AddHeader("BTNET","ERROR: username required");
		Response.Write("ERROR: username required");
		Response.End();
	}
	
	
	if (username != btnet.Util.get_setting("SvnHookUsername",""))
	{
		Response.AddHeader("BTNET","ERROR: wrong username. See Web.config svnHookUsername");
		Response.Write("ERROR: wrong username. See Web.config svnHookUsernam");
		Response.End();
	}

	if (password == null
	|| password == "")
	{
		Response.AddHeader("BTNET","ERROR: password required");
		Response.Write("ERROR: password required");
		Response.End();
	}

	// authenticate user

    bool authenticated = btnet.Authenticate.check_password(username, password);

    if (!authenticated)
    {
		Response.AddHeader("BTNET","ERROR: invalid username or password");
		Response.Write("ERROR: invalid username or password");
		Response.End();
    }


	btnet.Util.write_to_log("svn_log follows");
	btnet.Util.write_to_log(svn_log);
	
	btnet.Util.write_to_log("repo follows");
	btnet.Util.write_to_log(repo);

	
    XmlDocument doc = new XmlDocument();

    doc.LoadXml(svn_log);
    XmlNodeList revisions = doc.GetElementsByTagName("logentry");


    for (int i = 0; i < revisions.Count; i++)
    {
        XmlElement logentry = (XmlElement)revisions[i];
        
        string msg = logentry.GetElementsByTagName("msg")[0].InnerText;
		string revision = logentry.GetAttribute("revision");
		string author = logentry.GetElementsByTagName("author")[0].InnerText;
		string date = logentry.GetElementsByTagName("date")[0].InnerText;

        string bugids = get_bugids_from_msg(msg);

        if (bugids == "")
        {
            bugids = "0";
        }

        foreach (string bugid in bugids.Split(','))
        {
            if (btnet.Util.is_int(bugid))
            {
                insert_revision_row_per_bug(bugid, repo, revision, author, date, msg, logentry);
            }
        }

    } // end for each revision
		
	
	Response.Write ("OK:");
	Response.End();
}

void insert_revision_row_per_bug(string bugid, string repo, string revision, string author, string date, string msg, XmlElement logentry)
{

    var sql = new SQLString(@"
declare @cnt int
select @cnt = count(1) from svn_revisions 
where svnrev_revision = @svnrev_revision
and svnrev_repository = @svnrev_repository
and svnrev_bug = $svnrev_bug

if @cnt = 0 
BEGIN
insert into svn_revisions
(
	svnrev_revision,
	svnrev_bug,
	svnrev_repository,
	svnrev_author,
	svnrev_svn_date,
	svnrev_btnet_date,
	svnrev_msg
)
values
(
	@svnrev_revision,
	$svnrev_bug,
	@svnrev_repository,
	@svnrev_author,
	@svnrev_svn_date,
	getdate(),
	@svnrev_msg
)

select scope_identity()
END	
ELSE
select 0
");

    sql = sql.Replace("svnrev_revision", revision);
    sql = sql.Replace("svnrev_bug", bugid);
    sql = sql.Replace("svnrev_repository", repo);
    sql = sql.Replace("svnrev_author", author);
    sql = sql.Replace("svnrev_svn_date", date);
    sql = sql.Replace("svnrev_msg", msg);

    int svnrev_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

    if (svnrev_id > 0)
    {
        XmlNodeList paths = logentry.GetElementsByTagName("path");

        for (int j = 0; j < paths.Count; j++)
        {

            XmlElement path_element = (XmlElement)paths[j];

            string action = path_element.GetAttribute("action");
            string file_path = path_element.InnerText;


            sql = new SQLString(@"
insert into svn_affected_paths
(
svnap_svnrev_id,
svnap_action,
svnap_path
)
values
(
@svnap_svnrev_id,
@svnap_action,
@svnap_path
)");

            sql = sql.Replace("svnap_svnrev_id", Convert.ToString(svnrev_id));
            sql = sql.Replace("svnap_action", action);
            sql = sql.Replace("svnap_path", file_path);

            btnet.DbUtil.execute_nonquery(sql);

        } // end for each path
    }  // if we inserted a revision
}




string get_bugids_from_msg(string msg)
{
	string without_line_breaks = msg.Replace("\r\n","").Replace("\n","");

    string regex_pattern1 = btnet.Util.get_setting("SvnBugidRegexPattern1", "([0-9,]+$)");  // at end

    Regex reIntegerAtEnd = new Regex(regex_pattern1);
	Match m = reIntegerAtEnd.Match(without_line_breaks);
	
	if (m.Success)
	{
		return m.Groups[1].ToString();
	}
	else
	{
        string regex_pattern2 = btnet.Util.get_setting("SvnBugidRegexPattern2", "(^[0-9,]+ )");  // comma delimited at start
        Regex reIntegerAtStart = new Regex(regex_pattern2);
		Match m2 = reIntegerAtStart.Match(without_line_breaks);
		
        if (m2.Success)
		{
            string bugids = m2.Groups[1].ToString().Trim();
            btnet.Util.write_to_log("bugids string: " + bugids);
            return bugids;
		}
		else
		{
			return "";
		}
	}
}

</script>
