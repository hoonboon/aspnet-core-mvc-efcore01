using MyApp.Common.Public.Dtos;
using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services
{
    public interface IStudentService
    {
        Task<PaginatedListDto<StudentListItem>> ListAllStudentsAsync(ListingFilterSortPageDto filterSortPageDto);

        Task<StudentDetailDto> GetStudentByIdForDetailAsync(int studentId);

        Task<int> CreateStudentAndSaveAsync(StudentAddEditDto dto);

        Task<StudentAddEditDto> GetStudentByIdForEditAsync(int studentId);

        Task<int> UpdateStudentAndSaveAsync(StudentAddEditDto dto);

        Task<int> DeleteStudentAndSaveAsync(int studentId);
    }
}
