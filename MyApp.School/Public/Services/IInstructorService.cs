using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorListItem>> ListAllInstructorsAsync();

        Task<IEnumerable<CourseListItem>> ListAllInstructorCoursesAsync(int instructorId);

        Task<IEnumerable<EnrollmentListItem>> ListAllStudentsEnrolledInCourseAsync(int courseId);

        Task<InstructorDetailDto> GetInstructorByIdForDetailAsync(int instructorId);

        Task<IEnumerable<CourseAssignedData>> ListAllCourseOptionsAsync(IEnumerable<int> currentAssignedCoursesId);

        Task<int> CreateInstructorAndSaveAsync(InstructorAddEditDto dto);

        Task<InstructorAddEditDto> GetInstructorByIdForEditAsync(int instructorId);

        Task<int> UpdateInstructorAndSaveAsync(InstructorAddEditDto dto);

        Task<int> DeleteInstructorAndSaveAsync(int instructorId);
    }
}
