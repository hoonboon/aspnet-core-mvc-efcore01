using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyApp.Common.Public.Exceptions;
using MyApp.Common.Public.Dtos;
using MyApp.School.Domains;
using MyApp.School.Efcore;
using MyApp.School.Public.Data;
using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services.Impl
{
    public class StudentService : IStudentService
    {
        private readonly ILogger<StudentService> _logger;
        private readonly SchoolDbContext _context;

        public StudentService(ILogger<StudentService> logger, SchoolDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<PaginatedListDto<StudentListItem>> ListAllStudentsAsync(
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
                            DateTime filterValueDate = DateTime.ParseExact(
                                filterValue, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                            students = students.Where(s => (s.EnrollmentDate >= filterValueDate));
                        }
                        catch (Exception)
                        {
                            throw new GeneralException("Please enter a valid date value and retry.");
                        }
                        break;
                    case StudentsFilterOptions.EnrollmentDateBefore:
                        try
                        {
                            DateTime filterValueDate = DateTime.ParseExact(
                                filterValue, "yyyy-MM-dd", CultureInfo.CurrentCulture);
                            students = students.Where(s => (s.EnrollmentDate <= filterValueDate));
                        }
                        catch (Exception)
                        {
                            throw new GeneralException("Please enter a valid date value and retry.");
                        }
                        break;
                }

            }

            // Order By
            StudentsSortByOptions sortBy = (StudentsSortByOptions)filterSortPageDto.SortBy;
            bool sortAscending = filterSortPageDto.SortAscending;
            if (sortAscending)
            {
                students = students.OrderBy(e => EF.Property<object>(e, sortBy.ToString()));
            }
            else
            {
                students = students.OrderByDescending(e => EF.Property<object>(e, sortBy.ToString()));
            }

            PageSizeOptions pageSize = (PageSizeOptions)filterSortPageDto.PageSize;
            var pageIndex = filterSortPageDto.PageIndex;

            var paginatedList = await PaginatedListHelper<StudentListItem>.CreateAsync(
                students.AsNoTracking(), pageIndex, (int)pageSize);

            return paginatedList;
        }

        public async Task<StudentDetailDto> GetStudentByIdForDetailAsync(int studentId)
        {
            return await _context.Students
                    .Select(s => new StudentDetailDto
                    {
                        StudentId = s.StudentId,
                        LastName = s.LastName,
                        FirstMidName = s.FirstMidName,
                        EnrollmentDate = s.EnrollmentDate,
                        Enrollments = s.Enrollments.Select(e => new StudentEnrollmentListDto
                        {
                            CourseTitle = e.Course.Title,
                            Grade = e.Grade
                        })
                    })
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.StudentId == studentId);
        }

        public async Task<int> CreateStudentAndSaveAsync(StudentAddEditDto dto)
        {
            int result;
            try
            {
                _context.Add(new Student
                {
                    StudentId = dto.StudentId,
                    LastName = dto.LastName,
                    FirstMidName = dto.FirstMidName,
                    EnrollmentDate = dto.EnrollmentDate
                });
                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create new Student: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<StudentAddEditDto> GetStudentByIdForEditAsync(int studentId)
        {
            return await _context.Students
                .Select(c => new StudentAddEditDto
                {
                    StudentId = c.StudentId,
                    LastName = c.LastName,
                    FirstMidName = c.FirstMidName,
                    EnrollmentDate = c.EnrollmentDate
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentId == studentId);
        }

        public async Task<int> UpdateStudentAndSaveAsync(StudentAddEditDto dto)
        {
            var result = 0;

            try
            {
                var studentToUpdate = await _context.Students
                    .FirstOrDefaultAsync(m => m.StudentId == dto.StudentId);

                if (studentToUpdate == null)
                {
                    var errMsg = $"Record does not exist or has been deleted for StudentId={dto.StudentId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                studentToUpdate.LastName = dto.LastName;
                studentToUpdate.FirstMidName = dto.FirstMidName;
                studentToUpdate.EnrollmentDate = dto.EnrollmentDate;

                // Comment out this line to enable updating only the fields with changed values
                //_context.Departments.Update(courseInDb);

                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update Student: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<int> DeleteStudentAndSaveAsync(int studentId)
        {
            var result = 0;
            try
            {
                var studentToDelete = await _context.Students
                    .FirstOrDefaultAsync(m => m.StudentId == studentId);

                if (studentToDelete == null)
                {
                    var errMsg = $"Record does not exist for StudentId={studentId}";
                    _logger.LogError("", errMsg);
                    throw new GeneralException(errMsg);
                }

                _context.Students.Remove(studentToDelete);

                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete StudentId: {studentId}");
                throw ex;
            }

            return result;
        }
    }
}
