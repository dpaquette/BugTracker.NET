<%@ Page language="C#" validateRequest="false"%>
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
	
	string hg_log = Request["hg_log"];
	string repo = Request["repo"];
	

	if (username == null
	|| username == "")
	{
		Response.AddHeader("BTNET","ERROR: username required");
		Response.Write("ERROR: username required");
		Response.End();
	}
	
	
	if (username != btnet.Util.get_setting("MercurialHookUsername",""))
	{
		Response.AddHeader("BTNET","ERROR: wrong username. See Web.config MercurialHookUsername");
		Response.Write("ERROR: wrong username. See Web.config MercurialHookUsernam");
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


	btnet.Util.write_to_log("hg_log follows");
	btnet.Util.write_to_log(hg_log);
	
	btnet.Util.write_to_log("repo follows");
	btnet.Util.write_to_log(repo);

	
    XmlDocument doc = new XmlDocument();

    doc.LoadXml("<log>" + hg_log + "</log>");
    XmlNodeList revisions = doc.GetElementsByTagName("changeset");


    for (int i = 0; i < revisions.Count; i++)
    {
        XmlElement changeset = (XmlElement)revisions[i];
        
        string desc = changeset.GetElementsByTagName("desc")[0].InnerText;
		string bug = get_bugid_from_desc(desc);

        if (bug == "")
        {
            bug = "0";
            
        }

		string revision = changeset.GetAttribute("rev");
		string author = changeset.GetElementsByTagName("auth")[0].InnerText;
		string date = changeset.GetElementsByTagName("date")[0].InnerText;


	    string sql = @"
declare @cnt int
select @cnt = count(1) from hg_revisions 
where hgrev_revision = '$hgrev_revision'
and hgrev_repository = N'$hgrev_repository'

if @cnt = 0 
BEGIN
insert into hg_revisions
(
	hgrev_revision,
	hgrev_bug,
	hgrev_repository,
	hgrev_author,
	hgrev_hg_date,
	hgrev_btnet_date,
	hgrev_msg
)
values
(
	$hgrev_revision,
	$hgrev_bug,
	N'$hgrev_repository',
	N'$hgrev_author',
	N'$hgrev_hg_date',
	getdate(),
	N'$hgrev_desc'
)

select scope_identity()
END	
ELSE
select 0
";

		sql = sql.Replace("$hgrev_revision",revision.Replace("'","''"));
		sql = sql.Replace("$hgrev_bug", Convert.ToString(bug));
		sql = sql.Replace("$hgrev_repository",repo.Replace("'","''"));
		sql = sql.Replace("$hgrev_author",author.Replace("'","''"));
		sql = sql.Replace("$hgrev_hg_date",date.Replace("'","''"));
		sql = sql.Replace("$hgrev_desc",desc.Replace("'","''"));

		int hgrev_id =  Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

		if (hgrev_id > 0)
		{
			XmlNodeList paths = changeset.GetElementsByTagName("file");

			for (int j = 0; j < paths.Count; j++)
			{

				XmlElement path_element = (XmlElement) paths[j];

				string action = ""; // no action in hg?  path_element.GetAttribute("action");
				string file_path = path_element.InnerText;


				sql = @"
insert into hg_affected_paths
(
hgap_hgrev_id,
hgap_action,
hgap_path
)
values
(
$hgap_hgrev_id,
N'$hgap_action',
N'$hgap_path'
)";		

				sql = sql.Replace("$hgap_hgrev_id", Convert.ToString(hgrev_id));
				sql = sql.Replace("$hgap_action", action.Replace("'","''"));
				sql = sql.Replace("$hgap_path", file_path.Replace("'","''"));

				btnet.DbUtil.execute_nonquery(sql);

			} // end for each path
		}  // if we inserted a revision
    } // end for each revision
		
	
	Response.Write ("OK:");
	Response.End();
}


string get_bugid_from_desc(string desc)
{
    string regex_pattern = btnet.Util.get_setting("MercurialBugidRegexPattern", "(^[0-9]+)");
    Regex reInteger = new Regex(regex_pattern);
	Match m = reInteger.Match(desc);
	if (m.Success)
	{
		return m.Groups[1].ToString();
	}
	else
	{
		return "";
	}
	
}

</script>