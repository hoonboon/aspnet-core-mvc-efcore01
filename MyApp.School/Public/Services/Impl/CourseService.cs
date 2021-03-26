using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.School.Domains;
using MyApp.School.Public.Data;
using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services.Impl
{
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly SchoolDbContext _context;

        public CourseService(ILogger<CourseService> logger, SchoolDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<DepartmentOptionDto>> ListAllDepartmentOptionsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentOptionDto { 
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.Name
                })
                .OrderBy(d => d.DepartmentName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<CourseDetailDto> GetCourseByIdForDetailAsync(int courseId)
        {
            return await _context.Courses
                .Select(c => new CourseDetailDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Credits = c.Credits,
                    DepartmentName = c.Department.Name
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseId == courseId);
        }

        public async Task<IEnumerable<CourseListDto>> ListAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CourseListDto { 
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Credits = c.Credits,
                    DepartmentName = c.Department.Name
                })
                .OrderBy(c => c.CourseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateCourseAndSaveAsync(CourseAddEditDto dto)
        {
            var result = 0;
            try
            {
                _context.Add(new Course { 
                    CourseId = dto.CourseId,
                    Title = dto.Title,
                    Credits = dto.Credits,
                    DepartmentId = dto.DepartmentId
                });
                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create new Course: {Course}", dto.ToString());
                throw ex;
            }

            return result;
        }

        public async Task<CourseAddEditDto> GetCourseByIdForEditAsync(int courseId)
        {
            return await _context.Courses
                .Select(c => new CourseAddEditDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Credits = c.Credits,
                    DepartmentId = c.DepartmentId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseId == courseId);
        }

        public async Task<int> UpdateCourseAndSaveAsync(CourseAddEditDto dto)
        {
            var result = 0;
            try
            {
                var courseInDb = await _context.Courses
                    .FirstOrDefaultAsync(m => m.CourseId == dto.CourseId);

                if (courseInDb == null)
                {
                    _logger.LogError("", $"Record does not exist for CourseId={dto.CourseId}");
                    return -1;
                }

                courseInDb.Title = dto.Title;
                courseInDb.Credits = dto.Credits;
                courseInDb.DepartmentId = dto.DepartmentId;
                
                // Comment out this line to enable updating only the fields with changed values
                //_context.Courses.Update(courseInDb);

                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to update Course: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<int> DeleteCourseAndSaveAsync(int courseId)
        {
            var result = 0;
            try
            {
                var courseInDb = await _context.Courses
                    .FirstOrDefaultAsync(m => m.CourseId == courseId);

                if (courseInDb == null)
                {
                    _logger.LogError("", $"Record does not exist for CourseId={courseId}");
                    return -1;
                }

                _context.Courses.Remove(courseInDb);

                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to delete CourseId: {courseId}");
                throw ex;
            }

            return result;
        }
    }
}
