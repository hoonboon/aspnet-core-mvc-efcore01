using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseListDto>> ListAllCoursesAsync();

        Task<CourseDetailDto> GetCourseByIdForDetailAsync(int courseId);

        Task<IEnumerable<DepartmentOptionDto>> ListAllDepartmentOptionsAsync();

        Task<int> CreateCourseAndSaveAsync(CourseAddEditDto dto);

        Task<CourseAddEditDto> GetCourseByIdForEditAsync(int courseId);

        Task<int> UpdateCourseAndSaveAsync(CourseAddEditDto dto);

        Task<int> DeleteCourseAndSaveAsync(int courseId);
    }
}
