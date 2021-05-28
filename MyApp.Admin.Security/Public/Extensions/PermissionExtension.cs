// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;
using MyApp.Admin.Security.PermissionControl.Cookie;
using MyApp.Admin.Security.PermissionControl.Cookie.Impl;
using MyApp.Admin.Security.Public.Constants;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.PermissionControl.Cookie;
using MyApp.Admin.Security.Public.PermissionControl.Cookie.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MyApp.Admin.Security.Public.Extensions
{
    public static class PermissionExtension
    {
        public static string PackPermissionsIntoString(this IEnumerable<Permissions> permissions)
        {
            return permissions.Aggregate("", (s, permission) => s + (char)permission);
        }

        public static IEnumerable<Permissions> UnpackPermissionsFromString(this string packedPermissions)
        {
            if (packedPermissions == null)
                throw new ArgumentNullException(nameof(packedPermissions));
            foreach (var character in packedPermissions)
            {
                yield return ((Permissions)character);
            }
        }

        public static Permissions? FindPermissionViaName(this string permissionName)
        {
            return 
                Enum.TryParse(permissionName, out Permissions permission) 
                ? (Permissions?)permission : null;
        }

        /// <summary>
        /// This is the main checker of whether a user permissions allows them to access something with the given permission
        /// </summary>
        /// <param name="usersPermissions"></param>
        /// <param name="permissionToCheck"></param>
        /// <returns></returns>
        public static bool UserHasThisPermission(this Permissions[] usersPermissions, Permissions permissionToCheck)
        {
            return usersPermissions.Contains(permissionToCheck) || usersPermissions.Contains(Permissions.AccessAll);
        }

        /// <summary>
        /// This is used by the policy provider to check the permission name string
        /// </summary>
        /// <param name="packedPermissions"></param>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public static bool ThisPermissionIsAllowed(this string packedPermissions, string permissionName)
        {
            var usersPermissions = packedPermissions.UnpackPermissionsFromString().ToArray();

            if (!Enum.TryParse(permissionName, true, out Permissions permissionToCheck))
                throw new InvalidEnumArgumentException($"{permissionName} could not be converted to a {nameof(Permissions)}.");

            return usersPermissions.UserHasThisPermission(permissionToCheck);
        }

        public static string GetUserIdFromClaims(this IEnumerable<Claim> claims)
        {
            return claims?.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static void ConfigureCookiesForExtraAuth(this IServiceCollection services)
        {
            // Event - Permissions and DataKey set up, provides User Impersonation + possible "RefreshClaims"
            services.AddDataProtection();   //DataProtection is needed to encrypt the data in the Impersonation cookie
            IAuthCookieValidate validateAsyncVersion = new AuthCookieValidateEverything();
            //We need two events for impersonation, so we do this here
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnValidatePrincipal = validateAsyncVersion.ValidateAsync;
                //This ensures the impersonation cookie is deleted when a user signs out
                options.Events.OnSigningOut = new AuthCookieSigningOut().SigningOutAsync;
            });

            //IAuthChanges is used to detect changes in the ExtraAuthClasses so we can update the user's permission claims
            //services.AddSingleton<IAuthChanges, AuthChanges>();

        }

        /// <summary>
        /// This returns true if the current user has the permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static bool HasPermission(this ClaimsPrincipal user, Permissions permission)
        {
            var permissionClaim =
                user?.Claims.SingleOrDefault(x => x.Type == PermissionConstants.PackedPermissionClaimType);
            return permissionClaim?.Value.UnpackPermissionsFromString().ToArray().UserHasThisPermission(permission) == true;
        }
    }
}
