/*
Copyright 2002-2011 Corey Trager
Distributed under the terms of the GNU General Public License
*/

using System;
using System.Web;
using System.Data;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace btnet
{

	public class Security {


        public const int MUST_BE_ADMIN = 1;
		public const int ANY_USER_OK = 2;
		public const int ANY_USER_OK_EXCEPT_GUEST = 3;
		public const int MUST_BE_ADMIN_OR_PROJECT_ADMIN = 4;
        public const int PERMISSION_NONE = 0;
        public const int PERMISSION_READONLY = 1;
        public const int PERMISSION_REPORTER = 3;
        public const int PERMISSION_ALL = 2;

        public User user = new User();
        public string auth_method = "";
        public HttpContext context = null;

        static string goto_form = @"
<td nowrap valign=middle>
    <form style='margin: 0px; padding: 0px;' action=edit_bug.aspx method=get>
        <input class=menubtn type=submit value='go to ID'>
        <input class=menuinput size=4 type=text class=txt name=id accesskey=g>
    </form>
</td>";

		///////////////////////////////////////////////////////////////////////
		public void check_security(HttpContext asp_net_context, int level)
		{
			Util.set_context(asp_net_context);
			HttpRequest Request = asp_net_context.Request;
			HttpResponse Response = asp_net_context.Response;
			HttpCookie cookie = Request.Cookies["se_id"];

			// This logic allows somebody to put a link in an email, like
			// edit_bug.aspx?id=66
			// The user would click on the link, go to the logon page (default.aspx),
			// and then after logging in continue on to edit_bug.aspx?id=66
			string original_url = Request.ServerVariables["URL"].ToString().ToLower();
			string original_querystring = Request.ServerVariables["QUERY_STRING"].ToString().ToLower();

			string target = "default.aspx";

            if (original_url.EndsWith("mbug.aspx"))
            {
                target = "mlogin.aspx";
            }

            target += "?url=" + original_url + "&qs=" + HttpUtility.UrlEncode(original_querystring);

			DataRow dr = null;

			if (cookie == null)
			{
				if (Util.get_setting("AllowGuestWithoutLogin","0") == "0")
				{
					Util.write_to_log ("se_id cookie is null, so redirecting");
					Response.Redirect(target);
				}
			}
			else
			{
				// guard against "Sql Injection" exploit
				string se_id = cookie.Value.Replace("'", "''");
				int user_id = 0;
				object obj = asp_net_context.Session[se_id];
				if (obj != null)
				{
					user_id = Convert.ToInt32(obj);
				}

				// check for existing session for active user
				string sql = @"
/* check session */
declare @project_admin int
select @project_admin = count(1)
	from sessions
	inner join project_user_xref on pu_user = se_user
	and pu_admin = 1
	where se_id = '$se';

select us_id, us_admin,
us_username, us_firstname, us_lastname,
isnull(us_email,'') us_email,
isnull(us_bugs_per_page,10) us_bugs_per_page,
isnull(us_forced_project,0) us_forced_project,
us_use_fckeditor,
us_enable_bug_list_popups,
og.*,
isnull(us_forced_project, 0 ) us_forced_project,
isnull(pu_permission_level, $dpl) pu_permission_level,
@project_admin [project_admin]
from sessions
inner join users on se_user = us_id
inner join orgs og on us_org = og_id
left outer join project_user_xref
	on pu_project = us_forced_project
	and pu_user = us_id
where se_id = '$se'
and us_active = 1";

				sql = sql.Replace("$se", se_id);
				sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel","2"));
				dr = btnet.DbUtil.get_datarow(sql);

			}

			if (dr == null)
			{
				if (Util.get_setting("AllowGuestWithoutLogin","0") == "1")
				{
					// allow users in, even without logging on.
					// The user will have the permissions of the "guest" user.
					string sql = @"
/* get guest  */
select us_id, us_admin,
us_username, us_firstname, us_lastname,
isnull(us_email,'') us_email,
isnull(us_bugs_per_page,10) us_bugs_per_page,
isnull(us_forced_project,0) us_forced_project,
us_use_fckeditor,
us_enable_bug_list_popups,
og.*,
isnull(us_forced_project, 0 ) us_forced_project,
isnull(pu_permission_level, $dpl) pu_permission_level,
0 [project_admin]
from users
inner join orgs og on us_org = og_id
left outer join project_user_xref
	on pu_project = us_forced_project
	and pu_user = us_id
where us_username = 'guest'
and us_active = 1";

					sql = sql.Replace("$dpl", Util.get_setting("DefaultPermissionLevel","2"));
					dr = btnet.DbUtil.get_datarow(sql);
				}
			}

			// no previous session, no guest login allowed
			if (dr == null)
			{
				btnet.Util.write_to_log("no previous session, no guest login allowed");
				Response.Redirect(target);
			}
			else
			{
				user.set_from_db(dr);
			}


            if (cookie != null)
            {
            	asp_net_context.Session["session_cookie"] = cookie.Value;
			}
			else
			{
				btnet.Util.write_to_log("blanking cookie");
				asp_net_context.Session["session_cookie"]  = "";
			}

			if (level == MUST_BE_ADMIN && !user.is_admin)
			{
				btnet.Util.write_to_log("must be admin, redirecting");
				Response.Redirect("default.aspx");
			}
			else if (level == ANY_USER_OK_EXCEPT_GUEST && user.is_guest)
			{
				btnet.Util.write_to_log("cant be guest, redirecting");
				Response.Redirect("default.aspx");
			}
			else if (level == MUST_BE_ADMIN_OR_PROJECT_ADMIN && !user.is_admin && !user.is_project_admin)
			{
				btnet.Util.write_to_log("must be project admin, redirecting");
				Response.Redirect("default.aspx");
			}

			if (Util.get_setting("WindowsAuthentication","0") == "1")
			{
				auth_method = "windows";
			}
			else
			{
				auth_method = "plain";
			}
		}

		///////////////////////////////////////////////////////////////////////
		public static void create_session(HttpRequest Request, HttpResponse Response, int userid, string username, string NTLM)
		{

			// Generate a random session id
			// Don't use a regularly incrementing identity
			// column because that can be guessed.
			string guid = Guid.NewGuid().ToString();

			btnet.Util.write_to_log("guid=" + guid);
			
			string sql = @"insert into sessions (se_id, se_user) values('$gu', $us)";
			sql = sql.Replace("$gu", guid);
			sql = sql.Replace("$us", Convert.ToString(userid));

			btnet.DbUtil.execute_nonquery(sql);			

			HttpContext.Current.Session[guid] = userid;
			
			string sAppPath = Request.Url.AbsolutePath;
			sAppPath = sAppPath.Substring(0, sAppPath.LastIndexOf('/'));
			Util.write_to_log("AppPath:" + sAppPath);

			Response.Cookies["se_id"].Value = guid;
			Response.Cookies["se_id"].Path = sAppPath;
			Response.Cookies["user"]["name"] = username;
			Response.Cookies["user"]["NTLM"] = NTLM;
			Response.Cookies["user"].Path = sAppPath;
			DateTime dt = DateTime.Now;
			TimeSpan ts = new TimeSpan(365, 0, 0, 0);
			Response.Cookies["user"].Expires = dt.Add(ts);
		}

        ///////////////////////////////////////////////////////////////////////
        public void write_menu_item(HttpResponse Response,
            string this_link, string menu_item, string href)
        {
            Response.Write("<td class='menu_td'>");
            if (this_link == menu_item)
            {
                Response.Write("<a href=" + href + "><span class='selected_menu_item warn'  style='margin-left:3px;'>" + menu_item + "</span></a>");
            }
            else
            {
                Response.Write("<a href=" + href + "><span class='menu_item warn' style='margin-left:3px;'>" + menu_item + "</span></a>");
            }
            Response.Write("</td>");
        }


        ///////////////////////////////////////////////////////////////////////
        public void write_menu(HttpResponse Response, string this_link)
        {

            // topmost visible HTML
            string custom_header = (string)Util.context.Application["custom_header"];
            Response.Write(custom_header);

            Response.Write(@"
<span id=debug style='position:absolute;top:0;left:0;'></span>
<script>
function dbg(s)
{
	document.getElementById('debug').innerHTML += (s + '<br>')
}
function on_submit_search()
{
	el = document.getElementById('lucene_input')
	if (el.value == '')
	{
		alert('Enter the words you are search for.');
		el.focus()
		return false;
	}
	else
	{
		return true;
	}

}

</script>
<table border=0 width=100% cellpadding=0 cellspacing=0 class=menubar><tr>");

            // logo
            string logo = (string)Util.context.Application["custom_logo"];
            Response.Write(logo);

            Response.Write("<td width=20>&nbsp;</td>");
            write_menu_item(Response, this_link, Util.get_setting("PluralBugLabel", "bugs"), "bugs.aspx");
            
            if (user.can_search)
            {
            	write_menu_item(Response, this_link, "search", "search.aspx");
            }

            if (Util.get_setting("EnableWhatsNewPage", "0") == "1")
            {
				write_menu_item(Response, this_link, "news", "view_whatsnew.aspx");
			}

            if (!user.is_guest)
            {
                write_menu_item(Response, this_link, "queries", "queries.aspx");
            }

            if (user.is_admin || user.can_use_reports || user.can_edit_reports)
            {
                write_menu_item(Response, this_link, "reports", "reports.aspx");
            }

            if (Util.get_setting("CustomMenuLinkLabel", "") != "")
            {
                write_menu_item(Response, this_link,
                    Util.get_setting("CustomMenuLinkLabel", ""),
                    Util.get_setting("CustomMenuLinkUrl", ""));
            }

            if (user.is_admin)
            {
                write_menu_item(Response, this_link, "admin", "admin.aspx");
            }
            else if (user.is_project_admin)
            {
                write_menu_item(Response, this_link, "users", "users.aspx");
            }


            // go to

            Response.Write (goto_form);

            // search
            if (Util.get_setting("EnableLucene", "1") == "1" && user.can_search)
            {
                string query = (string) HttpContext.Current.Session["query"];
                if (query == null)
                {
                    query = "";
                }
                string search_form = @"

<td nowrap valign=middle>
    <form style='margin: 0px; padding: 0px;' action=search_text.aspx method=get onsubmit='return on_submit_search()'>
        <input class=menubtn type=submit value='search text'>
        <input class=menuinput  id=lucene_input size=24 type=text class=txt
        value='" + query.Replace("'","") + @"' name=query accesskey=s>
        <a href=lucene_syntax.html target=_blank style='font-size: 7pt;'>advanced</a>
    </form>
</td>";
                //context.Session["query"] = null;
                Response.Write(search_form);
			}

            Response.Write("<td nowrap valign=middle>");
			if (user.is_guest && Util.get_setting("AllowGuestWithoutLogin","0") == "1")
			{
				Response.Write("<span class=smallnote>using as<br>");
			}
			else
			{
				Response.Write("<span class=smallnote>logged in as<br>");
			}
           	Response.Write(user.username);
            Response.Write("</span></td>");

            if (auth_method == "plain")
            {
                if (user.is_guest && Util.get_setting("AllowGuestWithoutLogin","0") == "1")
                {
                	write_menu_item(Response, this_link, "login", "default.aspx");
				}
				else
				{
					write_menu_item(Response, this_link, "logoff", "logoff.aspx");
				}
            }
            
            // for guest account, suppress display of "edit_self
            if (!user.is_guest)
            {
                write_menu_item(Response, this_link, "settings", "edit_self.aspx");
            }


            Response.Write("<td valign=middle align=left'>");
            Response.Write("<a target=_blank href=about.html><span class='menu_item' style='margin-left:3px;'>about</span></a></td>");
            Response.Write("<td nowrap valign=middle>");
            Response.Write("<a target=_blank href=http://ifdefined.com/README.html><span class='menu_item' style='margin-left:3px;'>help</span></a></td>");

            Response.Write("</tr></table><br>");
        }
	} // end Security
}
