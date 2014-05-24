<%@ Page language="C#" validateRequest="false"%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>

<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

        
///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.set_context(HttpContext.Current);
	Util.do_not_cache(Response);
	

	string username = Request["username"];
	string password = Request["password"];
	
	string git_log = Request["git_log"];
	string repo = Request["repo"];
	

	if (username == null
	|| username == "")
	{
		Response.AddHeader("BTNET","ERROR: username required");
		Response.Write("ERROR: username required");
		Response.End();
	}
	
	
	if (username != btnet.Util.get_setting("GitHookUsername",""))
	{
		Response.AddHeader("BTNET","ERROR: wrong username. See Web.config GitHookUsername");
		Response.Write("ERROR: wrong username. See Web.config GitHookUsernam");
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


	btnet.Util.write_to_log("git_log follows");
	btnet.Util.write_to_log(git_log);
	
	btnet.Util.write_to_log("repo follows");
	btnet.Util.write_to_log(repo);

	Regex regex = new Regex("\n");
	string [] lines = regex.Split(git_log);
	
	int bug = 0;
	string commit = null;
	string author = null;
	string date = null;
	string msg = "";
	
	List<string> actions = new List<string>();
	List<string> paths = new List<string>();

    string regex_pattern = btnet.Util.get_setting("GitBugidRegexPattern", "(^[0-9]+)");
    Regex reInteger = new Regex(regex_pattern);
	
	for (int i = 0; i < lines.Length; i++)
	{
		if (lines[i].StartsWith("commit "))
		{
			if (commit != null)
			{
				update_db(bug, repo, commit, author, date, msg, actions, paths);
				msg = "";
				bug = 0;
				actions.Clear();
				paths.Clear();
				
			}
			
			commit = lines[i].Substring(7);
		}
		else if (lines[i].StartsWith("Author: "))
		{
			author = lines[i].Substring(8);
		}
		else if (lines[i].StartsWith("Date:"))
		{
			date = lines[i].Substring(5).Trim();
		}
		else if (lines[i].StartsWith("    "))
		{
			if (msg != "")
			{
				msg += Environment.NewLine;
			}
			else
			{
				Match m = reInteger.Match(lines[i].Substring(4));
				if (m.Success)
				{
					bug = Convert.ToInt32(m.Groups[1].ToString());
				}
				
			}
			msg += lines[i].Substring(4);
		}
		else if (lines[i].Length > 1 && lines[i][1] == '\t')
		{
			actions.Add(lines[i].Substring(0,1));
			paths.Add(lines[i].Substring(2));
		}
	}
	
	if (commit != null)
	{
		update_db(bug, repo, commit, author, date, msg, actions, paths);
	}
	
	
	Response.Write ("OK:");

	Response.End();
	
}


void update_db(int bug, string repo, string commit, string author, string date, string msg, List<string> actions, List<string> paths)
{
	btnet.Util.write_to_log(commit);
	btnet.Util.write_to_log(author);
	btnet.Util.write_to_log(date);
	btnet.Util.write_to_log(msg);

	/*
	
	Because the python script sends us not just the most recent commit, but the most recent N commits, we need
	to have logic here not to do dupe inserts.
	
	*/

	string sql = @"

declare @cnt int
select @cnt = count(1) from git_commits 
where gitcom_commit = '$gitcom_commit'
and gitcom_repository = N'$gitcom_repository'

if @cnt = 0 
BEGIN
	insert into git_commits
	(
		gitcom_commit,
		gitcom_bug,
		gitcom_repository,
		gitcom_author,
		gitcom_git_date,
		gitcom_btnet_date,
		gitcom_msg
	)
	values
	(
		'$gitcom_commit',
		$gitcom_bug,
		N'$gitcom_repository',
		N'$gitcom_author',
		N'$gitcom_git_date',
		getdate(),
		N'$gitcom_msg'
	)

	select scope_identity()
END	
ELSE
	select 0

";
	
	sql = sql.Replace("$gitcom_commit",commit.Replace("'","''"));
	sql = sql.Replace("$gitcom_bug", Convert.ToString(bug));
	sql = sql.Replace("$gitcom_repository",repo.Replace("'","''"));
	sql = sql.Replace("$gitcom_author",author.Replace("'","''"));
	sql = sql.Replace("$gitcom_git_date",date.Replace("'","''"));
	sql = sql.Replace("$gitcom_msg",msg.Replace("'","''"));

	int gitcom_id =  Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));	

	if (gitcom_id != 0)
	{
		string gitcom_id_string = Convert.ToString(gitcom_id);

		btnet.Util.write_to_log(Convert.ToString(gitcom_id));	

		for (int i = 0; i < actions.Count; i++)
		{
			sql = @"
insert into git_affected_paths
(
gitap_gitcom_id,
gitap_action,
gitap_path
)
values
(
$gitap_gitcom_id,
N'$gitap_action',
N'$gitap_path'
)
	";		

			sql = sql.Replace("$gitap_gitcom_id", gitcom_id_string);
			sql = sql.Replace("$gitap_action", actions[i]);
			sql = sql.Replace("$gitap_path", paths[i].Replace("'","''"));

			btnet.DbUtil.execute_nonquery(sql);
		}
	}
}


/*

commit 9f3ac5e774db118189edf2f4c77554c7b4afff63
Author: corey <ctrager@yahoo.com>
Date:   Sun Oct 4 19:46:05 2009 -0500

    a line
    
    line 2
    
    line 3
    
    line 4

M	dir1/file3.txt

commit f36d6c45b93fdb03a3daec6bd90f79de47b304a0
Author: corey <ctrager@yahoo.com>
Date:   Sun Oct 4 19:44:42 2009 -0500

    the comment says
    line 2 that i have 3 files
    line 3
    line 4

A	Copy of file2.txt
M	dir1/file3.txt
M	file2.txt

commit 872dff5f4a49feb1079de32c572fd42f12def89d
Author: corey <ctrager@yahoo.com>
Date:   Sun Oct 4 15:43:11 2009 -0500

    this is insert jode
    and line two fo the comments

M	file2.txt

*/

</script>