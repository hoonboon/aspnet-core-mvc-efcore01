﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Admin.Security.Public.Constants;
using MyApp.Admin.Security.Public.Dtos;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.PermissionControl.Policy;
using MyApp.Admin.Security.Public.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.Admin.Security
{
    public class UserRolesController : Controller
    {
        private static readonly string _viewFolder = "/Views/Admin/Security/UserRoles/";

        [HasPermission(Permissions.UserRoleView)]
        public async Task<IActionResult> Index([FromServices] IUserRoleService service)
        {
            var userRolesList = await service.ListAllUsersWithRolesAsync();
            return View($"{_viewFolder}Index.cshtml", userRolesList);
        }

        [HasPermission(Permissions.UserRoleManage)]
        public async Task<IActionResult> Manage(string userId, [FromServices] IUserRoleService service)
        {
            ViewBag.userId = userId;
            var user = await service.GetUserProfileAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = await service.ListUserRolesForManageViewAsync(user);
            return View($"{_viewFolder}Manage.cshtml", model);
        }

        [HasPermission(Permissions.UserRoleManage)]
        [HttpPost]
        public async Task<IActionResult> Manage(
            List<UserRoleAssignedData> model, string userId, 
            [FromServices] IUserRoleService service,
            [FromServices] ICacheControlService cacheService)
        {
            var user = await service.GetUserProfileAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            
            await cacheService.UpdateLastRefreshTimeAsync(CacheKeys.USER_PERMISSIONS, false);

            var roles = await service.ListRolesAssignedToUserAsync(user);
            var result = await service.RemoveRolesFromUserAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await service.AddRolesToUserAsync(
                user, 
                model.Where(x => x.IsAssigned)
                    .Select(y => y.RoleName)
            );
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Index");
        }
    }
}
