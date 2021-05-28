using Microsoft.AspNetCore.Mvc;
using MyApp.Admin.Security.Public.Constants;
using MyApp.Admin.Security.Public.Dtos;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.PermissionControl.Policy;
using MyApp.Admin.Security.Public.Services;
using MyApp.Common.Public.Exceptions;
using MyApp.WebMvc03.Utils;
using System;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.Admin.Security
{
    public class RolesController : Controller
    {
        private static readonly string _viewFolder = "/Views/Admin/Security/Roles/";

        [HasPermission(Permissions.RoleView)]
        public async Task<IActionResult> Index([FromServices] IRoleService service)
        {
            var roles = await service.ListAllRolesAsync();
            return View($"{_viewFolder}Index.cshtml", roles);
        }

        [HasPermission(Permissions.RoleView)]
        public async Task<IActionResult> Details(string id, [FromServices] IRoleService service)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var roleDto = await service.GetRoleByIdForDetailAsync(id);
            if (roleDto == null)
            {
                TempData["Message"] = $"Role with Id = {id} not found";
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View($"{_viewFolder}Details.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleEdit)]
        public async Task<IActionResult> Edit(string id, [FromServices] IRoleService service)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var roleDto = await service.GetRoleByIdForEditAsync(id);
            if (roleDto == null)
            {
                TempData["Message"] = $"Role with Id = {id} not found";
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Index));
            }
            
            return View($"{_viewFolder}Edit.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleEdit)]
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(
            string id, RoleAddEditDto roleDto, [FromServices] IRoleService service, 
            [FromServices] ICacheControlService cacheService)
        {
            if (string.IsNullOrWhiteSpace(id)
                || !string.Equals(id, roleDto.RoleId))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await cacheService.UpdateLastRefreshTimeAsync(CacheKeys.USER_PERMISSIONS, false);

                    // call this last as this line will call DbContext.SaveChangesAsync() implicitly
                    await service.UpdateRoleAndSaveAsync(roleDto);

                    TempData["Message"] = Constants.SUCCESS_MESSAGE;
                    return RedirectToAction(nameof(Index));
                }
                catch (GeneralException ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
                }
            }

            // re-populate options for display in case of exception
            roleDto.PermissionOptions = RoleAddEditDto.GeneratePermissionOptions(roleDto.PermissionsInput);

            return View($"{_viewFolder}Edit.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleAdd)]
        public async Task<IActionResult> CreateCopy(string id, [FromServices] IRoleService service)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var roleDto = await service.GetRoleByIdForCreateCopyAsync(id);
            if (roleDto == null)
            {
                TempData["Message"] = $"Source Role with Id = {id} not found";
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View($"{_viewFolder}CreateCopy.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleAdd)]
        [HttpPost, ActionName("CreateCopy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostCreateCopy(
            string id, RoleAddEditDto roleDto, [FromServices] IRoleService service,
            [FromServices] ICacheControlService cacheService)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await cacheService.UpdateLastRefreshTimeAsync(CacheKeys.USER_PERMISSIONS, false);

                    // call this last as this line will call DbContext.SaveChangesAsync() implicitly
                    await service.CreateRoleAndSaveAsync(roleDto);

                    TempData["Message"] = Constants.SUCCESS_MESSAGE;
                    return RedirectToAction(nameof(Index));
                }
                catch (GeneralException ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
                }
            }

            // re-populate options for display in case of exception
            roleDto.PermissionOptions = RoleAddEditDto.GeneratePermissionOptions(roleDto.PermissionsInput);

            return View($"{_viewFolder}CreateCopy.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleDelete)]
        public async Task<IActionResult> Delete(string id, [FromServices] IRoleService service)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var hasPreviousError = TempData["HasError"] != null && Convert.ToBoolean(TempData["HasError"]);
            var message = Convert.ToString(TempData["Message"]);

            var roleDto = await service.GetRoleByIdForDetailAsync(id);
            if (roleDto == null)
            {
                // deleted by another user in previous delete request
                if (hasPreviousError)
                {
                    TempData["Message"] = message;
                    TempData["HasError"] = hasPreviousError;
                    return RedirectToAction(nameof(Index));
                }

                TempData["Message"] = $"Role with Id = {id} not found";
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Index));
            }
            else if (roleDto.IsBuiltInRole)
            {
                TempData["Message"] = $"Built-in Role with Id = {id} cannot be deleted.";
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Index));
            }

            return View($"{_viewFolder}Delete.cshtml", roleDto);
        }

        [HasPermission(Permissions.RoleDelete)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            string id, [FromServices] IRoleService service,
            [FromServices] ICacheControlService cacheService)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            try
            {
                await cacheService.UpdateLastRefreshTimeAsync(CacheKeys.USER_PERMISSIONS, false);

                // call this last as this line will call DbContext.SaveChangesAsync() implicitly
                await service.DeleteRoleAndSaveAsync(id);

                TempData["Message"] = Constants.SUCCESS_MESSAGE;
                return RedirectToAction(nameof(Index));
            }
            catch (GeneralException ex)
            {
                TempData["Message"] = ex.Message;
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Delete), new { id });
            }
            catch (Exception ex)
            {
                TempData["Message"] = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}
