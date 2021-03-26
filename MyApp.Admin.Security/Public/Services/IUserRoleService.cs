using Microsoft.AspNetCore.Identity;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services
{
    public interface IUserRoleService
    {
        Task<IEnumerable<UserRoleData>> ListAllUsersWithRolesAsync();

        Task<UserProfile> GetUserProfileAsync(string UserId);

        Task<IEnumerable<UserRoleAssignedData>> ListUserRolesForManageViewAsync(UserProfile User);

        Task<IEnumerable<string>> ListRolesAssignedToUserAsync(UserProfile User);

        Task<IdentityResult> RemoveRolesFromUserAsync(UserProfile User, IEnumerable<string> Roles);

        Task<IdentityResult> AddRolesToUserAsync(UserProfile User, IEnumerable<string> Roles);

    }
}
