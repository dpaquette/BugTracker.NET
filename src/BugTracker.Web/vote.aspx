<%@ Page language="C#" CodeBehind="vote.aspx.cs" Inherits="btnet.vote" AutoEventWireup="True" %>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{
	Util.do_not_cache(Response);
	
	if (!User.IsInRole(BtnetRoles.Guest))
	{
		if (Request.QueryString["ses"] != (string) Session["session_cookie"])
		{
			Response.Write ("session in URL doesn't match session cookie");
			Response.End();
		}
	}

	DataView dv = (DataView) Session["bugs"];
	if (dv == null)
	{
		Response.End();
	}

	int bugid = Convert.ToInt32(Util.sanitize_integer(Request["bugid"]));

    int permission_level = Bug.get_bug_permission_level(bugid, User.Identity);
	if (permission_level ==PermissionLevel.None)
	{
		Response.End();
	}

	for (int i = 0; i < dv.Count; i++)
	{
		if ((int)dv[i][1] == bugid)
		{
			// treat it like a delta and update the cached vote count.
			int vote = Convert.ToInt32(Util.sanitize_integer(Request["vote"]));
			object obj_vote_count = Application[Convert.ToString(bugid)];
			int vote_count = 0;
			
			if (obj_vote_count != null)
				vote_count = (int) obj_vote_count;
				
			vote_count += vote;
			
			Application[Convert.ToString(bugid)] = vote_count;

			// now treat it more like a boolean
			if (vote == -1)
				vote = 0;

			dv[i]["$VOTE"] = vote;
			var sql = new SQLString(@"
if not exists (select bu_bug from bug_user where bu_bug = @bg and bu_user = @us)
	insert into bug_user (bu_bug, bu_user, bu_flag, bu_seen, bu_vote) values(@bg, @us, 0, 0, 1) 
update bug_user set bu_vote = @vote, bu_vote_datetime = getdate() where bu_bug = @bg and bu_user = @us and bu_vote <> @vote");
				
			sql = sql.AddParameterWithValue("vote", Convert.ToString(vote));
			sql = sql.AddParameterWithValue("bg", Convert.ToString(bugid));
			sql = sql.AddParameterWithValue("us", Convert.ToString(User.Identity.GetUserId()));

			btnet.DbUtil.execute_nonquery(sql);

			break;
		}
	}

}



</script>
