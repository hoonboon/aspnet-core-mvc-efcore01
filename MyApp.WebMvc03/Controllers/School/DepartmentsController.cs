using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyApp.WebMvc03.Data;
using MyApp.WebMvc03.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using MyApp.School.Public.Data;
using MyApp.School.Domains;

namespace MyApp.WebMvc03.Controllers.School
{
    [Authorize(Roles = "Manager")]
    public class DepartmentsController : Controller
    {
        private readonly SchoolDbContext _context;
        private readonly ILogger<DepartmentsController> _logger;

        private static readonly string _viewFolder = "/Views/School/Departments/";

        public DepartmentsController(SchoolDbContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View($"{_viewFolder}Index.cshtml", await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View($"{_viewFolder}Details.cshtml", department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName");
            return View($"{_viewFolder}Create.cshtml");
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentId,Name,Budget,StartDate,InstructorId,RowVersion")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName", department.InstructorId);
            return View($"{_viewFolder}Create.cshtml", department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName", department.InstructorId);
            return View($"{_viewFolder}Edit.cshtml", department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(
                    string.Empty, 
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName", deletedDepartment.InstructorId);
            }

            _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            var isUpdateable = await TryUpdateModelAsync(
                departmentToUpdate, "",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorId);

            if (isUpdateable)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var userValues = (Department)exceptionEntry.Entity;
                    var dbEntry = exceptionEntry.GetDatabaseValues();
                    if (dbEntry == null)
                    {
                        ModelState.AddModelError(
                            string.Empty, 
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var dbValues = (Department)dbEntry.ToObject();

                        if (dbValues.Name != userValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {dbValues.Name}");
                        }
                        if (dbValues.Budget != userValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {dbValues.Budget:c}");
                        }
                        if (dbValues.StartDate != userValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {dbValues.StartDate:d}");
                        }
                        if (dbValues.InstructorId != userValues.InstructorId)
                        {
                            Instructor dbInstructor = await _context.Instructors
                                .FirstOrDefaultAsync(i => i.InstructorId == dbValues.InstructorId);
                            ModelState.AddModelError("InstructorId", $"Current value: {dbInstructor?.FullName}");
                        }

                        ModelState.AddModelError(
                            string.Empty, 
                            "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still intend to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToUpdate.RowVersion = (byte[])dbValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }

            ViewData["InstructorId"] = new SelectList(_context.Instructors, "InstructorId", "FullName", departmentToUpdate.InstructorId);
            return View($"{_viewFolder}Edit.cshtml", departmentToUpdate);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? hasConcurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                // deleted by another user
                if (hasConcurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (hasConcurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMsg"] =
                    "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database is being displayed. If you still intend to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View($"{_viewFolder}Delete.cshtml", department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Department department)
        {
            try
            {
                if (await _context.Departments.AnyAsync(d => d.DepartmentId == department.DepartmentId))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Failed to delete Department: id={id}", department.DepartmentId);
                return RedirectToAction(nameof(Delete), new { hasConcurrencyError = true, id = department.DepartmentId });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
    }
}
