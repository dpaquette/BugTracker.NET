using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using System.Web;

namespace btnet.Security
{

	public class Security {
  
        public static void SignIn(HttpRequest request, string username)
        {
            var identity = GetIdentity(username);
            var owinContext = request.GetOwinContext();
            owinContext.Authentication.SignIn(identity);
        }

	    public static ClaimsIdentity GetIdentity(string username)
	    {
            SQLString sql = new SQLString(@"
select u.us_id, u.us_username, u.us_org, u.us_bugs_per_page, u.us_enable_bug_list_popups,
       u.us_use_fckeditor, u.us_forced_project, u.us_email,
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

            var bugsPerPage = string.IsNullOrEmpty(dr["us_bugs_per_page"] as string) ? 10 : (int)dr["us_bugs_per_page"];

            var claims = new List<Claim>
            {
                new Claim(BtnetClaimTypes.UserId, Convert.ToString(dr["us_id"])),
                new Claim(ClaimTypes.Name, Convert.ToString(dr["us_username"])),
                new Claim(ClaimTypes.Email, Convert.ToString(dr["us_email"])),
                new Claim(BtnetClaimTypes.OrganizationId, Convert.ToString(dr["us_org"])),
                new Claim(BtnetClaimTypes.BugsPerPage, Convert.ToString(bugsPerPage)),
                new Claim(BtnetClaimTypes.EnablePopUps, Convert.ToString((int) dr["us_enable_bug_list_popups"] == 1)),
                new Claim(BtnetClaimTypes.CanOnlySeeOwnReportedBugs, Convert.ToString((int) dr["og_can_only_see_own_reported"] == 1)),
                new Claim(BtnetClaimTypes.CanUseReports, Convert.ToString((int) dr["og_can_use_reports"] == 1)),
                new Claim(BtnetClaimTypes.CanEditReports, Convert.ToString((int) dr["og_can_edit_reports"] == 1)),
                new Claim(BtnetClaimTypes.CanEditAndDeleteBugs, Convert.ToString((int) dr["og_can_edit_and_delete_posts"] == 1)), 
                new Claim(BtnetClaimTypes.CanDeleteBugs, Convert.ToString((int) dr["og_can_delete_bug"] == 1)), 
                new Claim(BtnetClaimTypes.CanMergeBugs, Convert.ToString((int) dr["og_can_merge_bugs"] == 1)), 
                new Claim(BtnetClaimTypes.CanMassEditBugs, Convert.ToString((int) dr["og_can_mass_edit_bugs"] == 1)), 
                new Claim(BtnetClaimTypes.CanAssignToInternalUsers, Convert.ToString((int) dr["og_can_assign_to_internal_users"] == 1)), 
                
                new Claim(BtnetClaimTypes.CanEditAndDeletePosts, Convert.ToString((int) dr["og_can_edit_and_delete_posts"] == 1)), 
                
                new Claim(BtnetClaimTypes.CanEditTasks, Convert.ToString((int) dr["og_can_edit_tasks"] == 1)), 
                new Claim(BtnetClaimTypes.CanViewTasks, Convert.ToString((int) dr["og_can_view_tasks"] == 1)), 
                

                new Claim(BtnetClaimTypes.OtherOrgsPermissionLevel, Convert.ToString(dr["og_other_orgs_permission_level"])),
                new Claim(BtnetClaimTypes.CategoryFieldPermissionLevel, Convert.ToString(dr["og_category_field_permission_level"])),
                new Claim(BtnetClaimTypes.PriorityFieldPermissionLevel, Convert.ToString(dr["og_priority_field_permission_level"])),
                new Claim(BtnetClaimTypes.ProjectFieldPermissionLevel, Convert.ToString(dr["og_project_field_permission_level"])),
                new Claim(BtnetClaimTypes.StatusFieldPermissionLevel, Convert.ToString(dr["og_status_field_permission_level"])),
                new Claim(BtnetClaimTypes.AssignedToFieldPermissionLevel, Convert.ToString(dr["og_assigned_to_field_permission_level"])),
                new Claim(BtnetClaimTypes.OrgFieldPermissionLevel, Convert.ToString(dr["og_org_field_permission_level"])),
                new Claim(BtnetClaimTypes.UdfFieldPermissionLevel, Convert.ToString(dr["og_udf_field_permission_level"])),
                
                new Claim(BtnetClaimTypes.CanOnlySeeOwnReportedBugs, Convert.ToString((int) dr["us_enable_bug_list_popups"] == 1)),
                new Claim(BtnetClaimTypes.CanSearch, Convert.ToString((int) dr["og_can_search"] == 1)),
                new Claim(BtnetClaimTypes.IsExternalUser, Convert.ToString((int) dr["og_external_user"] == 1)),
                new Claim(BtnetClaimTypes.UseFCKEditor, Convert.ToString((int) dr["us_use_fckeditor"] == 1))
                
            };

            bool canAdd = true;
            int permssionLevel = dr["pu_permission_level"] == DBNull.Value
                ? Convert.ToInt32(Util.get_setting("DefaultPermissionLevel", "2"))
                : (int)dr["pu_permission_level"];
            // if user is forced to a specific project, and doesn't have
            // at least reporter permission on that project, than user
            // can't add bugs
            int forcedProjectId = dr["us_forced_project"] == DBNull.Value ? 0 : (int)dr["us_forced_project"];
	        if (forcedProjectId != 0)
            {
                if (permssionLevel == PermissionLevel.ReadOnly || permssionLevel == PermissionLevel.None)
                {
                    canAdd = false;
                }
            }
            claims.Add(new Claim(BtnetClaimTypes.CanAddBugs, Convert.ToString(canAdd)));
	        claims.Add(new Claim(BtnetClaimTypes.ForcedProjectId, Convert.ToString(forcedProjectId)));
                
            int tagsPermissionLevel;
            if (Util.get_setting("EnableTags", "0") == "1")
            {
                tagsPermissionLevel = (int)dr["og_tags_field_permission_level"];
            }
            else
            {
                tagsPermissionLevel = PermissionLevel.None;
            }

            claims.Add(new Claim(BtnetClaimTypes.TagsFieldPermissionLevel, Convert.ToString(tagsPermissionLevel)));


            if ((int)dr["us_admin"] == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.Admin));
            }
            else
            {
                if ((int)dr["project_admin"] > 0)
                {
                    claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.ProjectAdmin));
                }
            }
            claims.Add(new Claim(ClaimTypes.Role, BtnetRoles.User));


            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimTypes.Name, ClaimTypes.Role);
	    }

	    public static void SignOut(HttpRequest request)
	    {
            var owinContext = request.GetOwinContext();
	        owinContext.Authentication.SignOut();
	    }
	} 
}
