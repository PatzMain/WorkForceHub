using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Application.Common;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<EmployeeDto?> GetByUserIdAsync(string userId);
        
        Task<PagedResult<EmployeeListDto>> GetPagedAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize);

        Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
        Task UpdateAsync(EmployeeUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
