// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Data;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.Extensions;

namespace MyApp.Admin.Security.PermissionControl
{
    /// <summary>
    /// This is the code that calculates what feature permissions a user has
    /// </summary>
    public class CalcAllowedPermissions
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly RoleManager<CustomRole> _roleManager;

        public CalcAllowedPermissions(UserManager<UserProfile> userManager, RoleManager<CustomRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// This is called if the Permissions that a user needs calculating.
        /// It looks at what permissions the user has, and then filters out any permissions
        /// they aren't allowed because they haven't get access to the module that permission is linked to.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>a string containing the packed permissions</returns>
        public async Task<string> CalcPermissionsForUserAsync(string userId)
        {
            //This gets all the permissions, with a distinct to remove duplicates
            var userProfile = await _userManager.FindByIdAsync(userId);
            var userRolesName = await _userManager.GetRolesAsync(userProfile);

            var permissionsForUser = _roleManager.Roles.Where(x => userRolesName.Contains(x.Name))
                .Select(x => x.PermissionsInRole)
                .ToList()
                //Because the permissions are packed we have to put these parts of the query after the ToListAsync()
                .SelectMany(x => x).Distinct();

            return permissionsForUser.PackPermissionsIntoString();
        }

    }
}