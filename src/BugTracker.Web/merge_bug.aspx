<%@ Page language="C#"%>
<!--
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
-->
<!-- #include file = "inc.aspx" -->

<script language="C#" runat="server">

String sql;

Security security;
DataRow dr;

///////////////////////////////////////////////////////////////////////
void Page_Load(Object sender, EventArgs e)
{

	Util.do_not_cache(Response);
	

	security = new Security();

	security.check_security( HttpContext.Current, Security.ANY_USER_OK_EXCEPT_GUEST);

	if (security.user.is_admin || security.user.can_merge_bugs)
	{
		//
	}
	else
	{
		Response.Write ("You are not allowed to use this page.");
		Response.End();
	}

	titl.InnerText = Util.get_setting("AppTitle","BugTracker.NET") + " - "
		+ "merge " + Util.get_setting("SingularBugLabel","bug");


	if (!IsPostBack)
	{
		string orig_id_string = Util.sanitize_integer(Request["id"]);
		orig_id.Value = orig_id_string;
		back_href.HRef = "edit_bug.aspx?id=" + orig_id_string;
		from_bug.Value = orig_id_string;
	}
	else
	{
		from_err.InnerText = "";
		into_err.InnerText = "";
		on_update();
	}

}


///////////////////////////////////////////////////////////////////////
bool validate()
{

	bool good = true;

	// validate FROM

	if (from_bug.Value == "")
	{
		from_err.InnerText = "\"From\" bug is required.";
		good = false;
	}
	else {
		if (!Util.is_int(from_bug.Value))
		{
			from_err.InnerText = "\"From\" bug must be an integer.";
			good = false;

		}
	}

	// validate INTO

	if (into_bug.Value == "")
	{
		into_err.InnerText = "\"Into\" bug is required.";
		good = false;
	}
	else {
		if (!Util.is_int(into_bug.Value))
		{
			into_err.InnerText = "\"Into\" bug must be an integer.";
			good = false;

		}
	}


	if (!good)
	{
		return false;
	}


	if (from_bug.Value == into_bug.Value)
	{
		from_err.InnerText = "\"From\" bug cannot be the same as \"Into\" bug.";
		return false;
	}

	// Continue and see if from and to exist in db

	sql = @"
	declare @from_desc nvarchar(200)
	declare @into_desc nvarchar(200)
	declare @from_id int
	declare @into_id int
	set @from_id = -1
	set @into_id = -1
	select @from_desc = bg_short_desc, @from_id = bg_id from bugs where bg_id = $from
	select @into_desc = bg_short_desc, @into_id = bg_id from bugs where bg_id = $into
	select @from_desc, @into_desc, @from_id, @into_id	";

	sql = sql.Replace("$from", from_bug.Value);
	sql = sql.Replace("$into", into_bug.Value);

	dr = btnet.DbUtil.get_datarow(sql);

	if ((int) dr[2] == -1)
	{
		from_err.InnerText = "\"From\" bug not found.";
		good = false;
	}


	if ((int) dr[3] == -1)
	{
		into_err.InnerText = "\"Into\" bug not found.";
		good = false;
	}


	if (!good)
	{
		return false;
	}
	else
	{
		return true;
	}

}


///////////////////////////////////////////////////////////////////////
void on_update()
{

	// does it say "Merge" or "Confirm Merge"?

	if (submit.Value == "Merge")
	{
		if (!validate())
		{
			prev_from_bug.Value = "";
			prev_into_bug.Value = "";
			return;
		}
	}


	if (prev_from_bug.Value == from_bug.Value
	&& prev_into_bug.Value == into_bug.Value)
	{

        prev_from_bug.Value = btnet.Util.sanitize_integer(prev_from_bug.Value);
        prev_into_bug.Value = btnet.Util.sanitize_integer(prev_into_bug.Value);

		// rename the attachments

		string upload_folder = Util.get_upload_folder();
        if (upload_folder != null)
        {

		sql = @"select bp_id, bp_file from bug_posts
			where bp_type = 'file' and bp_bug = $from";

		sql = sql.Replace("$from", prev_from_bug.Value);
		DataSet ds = btnet.DbUtil.get_dataset(sql);

		foreach (DataRow dr in ds.Tables[0].Rows)
		{

			// create path
			StringBuilder path = new StringBuilder(upload_folder);
			path.Append("\\");
			path.Append(prev_from_bug.Value);
			path.Append("_");
			path.Append(Convert.ToString(dr["bp_id"]));
			path.Append("_");
			path.Append(Convert.ToString(dr["bp_file"]));
			if (System.IO.File.Exists(path.ToString()))
			{

				StringBuilder path2 = new StringBuilder(upload_folder);
				path2.Append("\\");
				path2.Append(prev_into_bug.Value);
				path2.Append("_");
				path2.Append(Convert.ToString(dr["bp_id"]));
				path2.Append("_");
				path2.Append(Convert.ToString(dr["bp_file"]));

				System.IO.File.Move(path.ToString(), path2.ToString());
			}

		}
        }


		// copy the from db entries to the to
		sql = @"
insert into bug_subscriptions
(bs_bug, bs_user)
select $into, bs_user
from bug_subscriptions
where bs_bug = $from
and bs_user not in (select bs_user from bug_subscriptions where bs_bug = $into)

insert into bug_user
(bu_bug, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime)
select $into, bu_user, bu_flag, bu_flag_datetime, bu_seen, bu_seen_datetime, bu_vote, bu_vote_datetime
from bug_user
where bu_bug = $from
and bu_user not in (select bu_user from bug_user where bu_bug = $into)

update bug_posts     set bp_bug     = $into	where bp_bug = $from
update bug_tasks     set tsk_bug    = $into where tsk_bug = $from
update svn_revisions set svnrev_bug = $into where svnrev_bug = $from
update hg_revisions  set hgrev_bug  = $into where hgrev_bug = $from
update git_commits   set gitcom_bug = $into where gitcom_bug = $from
";
			
		sql = sql.Replace("$from",prev_from_bug.Value);
		sql = sql.Replace("$into",prev_into_bug.Value);

		btnet.DbUtil.execute_nonquery(sql);

		// record the merge itself

		sql = @"insert into bug_posts
			(bp_bug, bp_user, bp_date, bp_type, bp_comment, bp_comment_search)
			values($into,$us,getdate(), 'comment', 'merged bug $from into this bug:', 'merged bug $from into this bug:')
			select scope_identity()";

		sql = sql.Replace("$from",prev_from_bug.Value);
		sql = sql.Replace("$into",prev_into_bug.Value);
		sql = sql.Replace("$us",Convert.ToString(security.user.usid));

		int comment_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

		// update bug comments with info from old bug
		sql = @"update bug_posts
			set bp_comment = convert(nvarchar,bp_comment) + char(10) + bg_short_desc
			from bugs where bg_id = $from
			and bp_id = $bc";

		sql = sql.Replace("$from",prev_from_bug.Value);
		sql = sql.Replace("$bc",Convert.ToString(comment_id));
		btnet.DbUtil.execute_nonquery(sql);


		// delete the from bug
		int from_bugid = Convert.ToInt32(prev_from_bug.Value);
		Bug.delete_bug(from_bugid);

		// delete the from bug from the list, if there is a list
		DataView dv_bugs = (DataView) Session["bugs"];
		if (dv_bugs != null)
		{
			// read through the list of bugs looking for the one that matches the from
			int index = 0;
			foreach (DataRowView drv in dv_bugs)
			{
				if (from_bugid == (int) drv[1])
				{
					dv_bugs.Delete(index);
					break;
				}
				index++;
			}
		}

		btnet.Bug.send_notifications(btnet.Bug.UPDATE, Convert.ToInt32(prev_into_bug.Value), security);

		Response.Redirect ("edit_bug.aspx?id=" + prev_into_bug.Value);

	}
	else
	{
		prev_from_bug.Value = from_bug.Value;
		prev_into_bug.Value = into_bug.Value;
		static_from_bug.InnerText = from_bug.Value;
		static_into_bug.InnerText = into_bug.Value;
        static_from_desc.InnerText = (string)dr[0];
        static_into_desc.InnerText = (string)dr[1];
		from_bug.Style["display"] = "none";
		into_bug.Style["display"] = "none";
		static_from_bug.Style["display"] = "";
		static_into_bug.Style["display"] = "";
		static_from_desc.Style["display"] = "";
		static_into_desc.Style["display"] = "";
		submit.Value = "Confirm Merge";
	}

}

</script>

<html>
<head>
<title id="titl" runat="server">btnet merge bug</title>
<link rel="StyleSheet" href="btnet.css" type="text/css">
</head>
<body>
<% security.write_menu(Response, "admin"); %>
<p>
<div class=align><table border=0><tr><td>

<a id="back_href" runat="server" href="">back to <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %></a>
<!--<a id="confirm_href" runat="server" href="">confirm delete</a>
</a>-->
<p>
Merge all comments, attachments, and subscriptions
from "FROM" <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %>
into "INTO" <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %>.
<br>
<span class=err>Note:&nbsp;&nbsp;"FROM" <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %> will be deleted!</err>
<p>

<form runat="server" class=frm>
<table border=0>

<tr>
<td class=lbl align=right>
FROM <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %>:
<td align=left valign=bottom>
<input type=text class=txt id="from_bug" runat="server" size=8>
<span class="stat" id="static_from_bug" runat="server" style='display: none;'></span>
<br>
<span class="stat" id="static_from_desc" runat="server" style='display: none;'></span>

<tr><td colspan=2><span class=err id="from_err" runat="server">&nbsp;</span>


<tr>
<td class=lbl align=right>
INTO <% Response.Write(Util.get_setting("SingularBugLabel","bug")); %>:
<td align=left valign=bottom>
<input type=text class=txt id="into_bug" runat="server" size=8>
<span class="stat" id="static_into_bug" runat="server" style='display: none;'></span>
<br>
<span class="stat" id="static_into_desc" runat="server" style='display: none;'></span>

<tr><td colspan=2><span class=err id="into_err" runat="server">&nbsp;</span>


</tr>


<tr><td colspan=2 align=center><br>
<input class=btn type=submit runat="server" id="submit" value="Merge">



</table>

<input type=hidden id="confirm" runat="server">
<input type=hidden id="prev_from_bug" runat="server">
<input type=hidden id="prev_into_bug" runat="server">
<input type=hidden id="orig_id" runat="server">

</form>

<p>


</td></tr></table></div>
<% Response.Write(Application["custom_footer"]); %></body>
</html>


