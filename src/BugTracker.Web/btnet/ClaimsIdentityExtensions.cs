using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace btnet
{
    public static class ClaimsIdentityExtensions
    {
        public static int GetUserId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.UserId));
        }

        public static string GetEmail(this IIdentity identity)
        {
            return GetClaimsValue(identity, ClaimTypes.Email);
        }
        
        public static int GetOrganizationId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.OrganizationId));
        }
        public static int GetForcedProjectId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.ForcedProjectId));
        }

        public static int GetBugsPerPage(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.BugsPerPage));
        }

        public static bool GetCanOnlySeeOwnReportedBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanOnlySeeOwnReportedBugs));
        }

        public static bool GetCanAssignToInternalUsers(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanAssignToInternalUsers));
        }

        public static bool GetEnablePopups(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.EnablePopUps));
        }

        public static bool GetUseFCKEditor(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.UseFCKEditor));
        }

        public static bool GetCanEditTasks(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanEditTasks));
        }

        public static bool GetCanViewTasks(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanViewTasks));
        }

        public static bool GetCanAddBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanAddBugs));
        }

        public static bool GetCanUseReports(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanUseReports));
        }

        public static bool GetCanEditReports(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanEditReports));
        }

        public static bool GetCanMergeBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanMergeBugs));
        }

        public static bool GetCanMassEditBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanMassEditBugs));
        }

        public static int GetOtherOrgsPermissionLevels(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.OtherOrgsPermissionLevel));
        }

        public static int GetCategoryFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.CategoryFieldPermissionLevel));
        }

        public static int GetTagsFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.TagsFieldPermissionLevel));
        }

        public static int GetProjectFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.ProjectFieldPermissionLevel));
        }

        public static int GetStatusFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.StatusFieldPermissionLevel));
        }

        public static int GetPriorityFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.PriorityFieldPermissionLevel));
        }

        public static int GetAssignedToFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.AssignedToFieldPermissionLevel));
        }

        public static int GetOrgFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.OrgFieldPermissionLevel));
        }

        public static int GetUdfFieldPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.UdfFieldPermissionLevel));
        }
        
        public static bool GetCanSearch(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanSearch));
        }

        public static bool GetCanEditAndDeleteBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanEditAndDeleteBugs));
        }

        public static bool GetCanEditAndDeletePosts(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanEditAndDeletePosts));
        }

        public static bool GetCanDeleteBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanDeleteBugs));
        }

        public static bool GetIsExternalUser(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.IsExternalUser));
        }

        public static bool IsInRole(this IIdentity identity, string roleName)
        {
            if (identity is ClaimsIdentity)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity) identity;
                return claimsIdentity.HasClaim(ClaimTypes.Role, roleName);
            }
            else
            {
                throw new SecurityException("Identity is not a valid Claims Identity");
            }
        }

        private static string GetClaimsValue(IIdentity identity, string claimType)
        {
            if (identity is ClaimsIdentity)
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity) identity;

                Claim userIdClaim = claimsIdentity.FindFirst(claimType);
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
                else
                {
                    throw new SecurityException(string.Format("Identity is missing value for claim type {0}", claimType));
                }
            }
            else
            {
                throw new SecurityException("Identity is not a valid Claims Identity");
            }
        }
    }
}