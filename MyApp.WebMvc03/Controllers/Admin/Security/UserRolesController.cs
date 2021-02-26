using MyApp.WebMvc03.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.Admin.Security
{
    public class UserRolesController : Controller
    {
        private readonly UserManager<UserProfile> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private static readonly string _viewFolder = "/Views/Admin/Security/UserRoles/";
        
        public UserRolesController(
            UserManager<UserProfile> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
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
            return View($"{_viewFolder}Index.cshtml", userRolesList);
        }

        private async Task<List<string>> GetUserRoles(UserProfile user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Manage(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<UserRoleAssignedData>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesAssigned = new UserRoleAssignedData
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesAssigned.IsAssigned = true;
                }
                else
                {
                    userRolesAssigned.IsAssigned = false;
                }
                model.Add(userRolesAssigned);
            }
            return View($"{_viewFolder}Manage.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(List<UserRoleAssignedData> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(
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
