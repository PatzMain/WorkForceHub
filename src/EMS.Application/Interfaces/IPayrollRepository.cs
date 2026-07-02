using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IPayrollRepository : IRepository<PayrollRecord>
    {
        Task<PayrollRecord?> GetPayrollWithDetailsAsync(int id);
        Task<IReadOnlyList<PayrollRecord>> GetPayrollByEmployeeIdAsync(int employeeId);
        Task<IReadOnlyList<PayrollRecord>> GetAllPayrollWithDetailsAsync();
    }
}
