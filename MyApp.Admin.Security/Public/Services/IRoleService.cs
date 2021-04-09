using Microsoft.AspNetCore.Identity;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Dtos;
using MyApp.Admin.Security.Public.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Public.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleListDto>> ListAllRolesAsync();

        Task<RoleDetailDto> GetRoleByIdForDetailAsync(string roleId);

        Task<RoleAddEditDto> GetRoleByIdForEditAsync(string roleId);

        Task<IdentityResult> UpdateRoleAndSaveAsync(RoleAddEditDto roleDto);

        Task<RoleAddEditDto> GetRoleByIdForCreateCopyAsync(string copyFromRoleId);
        
        Task<IdentityResult> CreateRoleAndSaveAsync(RoleAddEditDto roleDto);

        Task<IdentityResult> DeleteRoleAndSaveAsync(string roleId);
    }
}
