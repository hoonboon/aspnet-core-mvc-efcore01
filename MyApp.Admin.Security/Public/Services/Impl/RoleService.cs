using Microsoft.AspNetCore.Identity;
using MyApp.Admin.Security.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateNewRoleAndSaveAsync(NewRoleDto newRoleDto)
        {
            if (newRoleDto.RoleName != null)
            {
                return await _roleManager.CreateAsync(new IdentityRole(newRoleDto.RoleName.Trim()));
            }
            return default;
        }

        IQueryable<IdentityRole> IRoleService.ListAllRoles()
        {
            return _roleManager.Roles.AsQueryable();
        }
    }
}
