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
using System.Security.Claims;

namespace btnet
{

	public class Security {
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

        public static void SignIn(HttpRequest request, string username)
        {
            SQLString sql = new SQLString(@"
select u.us_id, u.us_username, u.us_org, u.us_bugs_per_page, u.us_enable_bug_list_popups,
       org.*,
       isnull(u.us_forced_project, 0 ) us_forced_project,
       proj.pu_permission_level,
       isnull(proj.pu_admin, 0) pu_admin,
       u.us_admin
from users u
inner join orgs org 
    on u.us_org = org.og_id
left outer join project_user_xref proj
	on proj.pu_project = u.us_forced_project
	and proj.pu_user = u.us_id
where us_username = @us and u.us_active = 1");
            sql = sql.AddParameterWithValue("us", username);
            DataRow dr = btnet.DbUtil.get_datarow(sql);

            var bugsPerPage = string.IsNullOrEmpty(dr["us_bugs_per_page"] as string) ? 10 : (int) dr["us_bugs_per_page"];
            
            var claims = new List<Claim>
            {
                new Claim(BtnetClaimTypes.UserId, Convert.ToString(dr["us_id"])),
                new Claim(ClaimTypes.Name, Convert.ToString(dr["us_username"])),
                new Claim(BtnetClaimTypes.OrganizationId, Convert.ToString(dr["us_org"])),
                new Claim(BtnetClaimTypes.BugsPerPage, Convert.ToString(bugsPerPage)),
                new Claim(BtnetClaimTypes.EnablePopUps, Convert.ToString((int) dr["us_enable_bug_list_popups"] == 1)),
                new Claim(BtnetClaimTypes.CanOnlySeeOwnReportedBugs, Convert.ToString((int) dr["og_can_only_see_own_reported"] == 1)),
                new Claim(BtnetClaimTypes.CanUseReports, Convert.ToString((int) dr["og_can_use_reports"] == 1)),
                new Claim(BtnetClaimTypes.CanEditReports, Convert.ToString((int) dr["og_can_edit_reports"] == 1)),
                new Claim(BtnetClaimTypes.OtherOrgsPermissionLevel, Convert.ToString(dr["og_other_orgs_permission_level"])),
                new Claim(BtnetClaimTypes.CanOnlySeeOwnReportedBugs, Convert.ToString((int) dr["us_enable_bug_list_popups"] == 1)),
                new Claim(BtnetClaimTypes.CanSearch, Convert.ToString((int) dr["og_can_search"] == 1))

            };

            bool canAdd = true;
            int permssionLevel = dr["pu_permission_level"] == DBNull.Value
                ? Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"))
                : (int) dr["pu_permission_level"];
            // if user is forced to a specific project, and doesn't have
            // at least reporter permission on that project, than user
            // can't add bugs
            if ((int)dr["us_forced_project"] != 0)
            {
                if (permssionLevel == Security.PERMISSION_READONLY || permssionLevel  == Security.PERMISSION_NONE)
                {
                    canAdd = false;
                }
            }
            claims.Add(new Claim(BtnetClaimTypes.CanAddBugs, Convert.ToString(canAdd)));

            int tagsPermissionLevel;
            if (Util.get_setting("EnableTags", "0") == "1")
            {
                tagsPermissionLevel = (int)dr["og_tags_field_permission_level"];
            }
            else
            {
                tagsPermissionLevel = Security.PERMISSION_NONE;
            }

            claims.Add(new Claim(BtnetClaimTypes.TagsPermissionLevel, Convert.ToString(tagsPermissionLevel)));


            if ((int) dr["us_admin"] == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.Admin));
            }
            else
            {
                if ((int) dr["project_admin"] > 0)
                {
                    claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.ProjectAdmin));
                }
            }
            claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.User));
            

            var identity = new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);
            var owinContext = request.GetOwinContext();
            owinContext.Authentication.SignIn(identity);
        }

	    public static void SignOut(HttpRequest request)
	    {
            var owinContext = request.GetOwinContext();
	        owinContext.Authentication.SignOut();
	    }

		///////////////////////////////////////////////////////////////////////
		public static void create_session(HttpRequest Request, HttpResponse Response, int userid, string username, string NTLM)
		{

			// Generate a random session id
			// Don't use a regularly incrementing identity
			// column because that can be guessed.
			string guid = Guid.NewGuid().ToString();

			btnet.Util.write_to_log("guid=" + guid);
			
			var sql = new SQLString(@"insert into sessions (se_id, se_user) values(@gu, @us)");
			sql = sql.AddParameterWithValue("gu", guid);
			sql = sql.AddParameterWithValue("us", Convert.ToString(userid));

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

	} // end Security
}
