using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Application.Common;
using EMS.Application.DTOs.Payroll;

namespace EMS.Application.Interfaces
{
    public interface IPayrollService
    {
        Task<PayrollRecordDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<PayrollRecordDto>> GetByEmployeeIdAsync(int employeeId);
        Task<IReadOnlyList<PayrollRecordDto>> GetAllAsync();
        Task<PagedResult<PayrollRecordDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<PayrollRecordDto>> GetPagedByEmployeeIdAsync(int employeeId, int pageNumber, int pageSize);
        Task<PayrollRecordDto> CreateAsync(PayrollRecordCreateDto dto);
        Task UpdateAsync(PayrollRecordUpdateDto dto);
        Task ProcessAsync(int id);
        Task PayAsync(int id);
        Task DeleteAsync(int id);
    }
}
