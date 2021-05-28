using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Domains;
using MyApp.Admin.Security.Public.Dtos;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.Services;
using MyApp.Common.Public.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Admin.Security.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<CustomRole> _roleManager;

        public RoleService(ILogger<RoleService> logger, RoleManager<CustomRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<RoleListDto>> ListAllRolesAsync()
        {
            return await _roleManager.Roles.AsQueryable()
                    .Select(r => new RoleListDto
                    {
                        RoleId = r.Id,
                        Name = r.Name,
                        RoleDescription = r.RoleDescription,
                        IsBuiltInRole = r.IsBuiltInRole
                    })
                    .Where(r => r.Name != Roles.SuperAdmin.ToString()) // Exclude SuperAdmin from front end management
                    .OrderBy(r => r.Name)
                    .ToListAsync();
        }

        public async Task<RoleDetailDto> GetRoleByIdForDetailAsync(string roleId)
        {
            return await _roleManager.Roles.AsQueryable()
                    .Select(r => new RoleDetailDto
                    {
                        RoleId = r.Id,
                        Name = r.Name,
                        RoleDescription = r.RoleDescription,
                        IsBuiltInRole = r.IsBuiltInRole,
                        PermissionOptions = RoleAddEditDto.GeneratePermissionOptions(r.PermissionsInRole)
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.RoleId == roleId);
        }

        public async Task<RoleAddEditDto> GetRoleByIdForEditAsync(string roleId)
        {
            return await _roleManager.Roles.AsQueryable()
                    .Select(r => new RoleAddEditDto
                    {
                        RoleId = r.Id,
                        Name = r.Name,
                        RoleDescription = r.RoleDescription,
                        PermissionOptions = RoleAddEditDto.GeneratePermissionOptions(r.PermissionsInRole)
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.RoleId == roleId);
        }

        public async Task<IdentityResult> UpdateRoleAndSaveAsync(RoleAddEditDto dto)
        {
            IdentityResult result = null;

            try
            {
                var roleToUpdate = await _roleManager.FindByIdAsync(dto.RoleId);

                if (roleToUpdate == null)
                {
                    var errMsg = $"Record does not exist or has been deleted with RoleId={dto.RoleId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                roleToUpdate.Name = dto.Name;
                roleToUpdate.RoleDescription = dto.RoleDescription;
                roleToUpdate.UpdatePermissionsInRole(dto.PermissionsInput);

                // Comment out this line to enable updating only the fields with changed values
                //_context.Departments.Update(courseInDb);

                result = await _roleManager.UpdateAsync(roleToUpdate);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update Role: {dto}");
                throw ex;
            }

            return result;
        }

        //public async Task<IdentityResult> CreateNewRoleAndSaveAsync(NewRoleDto newRoleDto)
        //{
        //    if (newRoleDto.RoleName != null)
        //    {
        //        return await _roleManager.CreateAsync(new CustomRole(newRoleDto.RoleName.Trim()));
        //    }
        //    return default;
        //}

        public async Task<RoleAddEditDto> GetRoleByIdForCreateCopyAsync(string copyFromRoleId)
        {
            return await _roleManager.Roles.AsQueryable()
                    .Select(r => new RoleAddEditDto
                    {
                        RoleId = r.Id,
                        Name = $"Copy of {r.Name}",
                        RoleDescription = $"Copy of {r.RoleDescription}",
                        PermissionOptions = RoleAddEditDto.GeneratePermissionOptions(r.PermissionsInRole)
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.RoleId == copyFromRoleId);
        }

        public async Task<IdentityResult> CreateRoleAndSaveAsync(RoleAddEditDto dto)
        {
            IdentityResult result;
            try
            {
                if (await _roleManager.RoleExistsAsync(dto.Name))
                {
                    var errMsg = $"Record already exists with Role Name={dto.Name}. Please use a different value.";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                var roleToCreate = new CustomRole(dto.Name)
                {
                    RoleDescription = dto.RoleDescription,
                    IsBuiltInRole = false
                };
                roleToCreate.UpdatePermissionsInRole(dto.PermissionsInput);

                // Comment out this line to enable updating only the fields with changed values
                //_context.Departments.Update(courseInDb);

                result = await _roleManager.CreateAsync(roleToCreate);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create Role: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<IdentityResult> DeleteRoleAndSaveAsync(string roleId)
        {
            IdentityResult result;
            try
            {
                var roleToDelete = await _roleManager.FindByIdAsync(roleId);

                if (roleToDelete == null)
                {
                    var errMsg = $"Record does not exist with RoleId={roleId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }
                else if (roleToDelete.IsBuiltInRole)
                {
                    var errMsg = $"Not allowed to delete Build-in Role with RoleId={roleId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                result = await _roleManager.DeleteAsync(roleToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete RoleId: {roleId}");
                throw ex;
            }

            return result;
        }
    }
}
