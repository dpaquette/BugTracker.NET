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