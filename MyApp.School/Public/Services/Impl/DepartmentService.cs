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
    public class DepartmentService : IDepartmentService
    {
        private readonly ILogger<DepartmentService> _logger;
        private readonly SchoolDbContext _context;

        public DepartmentService(ILogger<DepartmentService> logger, SchoolDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IEnumerable<DepartmentListDto>> ListAllDepartmentsAsync()
        {
            return await _context.Departments
                .Select(d => new DepartmentListDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Budget = d.Budget,
                    StartDate = d.StartDate,
                    InstructorName = d.InstructorId.HasValue ? d.Administrator.FullName : null
                })
                .OrderBy(d => d.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<DepartmentDetailDto> GetDepartmentByIdForDetailAsync(int departmentId)
        {
            return await _context.Departments
                .Select(d => new DepartmentDetailDto
                {
                    DepartmentId = d.DepartmentId,
                    Name = d.Name,
                    Budget = d.Budget,
                    StartDate = d.StartDate,
                    InstructorName = d.InstructorId.HasValue ? d.Administrator.FullName : null,
                    RowVersion = d.RowVersion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentId == departmentId);
        }

        public async Task<IEnumerable<InstructorOptionDto>> ListAllInstructorOptionsAsync()
        {
            return await _context.Instructors
                .OrderBy(d => d.LastName)
                    .ThenBy(d => d.FirstMidName)
                .Select(d => new InstructorOptionDto
                {
                    InstructorId = d.InstructorId,
                    InstructorName = d.FullName
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CreateDepartmentAndSaveAsync(DepartmentAddEditDto dto)
        {
            int result;
            try
            {
                _context.Add(new Department { 
                    DepartmentId = dto.DepartmentId,
                    Budget = dto.Budget,
                    InstructorId = dto.InstructorId,
                    Name = dto.Name,
                    StartDate = dto.StartDate,
                    RowVersion = dto.RowVersion
                });
                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to create new Department: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<DepartmentAddEditDto> GetDepartmentByIdForEditAsync(int departmentId)
        {
            return await _context.Departments
                .Select(c => new DepartmentAddEditDto
                {
                    DepartmentId = c.DepartmentId,
                    Budget = c.Budget,
                    InstructorId = c.InstructorId,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    RowVersion = c.RowVersion
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.DepartmentId == departmentId);
        }

        public async Task<int> UpdateDepartmentAndSaveAsync(DepartmentAddEditDto dto, byte[] rowVersion)
        {
            var result = 0;

            try
            {
                var departmentToUpdate = await _context.Departments
                    .FirstOrDefaultAsync(m => m.DepartmentId == dto.DepartmentId);

                if (departmentToUpdate == null)
                {
                    var errMsg = $"Record does not exist or has been deleted for DepartmentId={dto.DepartmentId}";
                    _logger.LogError("", errMsg);
                    throw new Exception(errMsg);
                }

                _context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;

                departmentToUpdate.Budget = dto.Budget;
                departmentToUpdate.InstructorId = dto.InstructorId;
                departmentToUpdate.Name = dto.Name;
                departmentToUpdate.StartDate = dto.StartDate;
                
                // Comment out this line to enable updating only the fields with changed values
                //_context.Departments.Update(courseInDb);

                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var exceptionEntry = ex.Entries.Single();
                var userValues = (Department)exceptionEntry.Entity;
                var dbEntry = exceptionEntry.GetDatabaseValues();
                var errMsg = "";
                if (dbEntry == null)
                {
                    errMsg = "Unable to save changes. The department was deleted by another user.";
                    throw new Exception(errMsg);
                }
                else
                {
                    var dbValues = (Department)dbEntry.ToObject();
                    
                    if (dbValues.Name != userValues.Name)
                    {
                        errMsg += $"<li>Conflict field \"Name\": New value is \"{dbValues.Name}\"</li>";
                    }
                    if (dbValues.Budget != userValues.Budget)
                    {
                        errMsg += $"<li>Conflict field \"Budget\": New value is \"{dbValues.Budget:c}\"</li>";
                    }
                    if (dbValues.StartDate != userValues.StartDate)
                    {
                        errMsg += $"<li>Conflict field \"StartDate\": New value is \"{dbValues.StartDate:d}\"</li>";
                    }
                    if (dbValues.InstructorId != userValues.InstructorId)
                    {
                        Instructor dbInstructor = await _context.Instructors
                            .FirstOrDefaultAsync(i => i.InstructorId == dbValues.InstructorId);
                        errMsg += $"<li>Conflict field \"Administrator\": New value is \"{dbInstructor?.FullName}\"</li>";
                    }

                    errMsg = "The record you attempted to edit "
                        + "was modified by another user after you got the original value." 
                        + "<br><br>The edit operation was canceled and the new values in the database displayed as below:"
                        + "<ul>" + errMsg + "</ul>"
                        + "<br>If you still intend to edit this record, click "
                        + "the \"Save\" button again. Otherwise click the \"Back to List\".";
                    
                    dto.RowVersion = (byte[])dbValues.RowVersion;
                    
                    throw new Exception(errMsg);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to update Department: {dto}");
                throw ex;
            }

            return result;
        }

        public async Task<int> DeleteDepartmentAndSaveAsync(int departmentId, byte[] rowVersion)
        {
            var result = 0;
            try
            {
                var departmentToDelete = await _context.Departments
                    .FirstOrDefaultAsync(m => m.DepartmentId == departmentId);

                if (departmentToDelete == null)
                {
                    var errMsg = $"Record does not exist for DepartmentId={departmentId}";
                    _logger.LogError("", errMsg);
                    throw new Exception(errMsg);
                }

                _context.Entry(departmentToDelete).Property("RowVersion").OriginalValue = rowVersion;

                _context.Departments.Remove(departmentToDelete);

                result = await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Failed to delete DepartmentId: {departmentId}");
                
                var errMsg = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database is being displayed. If you still intend to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
                throw new Exception(errMsg);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Failed to delete DepartmentId: {departmentId}");
                throw ex;
            }

            return result;
        }
    }
}
