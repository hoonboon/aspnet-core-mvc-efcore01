using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services.Impl
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRoleService(UserManager<UserProfile> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        private async Task<List<string>> GetUserRoles(UserProfile user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public async Task<IEnumerable<UserRoleData>> ListAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesList = new List<UserRoleData>();

            foreach (UserProfile user in users)
            {
                var tempViewModel = new UserRoleData();
                tempViewModel.UserId = user.Id;
                tempViewModel.Email = user.Email;
                tempViewModel.FirstName = user.FirstName;
                tempViewModel.LastName = user.LastName;
                tempViewModel.Roles = await GetUserRoles(user);
                userRolesList.Add(tempViewModel);
            }

            return userRolesList;
        }

        public async Task<UserProfile> GetUserProfileAsync(string UserId)
        {
            return await _userManager.FindByIdAsync(UserId);
        }

        public async Task<IEnumerable<UserRoleAssignedData>> ListUserRolesForManageViewAsync(UserProfile User)
        {
            var model = new List<UserRoleAssignedData>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesAssigned = new UserRoleAssignedData
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(User, role.Name))
                {
                    userRolesAssigned.IsAssigned = true;
                }
                else
                {
                    userRolesAssigned.IsAssigned = false;
                }
                model.Add(userRolesAssigned);
            }

            return model;
        }

        public async Task<IEnumerable<string>> ListRolesAssignedToUserAsync(UserProfile User)
        {
            return await _userManager.GetRolesAsync(User);
        }

        public async Task<IdentityResult> RemoveRolesFromUserAsync(UserProfile User, IEnumerable<string> Roles)
        {
            return await _userManager.RemoveFromRolesAsync(User, Roles);
        }

        public async Task<IdentityResult> AddRolesToUserAsync(UserProfile User, IEnumerable<string> Roles)
        {
            return await _userManager.AddToRolesAsync(User, Roles);
        }
    }
}
