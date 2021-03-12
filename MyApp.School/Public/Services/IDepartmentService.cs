using MyApp.School.Public.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.School.Public.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentListDto>> ListAllDepartmentsAsync();

        Task<DepartmentDetailDto> GetDepartmentByIdForDetailAsync(int departmentId);

        Task<IEnumerable<InstructorOptionDto>> ListAllInstructorOptionsAsync();

        Task<int> CreateDepartmentAndSaveAsync(DepartmentAddEditDto dto);

        Task<DepartmentAddEditDto> GetDepartmentByIdForEditAsync(int departmentId);

        Task<int> UpdateDepartmentAndSaveAsync(DepartmentAddEditDto dto, byte[] rowVersion);

        Task<int> DeleteDepartmentAndSaveAsync(int departmentId, byte[] rowVersion);
    }
}
