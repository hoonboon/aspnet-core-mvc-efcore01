using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Common.Public.Exceptions;
using MyApp.School.Domains;
using MyApp.School.Public.Data;
using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services.Impl
{
    public class InstructorService : IInstructorService
    {
        private readonly ILogger<InstructorService> _logger;
        private readonly SchoolDbContext _context;

        public InstructorService(ILogger<InstructorService> logger, SchoolDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<InstructorListItem>> ListAllInstructorsAsync()
        {
            return await _context.Instructors
                .OrderBy(i => i.LastName)
                .AsNoTracking()
                .Select(i => new InstructorListItem
                {
                    InstructorId = i.InstructorId,
                    LastName = i.LastName,
                    FirstMidName = i.FirstMidName,
                    HireDate = i.HireDate,
                    CoursesAssigned = i.CourseAssignments.Select(ca => new CourseListItem { 
                        CourseId = ca.CourseId,
                        CourseTitle = ca.Course.Title,
                        DepartmentName = ca.Course.Department.Name
                    }),
                    OfficeLocation = i.OfficeAssignment != null ? i.OfficeAssignment.Location : ""
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseListItem>> ListAllInstructorCoursesAsync(int instructorId)
        {
            return await _context.CourseAssignments
                .OrderBy(ca => ca.Course.CourseId)
                .Where(ca => ca.InstructorId == instructorId)
                .AsNoTracking()
                .Select(ca => new CourseListItem
                {
                    CourseId = ca.Course.CourseId,
                    CourseTitle = ca.Course.Title,
                    DepartmentName = ca.Course.Department.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentListItem>> ListAllStudentsEnrolledInCourseAsync(int courseId)
        {
            return await _context.Enrollments
                .OrderBy(e => e.Student.LastName)
                    .ThenBy(e => e.Student.FirstMidName)
                .Where(e => e.CourseId == courseId)
                .AsNoTracking()
                .Select(e => new EnrollmentListItem
                {
                    StudentName = e.Student.FullName,
                    StudentEnrollmentGrade = e.Grade
                })
                .ToListAsync();
        }

        public async Task<InstructorDetailDto> GetInstructorByIdForDetailAsync(int instructorId)
        {
            return await _context.Instructors
                    .Select(s => new InstructorDetailDto
                    {
                        InstructorId = s.InstructorId,
                        LastName = s.LastName,
                        FirstMidName = s.FirstMidName,
                        HireDate = s.HireDate
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.InstructorId == instructorId);
        }

        public async Task<IEnumerable<CourseAssignedData>> ListAllCourseOptionsAsync(IEnumerable<int> currentAssignedCoursesId)
        {
            var allCourses = await _context.Courses.OrderBy(c => c.CourseId).ToListAsync();

            var courseOptions = new List<CourseAssignedData>();
            foreach (var course in allCourses)
            {
                courseOptions.Add(
                    new CourseAssignedData
                    {
                        CourseId = course.CourseId,
                        Title = course.Title,
                        IsAssigned = currentAssignedCoursesId != null && currentAssignedCoursesId.Contains(course.CourseId)
                    });
            }
            return courseOptions;
        }

        public async Task<int> CreateInstructorAndSaveAsync(InstructorAddEditDto dto)
        {
            int result;
            try
            {
                OfficeAssignment officeAssignment = null;
                if (!string.IsNullOrWhiteSpace(dto.OfficeLocation))
                {
                    officeAssignment = new OfficeAssignment
                    {
                        InstructorId = dto.InstructorId,
                        Location = dto.OfficeLocation
                    };
                }

                List<CourseAssignment> courseAssignments = null;
                if (dto.CoursesAssigned != null)
                {
                    courseAssignments = new List<CourseAssignment>();
                    foreach (var courseId in dto.CoursesAssigned)
                    {
                        courseAssignments.Add(
                            new CourseAssignment
                            {
                                InstructorId = dto.InstructorId,
                                CourseId = courseId
                            });
                    }
                    
                }

                _context.Add(new Instructor
                {
                    InstructorId = dto.InstructorId,
                    LastName = dto.LastName,
                    FirstMidName = dto.FirstMidName,
                    HireDate = dto.HireDate,
                    OfficeAssignment = officeAssignment,
                    CourseAssignments = courseAssignments
                });

                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create new Instructor: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<InstructorAddEditDto> GetInstructorByIdForEditAsync(int instructorId)
        {
            return await _context.Instructors
                .Select(c => new InstructorAddEditDto
                {
                    InstructorId = c.InstructorId,
                    LastName = c.LastName,
                    FirstMidName = c.FirstMidName,
                    HireDate = c.HireDate,
                    OfficeLocation = c.OfficeAssignment != null ? c.OfficeAssignment.Location : "",
                    CoursesAssigned = c.CourseAssignments != null ? c.CourseAssignments.Select(ca => ca.CourseId) : null
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.InstructorId == instructorId);
        }

        private void UpdateInstructorCourses(IEnumerable<int> coursesAssignedNew, Instructor instructorToUpdate)
        {
            if (coursesAssignedNew == null || !coursesAssignedNew.Any())
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var coursesAssignedExisting = new HashSet<int>(
                instructorToUpdate.CourseAssignments.Select(ca => ca.CourseId));

            foreach (var course in _context.Courses)
            {
                if (coursesAssignedNew.Contains(course.CourseId))
                {
                    if (!coursesAssignedExisting.Contains(course.CourseId))
                    {
                        // add new record
                        instructorToUpdate.CourseAssignments.Add(
                            new CourseAssignment
                            {
                                InstructorId = instructorToUpdate.InstructorId,
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

        public async Task<int> UpdateInstructorAndSaveAsync(InstructorAddEditDto dto)
        {
            var result = 0;

            try
            {
                var instructorToUpdate = await _context.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Include(i => i.CourseAssignments)
                        .ThenInclude(ca => ca.Course)
                    .FirstOrDefaultAsync(m => m.InstructorId == dto.InstructorId);

                if (instructorToUpdate == null)
                {
                    var errMsg = $"Record does not exist or has been deleted for InstructorId={dto.InstructorId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                // update Instructor
                instructorToUpdate.LastName = dto.LastName;
                instructorToUpdate.FirstMidName = dto.FirstMidName;
                instructorToUpdate.HireDate = dto.HireDate;

                // update OfficeAssignment
                if (string.IsNullOrWhiteSpace(dto.OfficeLocation))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                else if (instructorToUpdate.OfficeAssignment != null)
                {
                    instructorToUpdate.OfficeAssignment.Location = dto.OfficeLocation;
                }
                else
                {
                    instructorToUpdate.OfficeAssignment = new OfficeAssignment
                    {
                        InstructorId = dto.InstructorId,
                        Location = dto.OfficeLocation
                    };
                }

                // update CourseAssignments
                UpdateInstructorCourses(dto.CoursesAssigned, instructorToUpdate);

                // Comment out this line to enable updating only the fields with changed values
                //_context.Departments.Update(courseInDb);

                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update Instructor: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<int> DeleteInstructorAndSaveAsync(int instructorId)
        {
            var result = 0;
            try
            {
                var instructorToDelete = await _context.Instructors
                    .Include(i => i.CourseAssignments) // explicitly trigger cascade delete
                    .Include(i => i.OfficeAssignment) // explicitly trigger cascade delete
                    .FirstOrDefaultAsync(m => m.InstructorId == instructorId);

                if (instructorToDelete == null)
                {
                    var errMsg = $"Record does not exist for InstructorId={instructorId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                var departments = await _context.Departments
                    .Where(d => d.InstructorId == instructorId)
                    .ToListAsync();
                departments.ForEach(d => d.InstructorId = null);

                _context.Instructors.Remove(instructorToDelete);

                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete InstructorId: {instructorId}");
                throw ex;
            }

            return result;
        }
    }
}
