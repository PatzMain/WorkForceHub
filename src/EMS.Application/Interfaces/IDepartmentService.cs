using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Application.DTOs.Department;

namespace EMS.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto);
        Task UpdateAsync(DepartmentUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
