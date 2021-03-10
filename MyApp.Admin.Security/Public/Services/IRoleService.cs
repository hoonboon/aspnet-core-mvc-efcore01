using Microsoft.AspNetCore.Identity;
using MyApp.Admin.Security.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services
{
    public interface IRoleService
    {
        IQueryable<IdentityRole> ListAllRoles();

        Task<IdentityResult> CreateNewRoleAndSaveAsync(NewRoleDto newRoleDto);
    }
}
