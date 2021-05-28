using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Admin.Security.Public.Enums;
using MyApp.Admin.Security.Public.PermissionControl.Policy;
using MyApp.Common.Public.Exceptions;
using MyApp.School.Public.Dtos;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.School
{
    public class InstructorsController : Controller
    {
        private readonly ILogger<InstructorsController> _logger;

        private static readonly string _viewFolder = "/Views/School/Instructors/";

        public InstructorsController(ILogger<InstructorsController> logger)
        {
            _logger = logger;
        }

        [HasPermission(Permissions.InstructorView)]
        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseId, [FromServices] IInstructorService service)
        {
            var viewModel = new InstructorListDto();
            try
            {
                viewModel.Instructors = await service.ListAllInstructorsAsync();

                if (id.HasValue)
                {
                    ViewData["InstructorId"] = id.Value;
                    viewModel.Courses = await service.ListAllInstructorCoursesAsync(id.Value);
                }

                if (courseId.HasValue)
                {
                    ViewData["CourseId"] = courseId.Value;
                    viewModel.Enrollments = await service.ListAllStudentsEnrolledInCourseAsync(courseId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in Index: " + ex.Message);
                ViewBag.HasError = true;
                ViewBag.Message = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
            }

            return View($"{_viewFolder}Index.cshtml", viewModel);
        }

        [HasPermission(Permissions.InstructorView)]
        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id, [FromServices] IInstructorService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var instructor = await service.GetInstructorByIdForDetailAsync(id.Value);
            if (instructor == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Details.cshtml", instructor);
        }

        private async Task PopulateCourseAssignedDataAsync(
            IEnumerable<int> currentAssignedCoursesId, IInstructorService service)
        {
            ViewData["Courses"] = await service.ListAllCourseOptionsAsync(currentAssignedCoursesId);
        }

        [HasPermission(Permissions.InstructorAdd)]
        // GET: Instructors/Create
        public async Task<IActionResult> Create([FromServices] IInstructorService service)
        {
            await PopulateCourseAssignedDataAsync(new List<int>(), service);
            return View($"{_viewFolder}Create.cshtml");
        }

        [HasPermission(Permissions.InstructorAdd)]
        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            InstructorAddEditDto instructorDto, [FromServices] IInstructorService service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.CreateInstructorAndSaveAsync(instructorDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (GeneralException ex)
                {
                    ViewBag.HasError = true;
                    ViewBag.Message = ex.Message;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in Create: " + ex.Message);
                    ViewBag.HasError = true;
                    ViewBag.Message = Constants.ERROR_MESSAGE_SAVE + ": " + ex.Message;
                }
            }

            await PopulateCourseAssignedDataAsync(instructorDto.CoursesAssigned, service);

            return View($"{_viewFolder}Create.cshtml", instructorDto);
        }

        [HasPermission(Permissions.InstructorEdit)]
        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id, [FromServices] IInstructorService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var instructor = await service.GetInstructorByIdForEditAsync(id.Value);
            if (instructor == null)
            {
                return NotFound();
            }
            
            await PopulateCourseAssignedDataAsync(instructor.CoursesAssigned, service);

            return View($"{_viewFolder}Edit.cshtml", instructor);
        }

        [HasPermission(Permissions.InstructorEdit)]
        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(
            int? id, InstructorAddEditDto instructorDto, [FromServices] IInstructorService service)
        {
            if (!id.HasValue || id.Value != instructorDto.InstructorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateInstructorAndSaveAsync(instructorDto);
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

                return RedirectToAction(nameof(Index));
            }

            await PopulateCourseAssignedDataAsync(instructorDto.CoursesAssigned, service);

            return View($"{_viewFolder}Edit.cshtml", instructorDto);
        }

        [HasPermission(Permissions.InstructorDelete)]
        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id, [FromServices] IInstructorService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var hasPreviousError = TempData["HasError"] != null && Convert.ToBoolean(TempData["HasError"]);
            var message = Convert.ToString(TempData["Message"]);

            var instructor = await service.GetInstructorByIdForDetailAsync(id.Value);
            if (instructor == null)
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

            return View($"{_viewFolder}Delete.cshtml", instructor);
        }

        [HasPermission(Permissions.InstructorDelete)]
        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int?  id, [FromServices] IInstructorService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                await service.DeleteInstructorAndSaveAsync(id.Value);
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
