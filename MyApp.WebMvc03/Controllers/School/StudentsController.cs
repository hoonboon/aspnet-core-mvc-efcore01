using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Common.Dtos;
using MyApp.School.Domains;
using MyApp.School.Efcore;
using MyApp.School.Public.Data;
using MyApp.School.Public.Dtos;
using MyApp.WebMvc03.Utils;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.WebMvc03.Controllers.School
{
    [Authorize(Roles = "Staff")]
    public class StudentsController : Controller
    {
        private readonly SchoolDbContext _context;
        private readonly ILogger<StudentsController> _logger;

        private static readonly string _viewFolder = "/Views/School/Students/";

        public StudentsController(SchoolDbContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            ListingFilterSortPageDto filterSortPageDto)
        {
            // call this first to:
            // - reset the PageNo by comparing the previous an current listing criteria
            filterSortPageDto.InitDto();

            var students = _context.Students
                .Select(s => new StudentListItem
                {
                    FirstMidName = s.FirstMidName,
                    LastName = s.LastName,
                    EnrollmentDate = s.EnrollmentDate,
                    StudentId = s.StudentId
                });

            StudentsFilterOptions filterBy = (StudentsFilterOptions)filterSortPageDto.FilterBy;
            var filterValue = filterSortPageDto.FilterValue;
            if (filterBy > 0 && !String.IsNullOrWhiteSpace(filterValue))
            {
                switch (filterBy)
                {
                    case StudentsFilterOptions.FirstMidName:
                        students = students.Where(s => (s.FirstMidName.Contains(filterValue)));
                        break;
                    case StudentsFilterOptions.LastName:
                        students = students.Where(s => (s.LastName.Contains(filterValue)));
                        break;
                    case StudentsFilterOptions.EnrollmentDateAfter:
                        try
                        {
                            DateTime filterValueDate = DateTime.ParseExact(filterValue, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                            students = students.Where(s => (s.EnrollmentDate >= filterValueDate));
                        }
                        catch (Exception)
                        {
                            // TODO: handle error
                        }
                        break;
                    case StudentsFilterOptions.EnrollmentDateBefore:
                        try
                        {
                            DateTime filterValueDate = DateTime.ParseExact(filterValue, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                            students = students.Where(s => (s.EnrollmentDate <= filterValueDate));
                        }
                        catch (Exception)
                        {
                            // TODO: handle error
                        }
                        break;
                }
                
            }

            // Order By
            StudentsSortByOptions sortBy = (StudentsSortByOptions) filterSortPageDto.SortBy;
            bool sortAscending = filterSortPageDto.SortAscending;
            if (sortAscending)
            {
                students = students.OrderBy(e => EF.Property<object>(e, sortBy.ToString()));
            }
            else
            {
                students = students.OrderByDescending(e => EF.Property<object>(e, sortBy.ToString())); 
            }

            PageSizeOptions pageSize = (PageSizeOptions) filterSortPageDto.PageSize;
            var pageIndex = filterSortPageDto.PageIndex;

            var paginatedList = await PaginatedListHelper<StudentListItem>.CreateAsync(students.AsNoTracking(), pageIndex, (int) pageSize);

            // set the final page index based on the latest database records available
            filterSortPageDto.PageIndex = paginatedList.PageIndex;

            var studentListDto = new StudentListDto
            {
                FilterSortPageValues = filterSortPageDto,
                Listing = paginatedList
            };

            return View($"{_viewFolder}Index.cshtml", studentListDto);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Enrollments).ThenInclude(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentId == id);
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
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                //_logger.LogError(ex, "Failed to create new Student: {Student}", student.ToString());
                ModelState.AddModelError("", Constants.ERROR_MESSAGE_SAVE);
            }
            return View($"{_viewFolder}Create.cshtml", student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View($"{_viewFolder}Edit.cshtml", student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("StudentId,LastName,FirstMidName,EnrollmentDate")] Student student)
        //{
        //    if (id != student.StudentId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(student);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!StudentExists(student.StudentId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(student);
        //}

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentDb = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id);
            var isTrySucccess = await TryUpdateModelAsync<Student>(
                    studentDb, "", 
                    s => s.FirstMidName,
                    s => s.LastName,
                    s => s.EnrollmentDate
                );
            if (isTrySucccess)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    //_logger.LogError(ex, "Failed to edit Student: id={id}", id);
                    ModelState.AddModelError("", Constants.ERROR_MESSAGE_SAVE);
                }
            }

            return View($"{_viewFolder}Edit.cshtml", studentDb);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? deleteConfirmedError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            if (deleteConfirmedError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = Constants.ERROR_MESSAGE_DELETE;
            }

            return View($"{_viewFolder}Delete.cshtml", student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                try
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    //_logger.LogError(ex, "Failed to delete Student: id={id}", id);
                    return RedirectToAction(nameof(Delete), new { id, deleteConfirmedError = true });
                }
            }

            return RedirectToAction(nameof(Index));
        }

        //private bool StudentExists(int id)
        //{
        //    return _context.Students.Any(e => e.StudentId == id);
        //}
    }
}
