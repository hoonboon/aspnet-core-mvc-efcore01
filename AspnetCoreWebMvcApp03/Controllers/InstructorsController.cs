using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspnetCoreWebMvcApp03.Data;
using AspnetCoreWebMvcApp03.Models;
using Microsoft.Extensions.Logging;
using AspnetCoreWebMvcApp03.Models.SchoolViewModels;
using AspnetCoreWebMvcApp03.Utils;

namespace AspnetCoreWebMvcApp03.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;
        private readonly ILogger<InstructorsController> _logger;

        public InstructorsController(SchoolContext context, ILogger<InstructorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseId)
        {
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Department)
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                ViewData["InstructorId"] = id.Value;
                Instructor instructor = viewModel.Instructors.Where(
                        i => i.Id == id.Value
                    ).Single();
                viewModel.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }

            if (courseId != null)
            {
                ViewData["CourseId"] = courseId.Value;
                viewModel.Enrollments = viewModel.Courses.Where(
                        c => c.CourseId == courseId.Value
                    ).Single().Enrollments;
            }

            return View(viewModel);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            PopulateCourseAssignedData(
                new Instructor { 
                    CourseAssignments = new List<CourseAssignment>() 
                });
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LastName,FirstMidName,HireDate,OfficeAssignment")] Instructor instructor,
            string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                var courseAssignments = new List<CourseAssignment>();
                foreach (var courseIdStr in selectedCourses)
                {
                    courseAssignments.Add(
                        new CourseAssignment { 
                            InstructorId = instructor.Id,
                            CourseId = int.Parse(courseIdStr)
                        });
                }
                instructor.CourseAssignments = courseAssignments;
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateCourseAssignedData(instructor);

            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }
            PopulateCourseAssignedData(instructor);
            return View(instructor);
        }

        private void PopulateCourseAssignedData(Instructor instructor)
        {
            var allCourses = _context.Courses.OrderBy(c => c.CourseId).ToList();
            var currentAssignedCoursesId = new HashSet<int>(instructor.CourseAssignments.Select(ca => ca.CourseId));
            var courseOptions = new List<CourseAssignedData>();
            foreach (var course in allCourses)
            {
                courseOptions.Add(
                    new CourseAssignedData { 
                        CourseId = course.CourseId,
                        Title = course.Title,
                        IsAssigned = currentAssignedCoursesId.Contains(course.CourseId)
                    });
            }
            ViewData["Courses"] = courseOptions;
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostEdit(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorDb = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                .FirstOrDefaultAsync(i => i.Id == id);

            var isUpdateable = await TryUpdateModelAsync<Instructor>(
                instructorDb, "",
                i => i.FirstMidName, i => i.LastName, 
                i => i.HireDate, i => i.OfficeAssignment);

            if (isUpdateable)
            {
                if (string.IsNullOrWhiteSpace(instructorDb.OfficeAssignment?.Location))
                {
                    instructorDb.OfficeAssignment = null;
                }

                UpdateInstructorCourses(selectedCourses, instructorDb);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    //_logger.LogError(ex, "Failed to edit Instructor: id={id}", id);
                    ModelState.AddModelError("", Constants.ERROR_MESSAGE_SAVE);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(instructorDb);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var coursesAssignedNew = new HashSet<string>(selectedCourses);
            var coursesAssignedExisting = new HashSet<int>(
                instructorToUpdate.CourseAssignments.Select(ca => ca.CourseId));

            foreach (var course in _context.Courses)
            {
                if (coursesAssignedNew.Contains(course.CourseId.ToString()))
                {
                    if (!coursesAssignedExisting.Contains(course.CourseId))
                    {
                        // add new record
                        instructorToUpdate.CourseAssignments.Add(
                            new CourseAssignment { 
                                InstructorId = instructorToUpdate.Id,
                                CourseId = course.CourseId
                            });
                    }
                }
                else
                {
                    if (coursesAssignedExisting.Contains(course.CourseId))
                    {
                        // remove existing record
                        var itemToRemove = instructorToUpdate.CourseAssignments.FirstOrDefault(ca => ca.CourseId == course.CourseId);
                        _context.Remove(itemToRemove);
                    }
                }
            }
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.CourseAssignments) // to effect cascade delete
                .SingleAsync(i => i.Id == id);

            var departments = await _context.Departments
                .Where(d => d.InstructorId == id)
                .ToListAsync();
            departments.ForEach(d => d.InstructorId = null);

            _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.Id == id);
        }
    }
}
