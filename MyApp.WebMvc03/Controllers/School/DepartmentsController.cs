using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.PermissionControl.Policy;
using MyApp.Common.Public.Exceptions;
using MyApp.School.Public.Dtos;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Utils;
using System;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.School
{
    public class DepartmentsController : Controller
    {
        private readonly ILogger<DepartmentsController> _logger;

        private static readonly string _viewFolder = "/Views/School/Departments/";

        public DepartmentsController(ILogger<DepartmentsController> logger)
        {
            _logger = logger;
        }

        [HasPermission(Permissions.DepartmentView)]
        // GET: Departments
        public async Task<IActionResult> Index(
            [FromServices] IDepartmentService service)
        {
            return View($"{_viewFolder}Index.cshtml", await service.ListAllDepartmentsAsync());
        }

        [HasPermission(Permissions.DepartmentView)]
        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id, [FromServices] IDepartmentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var department = await service.GetDepartmentByIdForDetailAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Details.cshtml", department);
        }

        private async Task PopulateInstructorsDropDownListAsync(IDepartmentService service, object selectedInstructor = null)
        {
            ViewBag.InstructorOptions = new SelectList(
                await service.ListAllInstructorOptionsAsync(),
                "InstructorId", "InstructorName",
                selectedInstructor);
        }

        [HasPermission(Permissions.DepartmentAdd)]
        // GET: Departments/Create
        public async Task<IActionResult> Create([FromServices] IDepartmentService service)
        {
            await PopulateInstructorsDropDownListAsync(service);
            return View($"{_viewFolder}Create.cshtml");
        }

        [HasPermission(Permissions.DepartmentAdd)]
        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DepartmentAddEditDto department, [FromServices] IDepartmentService service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.CreateDepartmentAndSaveAsync(department);
                    TempData["Message"] = Constants.SUCCESS_MESSAGE;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = ex.Message;
                }
            }
            await PopulateInstructorsDropDownListAsync(service, department.InstructorId);
            return View($"{_viewFolder}Create.cshtml", department);
        }

        [HasPermission(Permissions.DepartmentEdit)]
        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id, [FromServices] IDepartmentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var department = await service.GetDepartmentByIdForEditAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            await PopulateInstructorsDropDownListAsync(service, department.InstructorId);
            return View($"{_viewFolder}Edit.cshtml", department);
        }

        [HasPermission(Permissions.DepartmentEdit)]
        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int? id, byte[] rowVersion, DepartmentAddEditDto department, [FromServices] IDepartmentService service)
        {
            if (!id.HasValue 
                || id.Value != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateDepartmentAndSaveAsync(department, rowVersion);
                    TempData["Message"] = Constants.SUCCESS_MESSAGE;
                    return RedirectToAction(nameof(Index));
                }
                catch (GeneralException ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = ex.Message;
                    ModelState.Remove("RowVersion");
                }
                catch (Exception ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
                    ModelState.Remove("RowVersion");
                }
            }

            await PopulateInstructorsDropDownListAsync(service, department.InstructorId);
            return View($"{_viewFolder}Edit.cshtml", department);
        }

        [HasPermission(Permissions.DepartmentDelete)]
        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(
            int? id, [FromServices] IDepartmentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var hasPreviousError = TempData["HasError"] != null && Convert.ToBoolean(TempData["HasError"]);
            var message = Convert.ToString(TempData["Message"]);

            var department = await service.GetDepartmentByIdForDetailAsync(id.Value);
            if (department == null)
            {
                // deleted by another user in previous delete request
                if (hasPreviousError)
                {
                    TempData["Message"] = message;
                    TempData["HasError"] = hasPreviousError;
                    return RedirectToAction(nameof(Index));
                }
                return NotFound(); // new delete request
            }

            return View($"{_viewFolder}Delete.cshtml", department);
        }

        [HasPermission(Permissions.DepartmentDelete)]
        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int? id, byte[] rowVersion, [FromServices] IDepartmentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                await service.DeleteDepartmentAndSaveAsync(id.Value, rowVersion);
                TempData["Message"] = Constants.SUCCESS_MESSAGE;
                return RedirectToAction(nameof(Index));
            }
            catch (GeneralException ex)
            {
                TempData["Message"] = ex.Message;
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Delete), new { id = id.Value });
            }
            catch (Exception ex)
            {
                TempData["Message"] = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
                TempData["HasError"] = true;
                return RedirectToAction(nameof(Delete), new { id = id.Value });
            }
        }

    }
}
