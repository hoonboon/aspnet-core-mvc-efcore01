using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Admin.Security.Public.Dtos;
using MyApp.Admin.Security.Public.Services;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.Admin.Security
{
    public class RolesController : Controller
    {
        private static readonly string _viewFolder = "/Views/Admin/Security/Roles/";

        public async Task<IActionResult> Index([FromServices] IRoleService service)
        {
            var roles = await service.ListAllRoles().ToListAsync();
            return View($"{_viewFolder}Index.cshtml", roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(NewRoleDto newRoleDto, [FromServices] IRoleService service)
        {
            await service.CreateNewRoleAndSaveAsync(newRoleDto);
            return RedirectToAction("Index");
        }
    }
}
