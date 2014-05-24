/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace btnet
{

	public class Bug
	{

		public const int INSERT = 1;
		public const int UPDATE = 2;
		private static object dummy = new object(); // for a lock

		///////////////////////////////////////////////////////////////////////
		public static void auto_subscribe(int bugid)
		{

			// clean up bug subscriptions that no longer fit security rules
			// subscribe per auto_subscribe
			// subscribe project's default user
			// subscribe per-project auto_subscribers
			// subscribe per auto_subscribe_own_bugs
			string sql = @"
declare @pj int
select @pj = bg_project from bugs where bg_id = $id

delete from bug_subscriptions
where bs_bug = $id
and bs_user in
(select x.pu_user
from projects
left outer join project_user_xref x on pu_project = pj_id
where pu_project = @pj
and isnull(pu_permission_level,$dpl) = 0)

delete from bug_subscriptions
where bs_bug = $id
and bs_user in
(select us_id from users
 inner join orgs on us_org = og_id
 inner join bugs on bg_id = $id
 where og_other_orgs_permission_level = 0
 and bg_org <> og_id)

insert into bug_subscriptions (bs_bug, bs_user)
select $id, us_id
from users
inner join orgs on us_org = og_id
inner join bugs on bg_id = $id
left outer join project_user_xref on pu_project = @pj and pu_user = us_id
where us_auto_subscribe = 1
and
case
	when
		us_org <> bg_org
		and og_other_orgs_permission_level < 2
		and og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
			then og_other_orgs_permission_level
	else
		isnull(pu_permission_level,$dpl)
end <> 0
and us_active = 1
and us_id not in
(select bs_user from bug_subscriptions
where bs_bug = $id)

insert into bug_subscriptions (bs_bug, bs_user)
select $id, pj_default_user
from projects
inner join users on pj_default_user = us_id
where pj_id = @pj
and pj_default_user <> 0
and pj_auto_subscribe_default_user = 1
and us_active = 1
and pj_default_user not in
(select bs_user from bug_subscriptions
where bs_bug = $id)

insert into bug_subscriptions (bs_bug, bs_user)
select $id, pu_user from project_user_xref
inner join users on pu_user = us_id
inner join orgs on us_org = og_id
inner join bugs on bg_id = $id
where pu_auto_subscribe = 1
and
case
	when
		us_org <> bg_org
		and og_other_orgs_permission_level < 2
		and og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
			then og_other_orgs_permission_level
	else
		isnull(pu_permission_level,$dpl)
end <> 0
and us_active = 1
and pu_project = @pj
and pu_user not in
(select bs_user from bug_subscriptions
where bs_bug = $id)

insert into bug_subscriptions (bs_bug, bs_user)
select $id, us_id
from users
inner join bugs on bg_id = $id
inner join orgs on us_org = og_id
left outer join project_user_xref on pu_project = @pj and pu_user = us_id
where ((us_auto_subscribe_own_bugs = 1 and bg_assigned_to_user = us_id)
or
(us_auto_subscribe_reported_bugs = 1 and bg_reported_user = us_id))
and
case
	when
		us_org <> bg_org
		and og_other_orgs_permission_level < 2
		and og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
			then og_other_orgs_permission_level
	else
		isnull(pu_permission_level,$dpl)
end <> 0
and us_active = 1
and us_id not in
(select bs_user from bug_subscriptions
where bs_bug = $id)";

			sql = sql.Replace("$id", Convert.ToString(bugid));
			sql = sql.Replace("$dpl", btnet.Util.get_setting("DefaultPermissionLevel", "2"));

			
			btnet.DbUtil.execute_nonquery(sql);


		}

		///////////////////////////////////////////////////////////////////////
		public static void delete_bug(int bugid)
		{

			// delete attachements

			string id = Convert.ToString(bugid);

			string upload_folder = Util.get_upload_folder();
			string sql = @"select bp_id, bp_file from bug_posts where bp_type = 'file' and bp_bug = $bg";
			sql = sql.Replace("$bg", id);

			
			DataSet ds = btnet.DbUtil.get_dataset(sql);
			if (upload_folder != null && upload_folder != "")
			{
				foreach (DataRow dr in ds.Tables[0].Rows)
				{

					// create path
					StringBuilder path = new StringBuilder(upload_folder);
					path.Append("\\");
					path.Append(id);
					path.Append("_");
					path.Append(Convert.ToString(dr["bp_id"]));
					path.Append("_");
					path.Append(Convert.ToString(dr["bp_file"]));
					if (System.IO.File.Exists(path.ToString()))
					{
						System.IO.File.Delete(path.ToString());
					}

				}
			}

			// delete the database entries

			sql = @"
delete bug_post_attachments from bug_post_attachments inner join bug_posts on bug_post_attachments.bpa_post = bug_posts.bp_id where bug_posts.bp_bug = $bg
delete from bug_posts where bp_bug = $bg
delete from bug_subscriptions where bs_bug = $bg
delete from bug_relationships where re_bug1 = $bg
delete from bug_relationships where re_bug2 = $bg
delete from bug_user where bu_bug = $bg
delete from bug_tasks where tsk_bug = $bg
delete from bugs where bg_id = $bg";

			sql = sql.Replace("$bg", id);
			btnet.DbUtil.execute_nonquery(sql);


		}

		///////////////////////////////////////////////////////////////////////
		public static int insert_post_attachment_copy(
			btnet.Security security,
			int bugid,
			int copy_bpid,
			string comment,
			int parent,
			bool hidden_from_external_users,
			bool send_notifications)
		{
			return insert_post_attachment_impl(
				security,
				bugid,
				null,
				-1,
				copy_bpid,
				null,
				comment,
				null,
				parent,
				hidden_from_external_users,
				send_notifications);
		}

		///////////////////////////////////////////////////////////////////////
		public static int insert_post_attachment(
			btnet.Security security,
			int bugid,
			Stream content,
			int content_length,
			string file,
			string comment,
			string content_type,
			int parent,
			bool hidden_from_external_users,
			bool send_notifications)
		{
			return insert_post_attachment_impl(
				security,
				bugid,
				content,
				content_length,
				-1, // copy_bpid
				file,
				comment,
				content_type,
				parent,
				hidden_from_external_users,
				send_notifications);
		}

		///////////////////////////////////////////////////////////////////////
		private static int insert_post_attachment_impl(
			btnet.Security security,
			int bugid,
			Stream content,
			int content_length,
			int copy_bpid,
			string file,
			string comment,
			string content_type,
			int parent,
			bool hidden_from_external_users,
			bool send_notifications)
		{
			// Note that this method does not perform any security check nor does
			// it check that content_length is less than MaxUploadSize.
			// These are left up to the caller.

			
			string upload_folder = Util.get_upload_folder();
			string sql;
			bool store_attachments_in_database = (Util.get_setting("StoreAttachmentsInDatabase", "0") == "1");
			string effective_file = file;
			int effective_content_length = content_length;
			string effective_content_type = content_type;
			Stream effective_content = null;

			try
			{
				// Determine the content. We may be instructed to copy an existing
				// attachment via copy_bpid, or a Stream may be provided as the content parameter.

				if (copy_bpid != -1)
				{
					BugPostAttachment bpa = get_bug_post_attachment(copy_bpid);

					effective_content = bpa.content;
					effective_file = bpa.file;
					effective_content_length = bpa.content_length;
					effective_content_type = bpa.content_type;
				}
				else
				{
					effective_content = content;
					effective_file = file;
					effective_content_length = content_length;
					effective_content_type = content_type;
				}

				// Insert a new post into bug_posts.

				sql = @"
declare @now datetime

set @now = getdate()

update bugs
	set bg_last_updated_date = @now,
	bg_last_updated_user = $us
	where bg_id = $bg

insert into bug_posts
	(bp_type, bp_bug, bp_file, bp_comment, bp_size, bp_date, bp_user, bp_content_type, bp_parent, bp_hidden_from_external_users)
	values ('file', $bg, N'$fi', N'$de', $si, @now, $us, N'$ct', $pa, $internal)
	select scope_identity()";

				sql = sql.Replace("$bg", Convert.ToString(bugid));
				sql = sql.Replace("$fi", effective_file.Replace("'", "''"));
				sql = sql.Replace("$de", comment.Replace("'", "''"));
				sql = sql.Replace("$si", Convert.ToString(effective_content_length));
				sql = sql.Replace("$us", Convert.ToString(security.user.usid));

				// Sometimes, somehow, content type is null.  Not sure how.
				sql = sql.Replace("$ct",
					effective_content_type != null
						? effective_content_type.Replace("'", "''")
						: string.Empty);

				if (parent == -1)
				{
					sql = sql.Replace("$pa", "null");
				}
				else
				{
					sql = sql.Replace("$pa", Convert.ToString(parent));
				}
				sql = sql.Replace("$internal", btnet.Util.bool_to_string(hidden_from_external_users));

				int bp_id = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

				try
				{
					// Store attachment in bug_post_attachments table.

					if (store_attachments_in_database)
					{
						byte[] data = new byte[effective_content_length];
						int bytes_read = 0;

						while (bytes_read < effective_content_length)
						{
							int bytes_read_this_iteration = effective_content.Read(data, bytes_read, effective_content_length - bytes_read);
							if (bytes_read_this_iteration == 0)
							{
								throw new Exception("Unexpectedly reached the end of the stream before all data was read.");
							}
							bytes_read += bytes_read_this_iteration;
						}

						sql = @"insert into bug_post_attachments
								(bpa_post, bpa_content)
								values (@bp, @bc)";
						using (SqlCommand cmd = new SqlCommand(sql))
						{
							cmd.Parameters.AddWithValue("@bp", bp_id);
							cmd.Parameters.Add("@bc", SqlDbType.Image).Value = data;
                            cmd.CommandTimeout = Convert.ToInt32(Util.get_setting("SqlCommand.CommandTimeout", "30"));
							btnet.DbUtil.execute_nonquery(cmd);
						}
					}
					else
					{
						// Store attachment in UploadFolder.

						if (upload_folder == null)
						{
							throw new Exception("StoreAttachmentsInDatabase is false and UploadFolder is not set in web.config.");
						}

						// Copy the content Stream to a file in the upload_folder.
						byte[] buffer = new byte[16384];
						int bytes_read = 0;
						using (FileStream fs = new FileStream(upload_folder + "\\" + bugid + "_" + bp_id + "_" + effective_file, FileMode.CreateNew, FileAccess.Write))
						{
							while (bytes_read < effective_content_length)
							{
								int bytes_read_this_iteration = effective_content.Read(buffer, 0, buffer.Length);
								if (bytes_read_this_iteration == 0)
								{
									throw new Exception("Unexpectedly reached the end of the stream before all data was read.");
								}
								fs.Write(buffer, 0, bytes_read_this_iteration);
								bytes_read += bytes_read_this_iteration;
							}
						}
					}
				}
				catch
				{
					// clean up
					sql = @"delete from bug_posts where bp_id = $bp";

					sql = sql.Replace("$bp", Convert.ToString(bp_id));

					btnet.DbUtil.execute_nonquery(sql);

					throw;
				}

				if (send_notifications)
				{
					btnet.Bug.send_notifications(btnet.Bug.UPDATE, bugid, security);
				}
				return bp_id;
			}
			finally
			{
				// If this procedure "owns" the content (instead of our caller owning it), dispose it.
				if (effective_content != null && effective_content != content)
				{
					effective_content.Dispose();
				}
			}
		}

		///////////////////////////////////////////////////////////////////////
		public class BugPostAttachment
		{
			public BugPostAttachment(string file, Stream content, int content_length, string content_type)
			{
				this.file = file;
				this.content = content;
				this.content_length = content_length;
				this.content_type = content_type;
			}

			public string file;
			public Stream content;
			public int content_length;
			public string content_type;
		}

		///////////////////////////////////////////////////////////////////////
		public static BugPostAttachment get_bug_post_attachment(int bp_id)
		{
			// Note that this method does not perform any security check.
			// This is left up to the caller.

			
			string upload_folder = Util.get_upload_folder();
			string sql;
			bool store_attachments_in_database = (Util.get_setting("StoreAttachmentsInDatabase", "0") == "1");
			int bugid;
			string file;
			int content_length;
			string content_type;
			Stream content = null;

			try
			{
				sql = @"select bp_bug, bp_file, bp_size, bp_content_type
						from bug_posts
						where bp_id = $bp";
						
				sql = sql.Replace("$bp", Convert.ToString(bp_id));
				using (SqlDataReader reader = btnet.DbUtil.execute_reader(sql, CommandBehavior.CloseConnection))
				{
					if (reader.Read())
					{
						bugid = reader.GetInt32(reader.GetOrdinal("bp_bug"));
						file = reader.GetString(reader.GetOrdinal("bp_file"));
						content_length = reader.GetInt32(reader.GetOrdinal("bp_size"));
						content_type = reader.GetString(reader.GetOrdinal("bp_content_type"));
					}
					else
					{
						throw new Exception("Existing bug post not found.");
					}
				}

				sql = @"select bpa_content
							from bug_post_attachments
							where bpa_post = $bp";
							
				sql = sql.Replace("$bp", Convert.ToString(bp_id));

				object content_object;
				content_object = btnet.DbUtil.execute_scalar(sql);

				if (content_object != null && !Convert.IsDBNull(content_object))
				{
					content = new MemoryStream((byte[])content_object);
				}
				else
				{
					// Could not find in bug_post_attachments. Try the upload_folder.
					if (upload_folder == null)
					{
						throw new Exception("The attachment could not be found in the database and UploadFolder is not set in web.config.");
					}

					string upload_folder_filename = upload_folder + "\\" + bugid + "_" + bp_id + "_" + file;
					if (File.Exists(upload_folder_filename))
					{
						content = new FileStream(upload_folder_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
					}
					else
					{
						throw new Exception("Attachment not found in database or UploadFolder.");
					}
				}

				return new BugPostAttachment(file, content, content_length, content_type);
			}
			catch
			{
				if (content != null)
					content.Dispose();

				throw;
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static DataRow get_bug_datarow(
			int bugid,
			Security security)
		{

			
			DataSet ds_custom_cols = btnet.Util.get_custom_columns();
			return get_bug_datarow(bugid, security, ds_custom_cols);
		}


		///////////////////////////////////////////////////////////////////////
		public static DataRow get_bug_datarow(
			int bugid,
			Security security,
			DataSet ds_custom_cols)
		{
			string sql = @" /* get_bug_datarow */";

			if (btnet.Util.get_setting("EnableSeen", "0") == "1")
			{
				sql += @"
if not exists (select bu_bug from bug_user where bu_bug = $id and bu_user = $this_usid)
	insert into bug_user (bu_bug, bu_user, bu_flag, bu_seen, bu_vote) values($id, $this_usid, 0, 1, 0) 
update bug_user set bu_seen = 1, bu_seen_datetime = getdate() where bu_bug = $id and bu_user = $this_usid and bu_seen <> 1";

			}

			sql += @"
declare @svn_revisions int
declare @git_commits int
declare @hg_revisions int
declare @tasks int
declare @related int;
set @svn_revisions = 0
set @git_commits = 0
set @hg_revisions = 0
set @tasks = 0
set @related = 0";

			if (btnet.Util.get_setting("EnableSubversionIntegration", "0") == "1")
			{
				sql += @"
select @svn_revisions = count(1)
from svn_affected_paths
inner join svn_revisions on svnap_svnrev_id = svnrev_id
where svnrev_bug = $id;";
			}

			if (btnet.Util.get_setting("EnableGitIntegration", "0") == "1")
			{
				sql += @"
select @git_commits = count(1)
from git_affected_paths
inner join git_commits on gitap_gitcom_id = gitcom_id
where gitcom_bug = $id;";
			}

			if (btnet.Util.get_setting("EnableMercurialIntegration", "0") == "1")
			{
				sql += @"
select @hg_revisions = count(1)
from hg_affected_paths
inner join hg_revisions on hgap_hgrev_id = hgrev_id
where hgrev_bug = $id;";
			}

			if (btnet.Util.get_setting("EnableTasks", "0") == "1")
			{
				sql += @"
select @tasks = count(1)
from bug_tasks
where tsk_bug = $id;";
			}

			if (btnet.Util.get_setting("EnableRelationships", "0") == "1")
			{
                sql += @"
select @related = count(1)
from bug_relationships
where re_bug1 = $id;";
            }
    
            sql += @"

select bg_id [id],
bg_short_desc [short_desc],
isnull(bg_tags,'') [bg_tags],
isnull(ru.us_username,'[deleted user]') [reporter],
isnull(ru.us_email,'') [reporter_email],
case rtrim(ru.us_firstname)
	when null then isnull(ru.us_lastname, '')
	when '' then isnull(ru.us_lastname, '')
	else isnull(ru.us_lastname + ', ' + ru.us_firstname,'')
	end [reporter_fullname],
bg_reported_date [reported_date],
datediff(s,bg_reported_date,getdate()) [seconds_ago],
isnull(lu.us_username,'') [last_updated_user],
case rtrim(lu.us_firstname)
	when null then isnull(lu.us_lastname, '')
	when '' then isnull(lu.us_lastname, '')
	else isnull(lu.us_lastname + ', ' + lu.us_firstname,'')
	end [last_updated_fullname],


bg_last_updated_date [last_updated_date],
isnull(bg_project,0) [project],
isnull(pj_name,'[no project]') [current_project],

isnull(bg_org,0) [organization],
isnull(bugorg.og_name,'') [og_name],

isnull(bg_category,0) [category],
isnull(ct_name,'') [category_name],

isnull(bg_priority,0) [priority],
isnull(pr_name,'') [priority_name],

isnull(bg_status,0) [status],
isnull(st_name,'') [status_name],

isnull(bg_user_defined_attribute,0) [udf],
isnull(udf_name,'') [udf_name],

isnull(bg_assigned_to_user,0) [assigned_to_user],
isnull(asg.us_username,'[not assigned]') [assigned_to_username],
case rtrim(asg.us_firstname)
when null then isnull(asg.us_lastname, '[not assigned]')
when '' then isnull(asg.us_lastname, '[not assigned]')
else isnull(asg.us_lastname + ', ' + asg.us_firstname,'[not assigned]')
end [assigned_to_fullname],

isnull(bs_user,0) [subscribed],

case
when
	$this_org <> bg_org
	and userorg.og_other_orgs_permission_level < 2
	and userorg.og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
		then userorg.og_other_orgs_permission_level
else
	isnull(pu_permission_level,$dpl)
end [pu_permission_level],

isnull(bg_project_custom_dropdown_value1,'') [bg_project_custom_dropdown_value1],
isnull(bg_project_custom_dropdown_value2,'') [bg_project_custom_dropdown_value2],
isnull(bg_project_custom_dropdown_value3,'') [bg_project_custom_dropdown_value3],
@related [relationship_cnt],
@svn_revisions [svn_revision_cnt],
@git_commits [git_commit_cnt],
@hg_revisions [hg_commit_cnt],
@tasks [task_cnt],
getdate() [snapshot_timestamp]
$custom_cols_placeholder
from bugs
inner join users this_user on us_id = $this_usid
inner join orgs userorg on this_user.us_org = userorg.og_id
left outer join user_defined_attribute on bg_user_defined_attribute = udf_id
left outer join projects on bg_project = pj_id
left outer join orgs bugorg on bg_org = bugorg.og_id
left outer join categories on bg_category = ct_id
left outer join priorities on bg_priority = pr_id
left outer join statuses on bg_status = st_id
left outer join users asg on bg_assigned_to_user = asg.us_id
left outer join users ru on bg_reported_user = ru.us_id
left outer join users lu on bg_last_updated_user = lu.us_id
left outer join bug_subscriptions on bs_bug = bg_id and bs_user = $this_usid
left outer join project_user_xref on pj_id = pu_project
and pu_user = $this_usid
where bg_id = $id";

			if (ds_custom_cols.Tables[0].Rows.Count == 0)
			{
				sql = sql.Replace("$custom_cols_placeholder", "");
			}
			else
			{
				string custom_cols_sql = "";

				foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
				{
					custom_cols_sql += ",[" + drcc["name"].ToString() + "]";

				}
				sql = sql.Replace("$custom_cols_placeholder", custom_cols_sql);
			}

			sql = sql.Replace("$id", Convert.ToString(bugid));
			sql = sql.Replace("$this_usid", Convert.ToString(security.user.usid));
			sql = sql.Replace("$this_org", Convert.ToString(security.user.org));
			sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel", "2"));

			
			return btnet.DbUtil.get_datarow(sql);


		}

        ///////////////////////////////////////////////////////////////////////
        public static void apply_post_insert_rules(int bugid)
        {
            string sql = Util.get_setting("UpdateBugAfterInsertBugAspxSql", "");

            if (sql != "")
            {
                sql = sql.Replace("$BUGID$", Convert.ToString(bugid));
                btnet.DbUtil.execute_nonquery(sql);
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public static System.Data.DataRow get_bug_defaults()
        {

            string sql = @"/*fetch defaults*/
declare @pj int
declare @ct int
declare @pr int
declare @st int
declare @udf int
set @pj = 0
set @ct = 0
set @pr = 0
set @st = 0
set @udf = 0
select @pj = pj_id from projects where pj_default = 1 order by pj_name
select @ct = ct_id from categories where ct_default = 1 order by ct_name
select @pr = pr_id from priorities where pr_default = 1 order by pr_name
select @st = st_id from statuses where st_default = 1 order by st_name
select @udf = udf_id from user_defined_attribute where udf_default = 1 order by udf_name
select @pj pj, @ct ct, @pr pr, @st st, @udf udf";

            return btnet.DbUtil.get_datarow(sql);
        }

		///////////////////////////////////////////////////////////////////////
		public static int get_bug_permission_level(int bugid, Security security)
		{
			/*
					public const int PERMISSION_NONE = 0;
					public const int PERMISSION_READONLY = 1;
					public const int PERMISSION_REPORTER = 3;
					public const int PERMISSION_ALL = 2;
			*/

			// fetch the revised permission level
			string sql = @"
declare @bg_org int

select isnull(pu_permission_level,$dpl),
bg_org
from bugs
left outer join project_user_xref
on pu_project = bg_project
and pu_user = $us
where bg_id = $bg";
			;

			sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel", "2"));
			sql = sql.Replace("$bg", Convert.ToString(bugid));
			sql = sql.Replace("$us", Convert.ToString(security.user.usid));
			
			DataRow dr = btnet.DbUtil.get_datarow(sql);
			
			if (dr == null)
			{
				return Security.PERMISSION_NONE;
				
			}
			
			int pl = (int)dr[0];
			int bg_org = (int)dr[1];


			// maybe reduce permissions
			if (bg_org != security.user.org)
			{
				if (security.user.other_orgs_permission_level == Security.PERMISSION_NONE
				|| security.user.other_orgs_permission_level == Security.PERMISSION_READONLY)
				{
					if (security.user.other_orgs_permission_level < pl)
					{
						pl = security.user.other_orgs_permission_level;
					}
				}
			}

			return pl;
		}


		///////////////////////////////////////////////////////////////////////
		public class NewIds
		{
			public NewIds(int b, int p)
			{
				bugid = b;
				postid = p;
			}
			public int bugid;
			public int postid;
		};

		///////////////////////////////////////////////////////////////////////
		public static NewIds insert_bug(
			string short_desc,
			Security security,
			string tags,
			int projectid,
			int orgid,
			int categoryid,
			int priorityid,
			int statusid,
			int assigned_to_userid,
			int udfid,
			string project_custom_dropdown_value1,
			string project_custom_dropdown_value2,
			string project_custom_dropdown_value3,
			string comment_formated,
			string comment_search,
			string from,
			string cc,
			string content_type,
			bool internal_only,
			SortedDictionary<string,string> hash_custom_cols,
			bool send_notifications)
		{

			if (short_desc.Trim() == "")
			{
				short_desc = "[No Description]";
			}

			if (assigned_to_userid == 0)
			{
				assigned_to_userid = btnet.Util.get_default_user(projectid);
			}

			string sql = @"insert into bugs
					(bg_short_desc,
					bg_tags,
					bg_reported_user,
					bg_last_updated_user,
					bg_reported_date,
					bg_last_updated_date,
					bg_project,
					bg_org,
					bg_category,
					bg_priority,
					bg_status,
					bg_assigned_to_user,
					bg_user_defined_attribute,
					bg_project_custom_dropdown_value1,
					bg_project_custom_dropdown_value2,
					bg_project_custom_dropdown_value3
					$custom_cols_placeholder1)
					values (N'$short_desc', N'$tags', $reported_user,  $reported_user, getdate(), getdate(),
					$project, $org,
					$category, $priority, $status, $assigned_user, $udf,
					N'$pcd1',N'$pcd2',N'$pcd3' $custom_cols_placeholder2)";

			sql = sql.Replace("$short_desc", short_desc.Replace("'", "''"));
			sql = sql.Replace("$tags", tags.Replace("'", "''"));
			sql = sql.Replace("$reported_user", Convert.ToString(security.user.usid));
			sql = sql.Replace("$project", Convert.ToString(projectid));
			sql = sql.Replace("$org", Convert.ToString(orgid));
			sql = sql.Replace("$category", Convert.ToString(categoryid));
			sql = sql.Replace("$priority", Convert.ToString(priorityid));
			sql = sql.Replace("$status", Convert.ToString(statusid));
			sql = sql.Replace("$assigned_user", Convert.ToString(assigned_to_userid));
			sql = sql.Replace("$udf", Convert.ToString(udfid));
			sql = sql.Replace("$pcd1", project_custom_dropdown_value1);
			sql = sql.Replace("$pcd2", project_custom_dropdown_value2);
			sql = sql.Replace("$pcd3", project_custom_dropdown_value3);

			if (hash_custom_cols == null)
			{
				sql = sql.Replace("$custom_cols_placeholder1", "");
				sql = sql.Replace("$custom_cols_placeholder2", "");
			}
			else
			{

				string custom_cols_sql1 = "";
				string custom_cols_sql2 = "";

				DataSet ds_custom_cols = btnet.Util.get_custom_columns();

				foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
				{

					string column_name = (string) drcc["name"];

					// skip if no permission to update
					if (security.user.dict_custom_field_permission_level[column_name] != Security.PERMISSION_ALL)
					{
						continue;
					}

					custom_cols_sql1 += ",[" + column_name + "]";
					
					string datatype = (string) drcc["datatype"];
					
					string custom_col_val = btnet.Util.request_to_string_for_sql(
						hash_custom_cols[column_name],
						datatype);
					
					custom_cols_sql2 += "," + custom_col_val;
					
				}
				sql = sql.Replace("$custom_cols_placeholder1", custom_cols_sql1);
				sql = sql.Replace("$custom_cols_placeholder2", custom_cols_sql2);
			}



			sql += "\nselect scope_identity()";


			int bugid = Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));
			int postid = btnet.Bug.insert_comment(
				bugid,
				security.user.usid,
				comment_formated,
				comment_search,
				from,
				cc,
				content_type,
				internal_only);

			btnet.Bug.auto_subscribe(bugid);

			if (send_notifications)
			{
				btnet.Bug.send_notifications(btnet.Bug.INSERT, bugid, security);
			}

			return new NewIds(bugid, postid);

		}


		///////////////////////////////////////////////////////////////////////
		public static int insert_comment(
			int bugid,
			int this_usid,
			string comment_formated,
			string comment_search,
			string from,
			string cc,
			string content_type,
			bool internal_only)
		{

			if (comment_formated != "")
			{
				string sql = @"
declare @now datetime
set @now = getdate()

insert into bug_posts
(bp_bug, bp_user, bp_date, bp_comment, bp_comment_search, bp_email_from, bp_email_cc, bp_type, bp_content_type,
bp_hidden_from_external_users)
values(
$id,
$us,
@now,
N'$comment_formatted',
N'$comment_search',
N'$from',
N'$cc',
N'$type',
N'$content_type',
$internal)
select scope_identity();";

				if (from != null)
				{
					// Update the bugs timestamp here.
					// We don't do it unconditionally because it would mess up the locking.
					// The edit_bug.aspx page gets its snapshot timestamp from the update of the bug
					// row, not the comment row, so updating the bug again would confuse it.
					sql += @"update bugs
						set bg_last_updated_date = @now,
						bg_last_updated_user = $us
						where bg_id = $id";

					sql = sql.Replace("$from", from.Replace("'", "''"));
					sql = sql.Replace("$type", "received"); // received email
				}
				else
				{
					sql = sql.Replace("N'$from'", "null");
					sql = sql.Replace("$type", "comment"); // bug comment
				}

				sql = sql.Replace("$id", Convert.ToString(bugid));
				sql = sql.Replace("$us", Convert.ToString(this_usid));
				sql = sql.Replace("$comment_formatted", comment_formated.Replace("'", "''"));
				sql = sql.Replace("$comment_search", comment_search.Replace("'", "''"));
				sql = sql.Replace("$content_type", content_type);
				if (cc == null)
				{
					cc = "";
				}
				sql = sql.Replace("$cc", cc.Replace("'", "''"));
				sql = sql.Replace("$internal", btnet.Util.bool_to_string(internal_only));


				
				return Convert.ToInt32(btnet.DbUtil.execute_scalar(sql));

			}
			else
			{
				return 0;
			}

		}

		///////////////////////////////////////////////////////////////////////
		public static void send_notifications(int insert_or_update, int bugid, Security security, int just_to_this_user_id)
		{
			send_notifications(insert_or_update,
				bugid,
				security,
				just_to_this_user_id,
				false,  // status changed
				false,  // assigend to changed
				0);  // prev assigned to
		}

		///////////////////////////////////////////////////////////////////////
		public static void send_notifications(int insert_or_update, int bugid, Security security)
		{
			send_notifications(insert_or_update,
				bugid,
				security,
				0,  // just to this
				false,  // status changed
				false,  // assigend to changed
				0);  // prev assigned to
		}


		///////////////////////////////////////////////////////////////////////
		// This used to send the emails, but not now.  Now it just queues
		// the emails to be sent, then spawns a thread to send them.
		public static void send_notifications(int insert_or_update,  // The implementation
			int bugid,
			Security security,
			int just_to_this_userid,
			bool status_changed,
			bool assigned_to_changed,
			int prev_assigned_to_user)
		{

			// If there's something worth emailing about, then there's 
			// probably something worth updating the index about.
			// Really, though, we wouldn't want to update the index if it were
			// just the status that were changing...
			if (btnet.Util.get_setting("EnableLucene", "1") == "1")
			{
				MyLucene.update_lucene_index(bugid);
			}

			bool notification_email_enabled = (btnet.Util.get_setting("NotificationEmailEnabled", "1") == "1");

			if (!notification_email_enabled)
			{
				return;
			}
			// MAW -- 2006/01/27 -- Determine level of change detected
			int changeLevel = 0;
			if (insert_or_update == INSERT)
			{
				changeLevel = 1;
			}
			else if (status_changed)
			{
				changeLevel = 2;
			}
			else if (assigned_to_changed)
			{
				changeLevel = 3;
			}
			else
			{
				changeLevel = 4;
			}

			string sql;

			if (just_to_this_userid > 0)
			{
				sql = @"
/* get notification email for just one user  */
select us_email, us_id, us_admin, og.*
from bug_subscriptions
inner join users on bs_user = us_id
inner join orgs og on us_org = og_id
inner join bugs on bg_id = bs_bug
left outer join project_user_xref on pu_user = us_id and pu_project = bg_project
where us_email is not null
and us_enable_notifications = 1
-- $status_change
and us_active = 1
and us_email <> ''
and
case
when
	us_org <> bg_org
	and og_other_orgs_permission_level < 2
	and og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
		then og_other_orgs_permission_level
else
	isnull(pu_permission_level,$dpl)
end <> 0
and bs_bug = $id
and us_id = $just_this_usid";

				sql = sql.Replace("$just_this_usid", Convert.ToString(just_to_this_userid));
			}
			else
			{

				// MAW -- 2006/01/27 -- Added different notifications if reported or assigned-to
				sql = @"
/* get notification emails for all subscribers */
select us_email, us_id, us_admin, og.*
from bug_subscriptions
inner join users on bs_user = us_id
inner join orgs og on us_org = og_id
inner join bugs on bg_id = bs_bug
left outer join project_user_xref on pu_user = us_id and pu_project = bg_project
where us_email is not null
and us_enable_notifications = 1
-- $status_change
and us_active = 1
and us_email <> ''
and (   ($cl <= us_reported_notifications and bg_reported_user = bs_user)
or ($cl <= us_assigned_notifications and bg_assigned_to_user = bs_user)
or ($cl <= us_assigned_notifications and $pau = bs_user)
or ($cl <= us_subscribed_notifications))
and
case
when
us_org <> bg_org
and og_other_orgs_permission_level < 2
and og_other_orgs_permission_level < isnull(pu_permission_level,$dpl)
	then og_other_orgs_permission_level
else
isnull(pu_permission_level,$dpl)
end <> 0
and bs_bug = $id
and (us_id <> $us or isnull(us_send_notifications_to_self,0) = 1)";
			}

			sql = sql.Replace("$cl", changeLevel.ToString());
			sql = sql.Replace("$pau", prev_assigned_to_user.ToString());
			sql = sql.Replace("$id", Convert.ToString(bugid));
			sql = sql.Replace("$dpl", btnet.Util.get_setting("DefaultPermissionLevel", "2"));
			sql = sql.Replace("$us", Convert.ToString(security.user.usid));


			DataSet ds_subscribers = btnet.DbUtil.get_dataset(sql);

			if (ds_subscribers.Tables[0].Rows.Count > 0)
			{

				bool added_to_queue = false;


				// Get bug html
				DataRow bug_dr = btnet.Bug.get_bug_datarow(bugid, security);

				string from = btnet.Util.get_setting("NotificationEmailFrom", "");

				// Format the subject line
				string subject = btnet.Util.get_setting("NotificationSubjectFormat", "$THING$:$BUGID$ was $ACTION$ - $SHORTDESC$ $TRACKINGID$");

				subject = subject.Replace("$THING$", btnet.Util.capitalize_first_letter(btnet.Util.get_setting("SingularBugLabel", "bug")));

				string action = "";
				if (insert_or_update == INSERT)
				{
					action = "added";
				}
				else
				{
					action = "updated";
				}

				subject = subject.Replace("$ACTION$", action);
				subject = subject.Replace("$BUGID$", Convert.ToString(bugid));
				subject = subject.Replace("$SHORTDESC$", (string)bug_dr["short_desc"]);

				string tracking_id = " (";
				tracking_id += btnet.Util.get_setting("TrackingIdString", "DO NOT EDIT THIS:");
				tracking_id += Convert.ToString(bugid);
				tracking_id += ")";
				subject = subject.Replace("$TRACKINGID$", tracking_id);

				subject = subject.Replace("$PROJECT$", (string)bug_dr["current_project"]);
				subject = subject.Replace("$ORGANIZATION$", (string)bug_dr["og_name"]);
				subject = subject.Replace("$CATEGORY$", (string)bug_dr["category_name"]);
				subject = subject.Replace("$PRIORITY$", (string)bug_dr["priority_name"]);
				subject = subject.Replace("$STATUS$", (string)bug_dr["status_name"]);
				subject = subject.Replace("$ASSIGNED_TO$", (string)bug_dr["assigned_to_username"]);


				// send a separate email to each subscriber
				foreach (DataRow dr in ds_subscribers.Tables[0].Rows)
				{
					string to = (string)dr["us_email"];

					// Create a fake response and let the code
					// write the html to that response
					System.IO.StringWriter writer = new System.IO.StringWriter();
					HttpResponse my_response = new HttpResponse(writer);
					my_response.Write("<html>");
					my_response.Write("<base href=\"" +
					btnet.Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/") + "\"/>");

					// create a security rec for the user receiving the email
					Security sec2 = new Security();

					// fill in what we know is needed downstream
					sec2.user.is_admin = Convert.ToBoolean(dr["us_admin"]);
					sec2.user.external_user = Convert.ToBoolean(dr["og_external_user"]);
                    sec2.user.tags_field_permission_level = (int)dr["og_tags_field_permission_level"];
					sec2.user.category_field_permission_level = (int)dr["og_category_field_permission_level"];
					sec2.user.priority_field_permission_level = (int)dr["og_priority_field_permission_level"];
					sec2.user.assigned_to_field_permission_level = (int)dr["og_assigned_to_field_permission_level"];
					sec2.user.status_field_permission_level = (int)dr["og_status_field_permission_level"];
					sec2.user.project_field_permission_level = (int)dr["og_project_field_permission_level"];
					sec2.user.org_field_permission_level = (int)dr["og_org_field_permission_level"];
					sec2.user.udf_field_permission_level = (int)dr["og_udf_field_permission_level"];

					DataSet ds_custom = Util.get_custom_columns();
					foreach (DataRow dr_custom in ds_custom.Tables[0].Rows)
					{
						string bg_name = (string)dr_custom["name"];
						string og_name = "og_"
							+ (string)dr_custom["name"]
							+ "_field_permission_level";

						object obj = dr[og_name];
						if (Convert.IsDBNull(obj))
						{
							sec2.user.dict_custom_field_permission_level[bg_name] = Security.PERMISSION_ALL;
						}
						else
						{
							sec2.user.dict_custom_field_permission_level[bg_name] = (int) dr[og_name];
						}

					}

					PrintBug.print_bug(
						my_response,
						bug_dr,
						sec2,
						true,  // include style 
						false, // images_inline 
						true,  // history_inline
                        true); // internal_posts

					// at this point "writer" has the bug html

					sql = @"
delete from queued_notifications where qn_bug = $bug and qn_to = N'$to'

insert into queued_notifications
(qn_date_created, qn_bug, qn_user, qn_status, qn_retries, qn_to, qn_from, qn_subject, qn_body, qn_last_exception)
values (getdate(), $bug, $user, N'not sent', 0, N'$to', N'$from', N'$subject', N'$body', N'')";

					sql = sql.Replace("$bug",Convert.ToString(bugid));
					sql = sql.Replace("$user",Convert.ToString(dr["us_id"]));
					sql = sql.Replace("$to", to.Replace("'","''"));
					sql = sql.Replace("$from", from.Replace("'","''"));
					sql = sql.Replace("$subject", subject.Replace("'","''"));
					sql = sql.Replace("$body", writer.ToString().Replace("'","''"));

					btnet.DbUtil.execute_nonquery_without_logging(sql);

					added_to_queue = true;

				} // end loop through ds_subscribers

				if (added_to_queue)
				{
					// spawn a worker thread to send the emails
                    System.Threading.Thread thread = new System.Threading.Thread(threadproc_notifications);
                    thread.Start();
				}

			}  // if there are any subscribers


		}

        ///////////////////////////////////////////////////////////////////////
        // Send the emails in the queue
        protected static void actually_send_the_emails()
        {
            btnet.Util.write_to_log("actually_send_the_emails");
            
            string sql = @"select * from queued_notifications where qn_status = N'not sent' and qn_retries < 3";
            // create a new one, just in case there would be multithreading issues...

            // get the pending notifications
            DataSet ds = btnet.DbUtil.get_dataset(sql);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {

                string err = "";

                try
                {
                    string to = (string) dr["qn_to"];
                    
                    btnet.Util.write_to_log("sending email to " + to);

                    // try to send it
                    err = btnet.Email.send_email(
                        (string)dr["qn_to"],
                        (string)dr["qn_from"],
                        "", // cc
                        (string)dr["qn_subject"],
                        (string)dr["qn_body"],
                        BtnetMailFormat.Html);

                    if (err == "")
                    {
                        sql = "delete from queued_notifications where qn_id = $qn_id";
                    }
                }
                catch (Exception e)
                {
                    err = e.Message;
                    if (e.InnerException != null)
                    {
                        err += "; ";
                        err += e.InnerException.Message;
                    }

                }

                if (err != "")
                {
                    sql = "update queued_notifications  set qn_retries = qn_retries + 1, qn_last_exception = N'$ex' where qn_id = $qn_id";
                    sql = sql.Replace("$ex", err.Replace("'", "''"));
                }

                sql = sql.Replace("$qn_id", Convert.ToString(dr["qn_id"]));

                // update the row or delete the row
                btnet.DbUtil.execute_nonquery(sql);
            }
        
        }

		///////////////////////////////////////////////////////////////////////
		// Send the emails in the queue
		public static void threadproc_notifications()
		{
			// just to be safe, make the worker threads wait for each other
			lock (dummy)
			{
                try
                {
                    // Don't send emails right away, in case the guy is making a bunch of changes.
                    // Let's consolidate the notifications to one.
                    System.Threading.Thread.Sleep(1000 * 60);
                    actually_send_the_emails();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    btnet.Util.write_to_log("caught ThreadAbortException in threadproc_notifications");
                    actually_send_the_emails();
                }
			}

		} // end of notification thread proc
	}
}