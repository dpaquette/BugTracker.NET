/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;

namespace btnet
{

	public class PrintBug {

        static Regex reEmail = new Regex(
                @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\."
                + @")|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled);

        // convert URL's to hyperlinks
        static Regex reHyperlinks = new Regex(
                //@"(?<Protocol>\w+):\/\/(?<Domain>[\w.]+\/?)\S*",
                @"https?://[-A-Za-z0-9+&@#/%?=~_()|!:,.;]*[-A-Za-z0-9+&@#/%=~_()|]",
                RegexOptions.IgnoreCase
                | RegexOptions.CultureInvariant
                | RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled);


		///////////////////////////////////////////////////////////////////////
		public static void print_bug (HttpResponse Response, DataRow dr, Security security, 
            bool include_style, 
            bool images_inline, 
            bool history_inline,
            bool internal_posts)
		{

			int bugid = Convert.ToInt32(dr["id"]);
			string string_bugid = Convert.ToString(bugid);

            if (include_style) // when sending emails
            {
                Response.Write("\n<style>\n");

                // If this file exists, use it.

                string map_path = (string) HttpRuntime.Cache["MapPath"];

                string css_for_email_file = map_path + "\\custom\\btnet_css_for_email.css";

                try
                {
                    if (System.IO.File.Exists(css_for_email_file))
                    {
                        Response.WriteFile(css_for_email_file);
					    Response.Write("\n");
                    }
                    else
                    {
                        css_for_email_file = map_path + "\\btnet_base.css";
                        Response.WriteFile(css_for_email_file);
					    Response.Write("\n");
                        css_for_email_file = map_path + "\\custom\\" + "btnet_custom.css";
                        if (System.IO.File.Exists(css_for_email_file))
                        {
                            Response.WriteFile(css_for_email_file);
                            Response.Write("\n");
                        }
                    }
                }
                catch (Exception e)
                {
                    btnet.Util.write_to_log("Exception trying to read css file for email \"" 
                        + css_for_email_file
                        + "\":" 
                        + e.Message);
                }

                // underline links in the emails to make them more obvious
                Response.Write("\na {text-decoration: underline; }");
                Response.Write("\na:visited {text-decoration: underline; }");
                Response.Write("\na:hover {text-decoration: underline; }");
                Response.Write("\n</style>\n");
            }

			Response.Write ("<body style='background:white'>");
			Response.Write ("<b>"
				+ btnet.Util.capitalize_first_letter(btnet.Util.get_setting("SingularBugLabel","bug"))
				+ " ID:&nbsp;<a href="
				+ btnet.Util.get_setting("AbsoluteUrlPrefix","http://127.0.0.1/")
				+ "edit_bug.aspx?id="
				+ string_bugid
				+ ">"
				+ string_bugid
				+ "</a>");

            if (btnet.Util.get_setting("EnableMobile", "0") == "1")
            {
                Response.Write(
                    "&nbsp;&nbsp;&nbsp;&nbsp;Mobile link:&nbsp;<a href="
                    + btnet.Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
                    + "mbug.aspx?id="
                    + string_bugid
                    + ">"
                    + btnet.Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
                    + "mbug.aspx?id="
                    + string_bugid
                    + "</a>");

            }

            Response.Write("<br>");

			Response.Write ("Short desc:&nbsp;<a href="
				+ btnet.Util.get_setting("AbsoluteUrlPrefix","http://127.0.0.1/")
				+ "edit_bug.aspx?id="
				+ string_bugid
				+ ">"
				+ HttpUtility.HtmlEncode((string)dr["short_desc"])
				+ "</a></b><p>");

			// start of the table with the bug fields
			Response.Write ("\n<table border=1 cellpadding=3 cellspacing=0>");
            Response.Write("\n<tr><td>Last changed by<td>"
				+ format_username((string)dr["last_updated_user"],(string)dr["last_updated_fullname"])
				+ "&nbsp;");
            Response.Write("\n<tr><td>Reported By<td>"
				+ format_username((string)dr["reporter"],(string)dr["reporter_fullname"])
				+ "&nbsp;");
            Response.Write("\n<tr><td>Reported On<td>" + btnet.Util.format_db_date_and_time(dr["reported_date"]) + "&nbsp;");

            if (security.user.tags_field_permission_level > 0)
	            Response.Write("\n<tr><td>Tags<td>" + dr["bg_tags"] + "&nbsp;");

            if (security.user.project_field_permission_level > 0)
	            Response.Write("\n<tr><td>Project<td>" + dr["current_project"] + "&nbsp;");

            if (security.user.org_field_permission_level > 0)
	            Response.Write("\n<tr><td>Organization<td>" + dr["og_name"] + "&nbsp;");

            if (security.user.category_field_permission_level > 0)
	            Response.Write("\n<tr><td>Category<td>" + dr["category_name"] + "&nbsp;");

            if (security.user.priority_field_permission_level > 0)
	            Response.Write("\n<tr><td>Priority<td>" + dr["priority_name"] + "&nbsp;");

            if (security.user.assigned_to_field_permission_level > 0)
	            Response.Write("\n<tr><td>Assigned<td>"
					+ format_username((string)dr["assigned_to_username"],(string)dr["assigned_to_fullname"])
					+ "&nbsp;");

            if (security.user.status_field_permission_level > 0)
            	Response.Write("\n<tr><td>Status<td>" + dr["status_name"] + "&nbsp;");

			if (security.user.udf_field_permission_level > 0)
				if (btnet.Util.get_setting("ShowUserDefinedBugAttribute","1") == "1")
				{
					Response.Write("\n<tr><td>"
						+ btnet.Util.get_setting("UserDefinedBugAttributeName","YOUR ATTRIBUTE")
						+ "<td>"
						+ dr["udf_name"] + "&nbsp;");
				}

			// Get custom column info  (There's an inefficiency here - we just did this
			// same call in get_bug_datarow...)

			
			DataSet ds_custom_cols = btnet.Util.get_custom_columns();


			// Show custom columns

			foreach (DataRow drcc in ds_custom_cols.Tables[0].Rows)
			{
                string column_name = (string) drcc["name"];

                if (security.user.dict_custom_field_permission_level[column_name] == Security.PERMISSION_NONE)
                {
                    continue;
                }

                Response.Write("\n<tr><td>");
				Response.Write (column_name);
				Response.Write ("<td>");

				if ((string)drcc["datatype"] == "datetime")
				{
					object dt = dr[(string)drcc["name"]];

					Response.Write (btnet.Util.format_db_date_and_time(dt));
				}
				else
				{
					string s = "";

					if ((string)drcc["dropdown type"] == "users")
					{
						object obj = dr[(string)drcc["name"]];
						if (obj.GetType() != typeof(System.DBNull))
						{
							int userid = Convert.ToInt32(obj);
							if (userid != 0)
							{
								string sql_get_username = "select us_username from users where us_id = $1";
								s = (string) btnet.DbUtil.execute_scalar(sql_get_username.Replace("$1", Convert.ToString(userid)));
							}
						}
					}
					else
					{
						s = Convert.ToString(dr[(string)drcc["name"]]);
					}

					s = HttpUtility.HtmlEncode(s);
					s = s.Replace("\n","<br>");
					s = s.Replace("  ","&nbsp; ");
					s = s.Replace("\t","&nbsp;&nbsp;&nbsp;&nbsp;");
					Response.Write (s);
				}
				Response.Write ("&nbsp;");
			}


			// create project custom dropdowns
			if ((int)dr["project"] != 0)
			{

				string sql = @"select
					isnull(pj_enable_custom_dropdown1,0) [pj_enable_custom_dropdown1],
					isnull(pj_enable_custom_dropdown2,0) [pj_enable_custom_dropdown2],
					isnull(pj_enable_custom_dropdown3,0) [pj_enable_custom_dropdown3],
					isnull(pj_custom_dropdown_label1,'') [pj_custom_dropdown_label1],
					isnull(pj_custom_dropdown_label2,'') [pj_custom_dropdown_label2],
					isnull(pj_custom_dropdown_label3,'') [pj_custom_dropdown_label3]
					from projects where pj_id = $pj";

				sql = sql.Replace("$pj", Convert.ToString((int)dr["project"]));

				DataRow project_dr = btnet.DbUtil.get_datarow(sql);


				if (project_dr != null)
				{
					for (int i = 1; i < 4; i++)
					{
						if ((int)project_dr["pj_enable_custom_dropdown" + Convert.ToString(i)] == 1)
						{
                            Response.Write("\n<tr><td>");
							Response.Write (project_dr["pj_custom_dropdown_label" + Convert.ToString(i)]);
							Response.Write ("<td>");
							Response.Write (dr["bg_project_custom_dropdown_value"  + Convert.ToString(i)]);
							Response.Write ("&nbsp;");
						}
					}
				}
			}



			Response.Write("\n</table><p>"); // end of the table with the bug fields

			// Relationships
			if (btnet.Util.get_setting("EnableRelationships", "0") == "1")
			{
				write_relationships(Response, bugid);
			}

			// Tasks
			if (btnet.Util.get_setting("EnableTasks", "0") == "1")
			{
				write_tasks(Response, bugid);
			}


            DataSet ds_posts = get_bug_posts(bugid, security.user.external_user, history_inline);
			write_posts (
                ds_posts,
                Response, 
                bugid, 
                0, 
                false, /* don't write links */
                images_inline, 
                history_inline, 
                internal_posts,
                security.user);

			Response.Write ("</body>");

		}


		///////////////////////////////////////////////////////////////////////
		protected static void write_tasks(HttpResponse Response, int bugid)
		{
			
			DataSet ds_tasks = btnet.Util.get_all_tasks(null, bugid);

            if (ds_tasks.Tables[0].Rows.Count > 0)
            {
                Response.Write("<b>Tasks</b><p>");

                SortableHtmlTable.create_nonsortable_from_dataset(
                    Response, ds_tasks);
            }

		}
		
		///////////////////////////////////////////////////////////////////////
		protected static void write_relationships(HttpResponse Response, int bugid)
		{
		
			string sql = @"select bg_id [id],
				bg_short_desc [desc],
				re_type [comment],
				case
					when re_direction = 0 then ''
					when re_direction = 2 then 'child of $bg'
					else 'parent of $bg' end [parent/child]
				from bug_relationships
				inner join bugs on re_bug2 = bg_id
				where re_bug1 = $bg
				order by 1";

			sql = sql.Replace("$bg", Convert.ToString(bugid));
			DataSet ds_relationships = btnet.DbUtil.get_dataset(sql);

			if (ds_relationships.Tables[0].Rows.Count > 0)
			{
				Response.Write ("<b>Relationships</b><p><table border=1 class=datat><tr>");
				Response.Write ("<td class=datah valign=bottom>id</td>");
				Response.Write ("<td class=datah valign=bottom>desc</td>");
				Response.Write ("<td class=datah valign=bottom>comment</td>");
				Response.Write ("<td class=datah valign=bottom>parent/child</td>");

				foreach (DataRow dr_relationships in ds_relationships.Tables[0].Rows)
				{
					Response.Write ("<tr>");

					Response.Write ("<td class=datad valign=top align=right>");
					Response.Write (Convert.ToString((int) dr_relationships["id"]));

					Response.Write ("<td class=datad valign=top>");
					Response.Write (Convert.ToString(dr_relationships["desc"]));

					Response.Write ("<td class=datad valign=top>");
					Response.Write (Convert.ToString(dr_relationships["comment"]));

					Response.Write ("<td class=datad valign=top>");
					Response.Write (Convert.ToString(dr_relationships["parent/child"]));

				}

				Response.Write ("</table><p>");

			}
		
		}

		///////////////////////////////////////////////////////////////////////
		public static int write_posts(
			DataSet ds_posts,
            HttpResponse Response,
			int bugid,
			int permission_level,
			bool write_links,
			bool images_inline,
            bool history_inline,
            bool internal_posts,
			btnet.User user)
		{

			if (Util.get_setting("ForceBordersInEmails","0") == "1")
			{
				Response.Write ("\n<table id='posts_table' border=1 cellpadding=0 cellspacing=3>");
			}
			else
			{
				Response.Write ("\n<table id='posts_table' border=0 cellpadding=0 cellspacing=3>");
			}

			int post_cnt = ds_posts.Tables[0].Rows.Count;
			
			int bp_id;
			int prev_bp_id = -1;

			
			foreach (DataRow dr in ds_posts.Tables[0].Rows)
			{

                if (!internal_posts)
                {
                    if ((int)dr["bp_hidden_from_external_users"] == 1)
                    {
                        continue; 
                    }
                }

                bp_id = (int) dr["bp_id"];

				if ((string)dr["bp_type"] == "update")
				{

					string comment = (string) dr["bp_comment"];

					if (user.tags_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed tags from"))
						continue;

					if (user.project_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed project from"))
						continue;

					if (user.org_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed organization from"))
						continue;

					if (user.category_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed category from"))
						continue;

					if (user.priority_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed priority from"))
						continue;

					if (user.assigned_to_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed assigned_to from"))
						continue;

					if (user.status_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed status from"))
						continue;

					if (user.udf_field_permission_level == Security.PERMISSION_NONE
					&& comment.StartsWith("changed " + Util.get_setting("UserDefinedBugAttributeName","YOUR ATTRIBUTE") + " from"))
						continue;

                    bool bSkip = false;
                    foreach (string key in user.dict_custom_field_permission_level.Keys)
                    { 
                        int field_permission_level = user.dict_custom_field_permission_level[key];
                        if (field_permission_level == Security.PERMISSION_NONE)
                        {
                            if (comment.StartsWith("changed " + key + " from"))
                            {
                                bSkip = true;
                            }
                        }
                    }
                    if (bSkip)
                    {
                        continue;
                    }

				}

				if (bp_id == prev_bp_id)
				{
					// show another attachment
					write_email_attachment(Response, bugid, dr, write_links, images_inline);
				}
				else
				{
					// show the comment and maybe an attachment
					if (prev_bp_id != -1) {
						Response.Write ("\n</table>"); // end the previous table
					}

					write_post(Response, bugid, permission_level, dr, bp_id, write_links, images_inline,
						user.is_admin,
						user.can_edit_and_delete_posts,
						user.external_user);


					if (Convert.ToString(dr["ba_file"]) != "") // intentially "ba"
					{
						write_email_attachment(Response, bugid, dr, write_links, images_inline);
					}
					prev_bp_id = bp_id;
				}

			}

			if (prev_bp_id != -1)
			{
				Response.Write ("\n</table>"); // end the previous table
			}

			Response.Write ("\n</table>");
			
			return post_cnt;
		}

		///////////////////////////////////////////////////////////////////////
		static void write_post(
			HttpResponse Response,
			int bugid,
			int permission_level,
			DataRow dr,
			int post_id,
			bool write_links,
			bool images_inline,
			bool this_is_admin,
            bool this_can_edit_and_delete_posts,
            bool this_external_user)
		{
			string type = (string)dr["bp_type"];

			string string_post_id = Convert.ToString(post_id);
			string string_bug_id = Convert.ToString(bugid);

            if ((int) dr["seconds_ago"] < 2 && write_links)
			{
				// for the animation effect
                Response.Write ("\n\n<tr><td class=cmt name=new_post>\n<table width=100%>\n<tr><td align=left>");
			}
			else
			{
				Response.Write ("\n\n<tr><td class=cmt>\n<table width=100%>\n<tr><td align=left>");
			}


			/*
				Format one of the following:

				changed by
				email sent to
				email received from
				file attached by
				comment posted by

			*/

			if (type == "update")
			{
				if (write_links)
				{
					Response.Write ("<img src=database.png align=top>&nbsp;");
				}			
			
				// posted by
				Response.Write ("<span class=pst>changed by ");
				Response.Write (format_email_username(
					write_links,
					bugid,
                    permission_level,
					(string) dr["us_email"],
					(string) dr["us_username"],
					(string) dr["us_fullname"]));
			}
			else if (type == "sent")
			{
				if (write_links)
				{
					Response.Write ("<img src=email_edit.png align=top>&nbsp;");
				}
				
				Response.Write ("<span class=pst>email <a name=" + Convert.ToString(post_id) +  "></a>" + Convert.ToString(post_id) + " sent to ");

				if (write_links)
				{
					Response.Write (format_email_to(
						bugid,
						HttpUtility.HtmlEncode((string)dr["bp_email_to"])));
				}
				else
				{
					Response.Write (HttpUtility.HtmlEncode((string)dr["bp_email_to"]));
				}

				if ((string) dr["bp_email_cc"] != "")
				{
					Response.Write (", cc: ");

					if (write_links)
					{
						Response.Write (format_email_to(
							bugid,
							HttpUtility.HtmlEncode((string)dr["bp_email_cc"])));
					}
					else
					{
						Response.Write (HttpUtility.HtmlEncode((string)dr["bp_email_cc"]));
					}

					Response.Write (", ");
				}

				Response.Write (" by ");

				Response.Write (format_email_username(
					write_links,
					bugid,
                    permission_level,
					(string) dr["us_email"],
					(string) dr["us_username"],
					(string) dr["us_fullname"]));

			}
			else if (type == "received" )
			{
				if (write_links)
				{
					Response.Write ("<img src=email_open.png align=top>&nbsp;");
				}
				Response.Write ("<span class=pst>email <a name=" + Convert.ToString(post_id) +  "></a>" + Convert.ToString(post_id) + " received from ");
				if (write_links)
				{
					Response.Write (format_email_from(
						post_id,
						(string)dr["bp_email_from"]));
				}
				else
				{
					Response.Write ((string)dr["bp_email_from"]);
				}
			}
			else if (type == "file" )
			{
				if ((int) dr["bp_hidden_from_external_users"] == 1)
				{
					Response.Write("<div class=private>Internal Only!</div>");
				}
				Response.Write ("<span class=pst>file <a name=" + Convert.ToString(post_id) +  "></a>" + Convert.ToString(post_id) + " attached by ");
				Response.Write (format_email_username(
					write_links,
					bugid,
                    permission_level,
					(string) dr["us_email"],
					(string) dr["us_username"],
					(string) dr["us_fullname"]));
			}
			else if (type == "comment" )
			{
				if ((int) dr["bp_hidden_from_external_users"] == 1)
				{
					Response.Write("<div class=private>Internal Only!</div>");
				}
				
				if (write_links)
				{
					Response.Write ("<img src=comment.png align=top>&nbsp;");
				}
				
				Response.Write ("<span class=pst>comment <a name=" + Convert.ToString(post_id) +  "></a>" + Convert.ToString(post_id) + " posted by ");
				Response.Write (format_email_username(
					write_links,
					bugid,
                    permission_level,
					(string) dr["us_email"],
					(string) dr["us_username"],
					(string) dr["us_fullname"]));
			}
			else
			{
				System.Diagnostics.Debug.Assert(false);
			}


			// Format the date
			Response.Write (" on ");
			Response.Write (btnet.Util.format_db_date_and_time(dr["bp_date"]));
			Response.Write (", ");
			Response.Write (btnet.Util.how_long_ago((int) dr["seconds_ago"]));
			Response.Write ("</span>");


			// Write the links

			if (write_links)
			{

				Response.Write ("<td align=right>&nbsp;");

				if (permission_level != Security.PERMISSION_READONLY)
				{
					if (type == "comment" || type == "sent" || type == "received")
					{
						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=send_email.aspx?quote=1&bp_id=" + string_post_id + "&reply=forward");
						Response.Write (">forward</a>");
					}
				}

				// format links for responding to email
				if (type == "received" )
				{
					if (this_is_admin
					|| (this_can_edit_and_delete_posts
					&& permission_level == Security.PERMISSION_ALL))
					{
					// This doesn't just work.  Need to make changes in edit/delete pages.
					//	Response.Write ("&nbsp;&nbsp;&nbsp;<a style='font-size: 8pt;'");
					//	Response.Write (" href=edit_comment.aspx?id="
					//		+ string_post_id + "&bug_id=" + string_bug_id);
					//	Response.Write (">edit</a>");

						// This delete leaves debris around, but it's better than nothing
						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=delete_comment.aspx?id="
							+ string_post_id + "&bug_id=" + string_bug_id);
						Response.Write (">delete</a>");

					}

					if (permission_level != Security.PERMISSION_READONLY)
					{
						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=send_email.aspx?quote=1&bp_id=" + string_post_id);
						Response.Write (">reply</a>");

						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=send_email.aspx?quote=1&bp_id=" + string_post_id + "&reply=all");
						Response.Write (">reply all</a>");
					}

				}
				else if (type == "file")
				{

					if (this_is_admin
					|| (this_can_edit_and_delete_posts
					&& permission_level == Security.PERMISSION_ALL))
					{
						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=edit_attachment.aspx?id="
							+ string_post_id + "&bug_id=" + string_bug_id);
						Response.Write (">edit</a>");

						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=delete_attachment.aspx?id="
							+ string_post_id + "&bug_id=" + string_bug_id);
						Response.Write (">delete</a>");

					}
				}
				else if (type == "comment")
				{
					if (this_is_admin
					|| (this_can_edit_and_delete_posts
					&& permission_level == Security.PERMISSION_ALL))
					{
						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=edit_comment.aspx?id="
							+ string_post_id + "&bug_id=" + string_bug_id);
						Response.Write (">edit</a>");

						Response.Write ("&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;'");
						Response.Write (" href=delete_comment.aspx?id="
							+ string_post_id + "&bug_id=" + string_bug_id);
						Response.Write (">delete</a>");
					}
				}


				// custom bug link
				if (btnet.Util.get_setting("CustomPostLinkLabel","") != "")
				{

					string custom_post_link = "&nbsp;&nbsp;&nbsp;<a class=warn style='font-size: 8pt;' href="
						+ btnet.Util.get_setting("CustomPostLinkUrl","")
						+ "?postid="
						+ string_post_id
						+ ">"
						+ btnet.Util.get_setting("CustomPostLinkLabel","")
						+ "</a>";

					Response.Write (custom_post_link);

				}
			}

			Response.Write ("\n</table>\n<table border=0>\n<tr><td>");
			// the text itself
			string comment = (string) dr["bp_comment"];
			string comment_type = (string) dr["bp_content_type"];

            if (write_links)
            {
                comment = format_comment(post_id, comment, comment_type);
            }
            else
            {
                comment = format_comment(0, comment, comment_type);
            }



			if (type == "file")
			{
				if (comment.Length > 0)
				{
					Response.Write (comment);
					Response.Write ("<p>");
				}

				Response.Write ("<span class=pst>");
				if (write_links)
				{
					Response.Write("<img src=attach.gif>");
				}
				Response.Write ("attachment:&nbsp;</span><span class=cmt_text>");
				Response.Write (dr["bp_file"]);
				Response.Write ("</span>");

				if (write_links)
				{
                    if ((string)dr["bp_content_type"] != "text/html" || Util.get_setting("ShowPotentiallyDangerousHtml", "0") == "1")
                    {
                        Response.Write("&nbsp;&nbsp;&nbsp;<a target=_blank style='font-size: 8pt;'");
                        Response.Write(" href=view_attachment.aspx?download=0&id="
                            + string_post_id + "&bug_id=" + string_bug_id);
                        Response.Write(">view</a>");
                    }

                    Response.Write("&nbsp;&nbsp;&nbsp;<a target=_blank style='font-size: 8pt;'");
					Response.Write (" href=view_attachment.aspx?download=1&id="
						+ string_post_id + "&bug_id=" + string_bug_id);
					Response.Write (">save</a>");
				}

				Response.Write ("<p><span class=pst>size: ");
				Response.Write (dr["bp_size"]);
				Response.Write ("&nbsp;&nbsp;&nbsp;content-type: ");
				Response.Write (dr["bp_content_type"]);
				Response.Write ("</span>");


			}
			else
			{
				Response.Write (comment);
			}


			// maybe show inline images
			if (type == "file")
			{
				if (images_inline)
				{
					string file = Convert.ToString(dr["bp_file"]);
					write_file_inline (Response, file, string_post_id, string_bug_id, (string) dr["bp_content_type"]);
				}
			}

		}


		///////////////////////////////////////////////////////////////////////
		static void write_email_attachment(HttpResponse Response, int bugid, DataRow dr, bool write_links, bool images_inline)
		{

			string string_post_id = Convert.ToString(dr["ba_id"]); // intentially "ba"
			string string_bug_id = Convert.ToString(bugid);

			Response.Write ("\n<p><span class=pst>");
			if (write_links)
			{
				Response.Write("<img src=attach.gif>");
			}
			Response.Write("attachment:&nbsp;</span>");
			Response.Write (dr["ba_file"]); // intentially "ba"
			Response.Write ("&nbsp;&nbsp;&nbsp;&nbsp;");

			if (write_links)
			{
                if ((string)dr["bp_content_type"] != "text/html" || Util.get_setting("ShowPotentiallyDangerousHtml", "0") == "1")
                {
                    Response.Write("<a target=_blank href=view_attachment.aspx?download=0&id=");
                    Response.Write(string_post_id);
                    Response.Write("&bug_id=");
                    Response.Write(string_bug_id);
                    Response.Write(">view</a>");
                }

                Response.Write("&nbsp;&nbsp;&nbsp;<a target=_blank href=view_attachment.aspx?download=1&id=");
				Response.Write (string_post_id);
				Response.Write ("&bug_id=");
				Response.Write (string_bug_id);
				Response.Write (">save</a>");
			}

			if (images_inline)
			{
				string file = Convert.ToString(dr["ba_file"]);  // intentially "ba"
				write_file_inline (Response, file, string_post_id, string_bug_id, (string) dr["ba_content_type"]);

			}

			Response.Write ("<p><span class=pst>size: ");
			Response.Write (dr["ba_size"]);
			Response.Write ("&nbsp;&nbsp;&nbsp;content-type: ");
			Response.Write (dr["ba_content_type"]);
			Response.Write ("</span>");

		}


		///////////////////////////////////////////////////////////////////////
		static void write_file_inline (
			HttpResponse Response, 
			string filename, 
			string string_post_id, 
			string string_bug_id,
			string content_type)
		{

			if (content_type =="image/gif"
			|| content_type == "image/jpg"
			|| content_type == "image/jpeg"
			|| content_type == "image/pjpeg"
			|| content_type == "image/png"
			|| content_type == "image/x-png"
			|| content_type == "image/bmp"
			|| content_type == "image/tiff")
			{
				Response.Write ("<p>"
					+ "<a href=javascript:resize_image('im" + string_post_id + "',1.5)>" + "[+]</a>&nbsp;"
					+ "<a href=javascript:resize_image('im" + string_post_id + "',.6)>" + "[-]</a>"
					+ "<br><img id=im" + string_post_id
					+ " src=view_attachment.aspx?download=0&id="
					+ string_post_id + "&bug_id=" + string_bug_id
					+ ">");
			}
			else if (content_type == "text/plain"
			|| content_type == "text/xml"
			|| content_type == "text/css"
			|| content_type == "text/js"
            || (content_type == "text/html" && Util.get_setting("ShowPotentiallyDangerousHtml", "0") == "1"))
			{
				Response.Write ("<p>"
					+ "<a href=javascript:resize_iframe('if" + string_post_id + "',200)>" + "[+]</a>&nbsp;"
					+ "<a href=javascript:resize_iframe('if" + string_post_id + "',-200)>" + "[-]</a>"
					+ "<br><iframe id=if"
					+ string_post_id
					+ " width=780 height=200 src=view_attachment.aspx?download=0&id="
					+ string_post_id + "&bug_id=" + string_bug_id
					+ "></iframe>");
			}

		}

        ///////////////////////////////////////////////////////////////////////
        public static string format_email_username(
            bool write_links,
            int bugid,
            int permission_level,
            string email,
            string username,
            string fullname)
        {
            if (email != null && email != "" && write_links && permission_level != Security.PERMISSION_READONLY)
            {
                return "<a href="
                + Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
                + "send_email.aspx?bg_id=" 
                + Convert.ToString(bugid)
                + "&to=" 
                + email 
                + ">"
                + format_username(username, fullname)
                + "</a>";
            }
            else
            {
                return format_username(username, fullname);
            }

        }

        ///////////////////////////////////////////////////////////////////////
        static string format_email_to(int bugid, string email)
        {
            return "<a href="
            + Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
            + "send_email.aspx?bg_id=" + Convert.ToString(bugid)
            + "&to=" + HttpUtility.UrlEncode(HttpUtility.HtmlDecode(email)) + ">"
            + email
            + "</a>";
        }


        ///////////////////////////////////////////////////////////////////////
        static string format_email_from(int comment_id, string from)
        {

            string display_part = "";
            string email_part = "";
            int pos = from.IndexOf("<"); // "

            if (pos > 0)
            {
                display_part = from.Substring(0, pos);
                email_part = from.Substring(pos + 1, (from.Length - pos) - 2);
            }
            else
            {
                email_part = from;
            }

            return display_part
                + " <a href="
                + Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
                + "send_email.aspx?bp_id="
                + Convert.ToString(comment_id)
                + ">"
                + email_part
                + "</a>";

        }

        ///////////////////////////////////////////////////////////////////////
        static string format_comment(int post_id, string s1, string t1)
        {
            string s2;
            string link_marker;

            if (t1 != "text/html")
            {
                s2 = HttpUtility.HtmlEncode(s1);

                if (post_id != 0)
                {
                    // convert urls to links
                    s2 = reHyperlinks.Replace(
                        s2,
                        new MatchEvaluator(convert_to_hyperlink));

                    // This code doesn't perform well if s2 is one big string, no spaces, line breaks

                    // convert email addresses to send_email links
                    s2 = reEmail.Replace(
                        s2,
                        delegate(Match m)
                        {
                            return
                              "<a href=send_email.aspx?bp_id="
                              + Convert.ToString(post_id)
                              + "&to="
                              + m.ToString()
                              + ">"
                              + m.ToString()
                              + "</a>";
                        }
                        );
                }

                s2 = s2.Replace("\n", "<br>");
                s2 = s2.Replace("  ", " &nbsp;");
                s2 = s2.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

            }
            else
            {
                s2 = s1;
			}
			
            // convert references to other bugs to links
			link_marker = Util.get_setting("BugLinkMarker", "bugid#");
			Regex reLinkMarker = new Regex(link_marker + "([0-9]+)");
			s2 = reLinkMarker.Replace(
				s2,
				new MatchEvaluator(convert_bug_link));

			return "<span class=cmt_text>" + s2 + "</span>";
        }

        ///////////////////////////////////////////////////////////////////////
        static string convert_to_email(Match m)
        {
            // Get the matched string.
            return String.Format("<a href='mailto:{0}'>{0}</a>", m.ToString());
        }

        ///////////////////////////////////////////////////////////////////////
        static string convert_bug_link(Match m)
        {
            return "<a href="
                + btnet.Util.get_setting("AbsoluteUrlPrefix", "http://127.0.0.1/")
                + "edit_bug.aspx?id="
                + m.Groups[1].ToString()
                + ">"
                + m.ToString()
                + "</a>";
        }

        ///////////////////////////////////////////////////////////////////////
        static string convert_to_hyperlink(Match m)
        {
            return String.Format("<a target=_blank href='{0}'>{0}</a>", m.ToString());
        }

        ///////////////////////////////////////////////////////////////////////
        static string format_username(string username, string fullname)
        {

            if (Util.get_setting("UseFullNames", "0") == "0")
            {
                return username;
            }
            else
            {
                return fullname;
            }
        }


        ///////////////////////////////////////////////////////////////////////
        public static DataSet get_bug_posts(int bugid, bool external_user, bool history_inline)
        {
            string sql = @"
/* get_bug_posts */
select
a.bp_bug,
a.bp_comment,
isnull(us_username,'') [us_username],
case rtrim(us_firstname)
	when null then isnull(us_lastname, '')
	when '' then isnull(us_lastname, '')
	else isnull(us_lastname + ', ' + us_firstname,'')
	end [us_fullname],
isnull(us_email,'') [us_email],
a.bp_date,
datediff(s,a.bp_date,getdate()) [seconds_ago],
a.bp_id,
a.bp_type,
isnull(a.bp_email_from,'') bp_email_from,
isnull(a.bp_email_to,'') bp_email_to,
isnull(a.bp_email_cc,'') bp_email_cc,
isnull(a.bp_file,'') bp_file,
isnull(a.bp_size,0) bp_size,
isnull(a.bp_content_type,'') bp_content_type,
a.bp_hidden_from_external_users,
isnull(ba.bp_file,'') ba_file,  -- intentionally ba
isnull(ba.bp_id,'') ba_id, -- intentionally ba
isnull(ba.bp_size,'') ba_size,  -- intentionally ba
isnull(ba.bp_content_type,'') ba_content_type -- intentionally ba
from bug_posts a
left outer join users on us_id = a.bp_user
left outer join bug_posts ba on ba.bp_parent = a.bp_id and ba.bp_bug = a.bp_bug
where a.bp_bug = $id
and a.bp_parent is null";


			if (!history_inline)
			{
				sql += "\n and a.bp_type <> 'update'";
			}
			
			if (external_user)
			{
				sql += "\n and a.bp_hidden_from_external_users = 0";
			}
			
			sql += "\n order by a.bp_id "; 
			sql += btnet.Util.get_setting("CommentSortOrder","desc");
			sql += ", ba.bp_parent, ba.bp_id";

            sql = sql.Replace("$id", Convert.ToString(bugid));
            
            return btnet.DbUtil.get_dataset(sql);

        }


	} // end PrintBug

    public class WritePostResult
    {
        public int post_cnt;
        public string post_string;
        public string related_bugs;
    }

} // end namespace