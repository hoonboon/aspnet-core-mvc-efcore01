using AspnetCoreWebMvcApp03.Data;
using AspnetCoreWebMvcApp03.Models;
using AspnetCoreWebMvcApp03.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCoreWebMvcApp03.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(SchoolContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Students
        public async Task<IActionResult> Index(
            string sortOrder, string searchStringCurrent, string searchStringNext, int? pageNumber)
        {
            var students = from s in _context.Students
                           select s;

            // Search
            if (!String.IsNullOrWhiteSpace(searchStringNext))
            {
                pageNumber = 1;
            }
            else
            {
                searchStringNext = searchStringCurrent;
            }

            ViewData["CurrentSearchString"] = searchStringNext;

            if (!String.IsNullOrWhiteSpace(searchStringNext))
            {
                students = students.Where(s => (s.LastName.Contains(searchStringNext) || s.FirstMidName.Contains(searchStringNext)));
            }

            // Order By
            if (String.IsNullOrWhiteSpace(sortOrder))
            {
                sortOrder = "LastName";
            }

            ViewData["SortOrder"] = sortOrder;
            ViewData["NextSortLname"] = sortOrder == "LastName" ? "LastName_desc" : "LastName";
            ViewData["NextSortFname"] = sortOrder == "FirstMidName" ? "FirstMidName_desc" : "FirstMidName";
            ViewData["NextSortDate"] = sortOrder == "EnrollmentDate" ? "EnrollmentDate_desc" : "EnrollmentDate";

            bool descending = false;
            if (sortOrder.EndsWith("_desc"))
            {
                sortOrder = sortOrder.Substring(0, sortOrder.Length - 5);
                descending = true;
            }

            if (descending)
            {
                students = students.OrderByDescending(e => EF.Property<object>(e, sortOrder));
            }
            else
            {
                students = students.OrderBy(e => EF.Property<object>(e, sortOrder));
            }

            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1));
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
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
            return View(student);
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
            return View(student);
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

            var studentDb = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
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

            return View(studentDb);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            if (deleteConfirmedError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = Constants.ERROR_MESSAGE_DELETE;
            }

            return View(student);
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
                    return RedirectToAction(nameof(Delete), new { id = id, deleteConfirmedError = true });
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
