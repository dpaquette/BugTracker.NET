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

        public static int GetOrganizationId(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.OrganizationId));
        }

        public static int GetBugsPerPage(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.BugsPerPage));
        }

        public static bool GetCanOnlySeeOwnReportedBugs(this IIdentity identity)
        {
            return Convert.ToBoolean(GetClaimsValue(identity, BtnetClaimTypes.CanOnlySeeOwnReportedBugs));
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

        public static int GetOtherOrgsPermissionLevels(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.OtherOrgsPermissionLevel));
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

        public static int GetTagsPermissionLevel(this IIdentity identity)
        {
            return Convert.ToInt32(GetClaimsValue(identity, BtnetClaimTypes.TagsPermissionLevel));
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