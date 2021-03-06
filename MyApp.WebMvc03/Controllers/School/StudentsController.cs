﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyApp.Common.Dtos;
using MyApp.Common.Exceptions;
using MyApp.School.Public.Dtos;
using MyApp.School.Public.Services;
using MyApp.WebMvc03.Utils;
using System;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.School
{
    [Authorize(Roles = "Staff")]
    public class StudentsController : Controller
    {
        private readonly ILogger<StudentsController> _logger;

        private static readonly string _viewFolder = "/Views/School/Students/";

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            ListingFilterSortPageDto filterSortPageDto, [FromServices] IStudentService service)
        {
            PaginatedListDto<StudentListItem> paginatedList = null;

            try
            {
                paginatedList = await service.ListAllStudentssAsync(filterSortPageDto);

                // set the final page index based on the latest database records available
                filterSortPageDto.PageIndex = paginatedList.PageIndex;
            }
            catch (GeneralException ex)
            {
                _logger.LogError("GeneralException in Index: " + ex.Message);
                ViewBag.HasError = true;
                ViewBag.Message = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in Index: " + ex.Message);
                ViewBag.HasError = true;
                ViewBag.Message = Constants.ERROR_MESSAGE_STANDARD + ": " + ex.Message;
            }

            var studentListDto = new StudentListDto
            {
                FilterSortPageValues = filterSortPageDto,
                Listing = paginatedList
            };

            return View($"{_viewFolder}Index.cshtml", studentListDto);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id, [FromServices] IStudentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var student = await service.GetStudentByIdForDetailAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Details.cshtml", student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View($"{_viewFolder}Create.cshtml");
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StudentAddEditDto student, [FromServices] IStudentService service)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.CreateStudentAndSaveAsync(student);
                    TempData["Message"] = Constants.SUCCESS_MESSAGE;
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in Create: " + ex.Message);
                    ViewBag.HasError = true;
                    ViewBag.Message = Constants.ERROR_MESSAGE_SAVE + ": " + ex.Message;
                }
            }

            return View($"{_viewFolder}Create.cshtml", student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id, [FromServices] IStudentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var student = await service.GetStudentByIdForEditAsync(id.Value);
            if (student == null)
            {
                return NotFound();
            }
            return View($"{_viewFolder}Edit.cshtml", student);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(
            int? id, StudentAddEditDto studentDto, [FromServices] IStudentService service)
        {
            if (!id.HasValue
                || id.Value != studentDto.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await service.UpdateStudentAndSaveAsync(studentDto);
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

            return View($"{_viewFolder}Edit.cshtml", studentDto);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(
            int? id, [FromServices] IStudentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var hasPreviousError = TempData["HasError"] != null && Convert.ToBoolean(TempData["HasError"]);
            var message = Convert.ToString(TempData["Message"]);

            var student = await service.GetStudentByIdForDetailAsync(id.Value);
            if (student == null)
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

            return View($"{_viewFolder}Delete.cshtml", student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int? id, [FromServices] IStudentService service)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                await service.DeleteStudentAndSaveAsync(id.Value);
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
