using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MyApp.School.Public.Dtos;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Utils;
using System;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.School
{
    [Authorize(Roles = "Manager")]
    public class CoursesController : Controller
    {
        private readonly ILogger<CoursesController> _logger;

        private static readonly string _viewFolder = "/Views/School/Courses/";

        public CoursesController(ILogger<CoursesController> logger)
        {
            _logger = logger;
        }

        // GET: Courses
        public async Task<IActionResult> Index([FromServices] ICourseService service)
        {
            return View($"{_viewFolder}Index.cshtml", await service.ListAllCoursesAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id, [FromServices] ICourseService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var course = await service.GetCourseByIdForDetailAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Details.cshtml", course);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create([FromServices] ICourseService service)
        {
            await PopulateDepartmentsDropDownListAsync(service);
            return View($"{_viewFolder}Create.cshtml");
        }

        private async Task PopulateDepartmentsDropDownListAsync(ICourseService service, object selectedDepartment = null)
        {
            ViewBag.DepartmentOptions = new SelectList(
                await service.ListAllDepartmentOptionsAsync(), 
                "DepartmentId", "DepartmentName", 
                selectedDepartment);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CourseAddEditDto courseDto, [FromServices] ICourseService service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.CreateCourseAndSaveAsync(courseDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", Constants.ERROR_MESSAGE_SAVE);
                }
            }
            await PopulateDepartmentsDropDownListAsync(service);
            return View($"{_viewFolder}Create.cshtml", courseDto);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id, [FromServices] ICourseService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var course = await service.GetCourseByIdForEditAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }
            await PopulateDepartmentsDropDownListAsync(service, course.DepartmentId);
            return View($"{_viewFolder}Edit.cshtml", course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(
            CourseAddEditDto courseDto, [FromServices] ICourseService service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateCourseAndSaveAsync(courseDto);
                }
                catch (Exception)
                {
                    //_logger.LogError(ex, "Failed to edit Course: id={id}", id);
                    ModelState.AddModelError("", Constants.ERROR_MESSAGE_SAVE);
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateDepartmentsDropDownListAsync(service, courseDto.DepartmentId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id, [FromServices] ICourseService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var course = await service.GetCourseByIdForDetailAsync(id.Value);
            if (course == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Delete.cshtml", course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostDelete(int id, [FromServices] ICourseService service)
        {
            await service.DeleteCourseAndSaveAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
