using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMS.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<Dictionary<string, int>> GetEmployeeCountsByDepartmentAsync();
        Task<Dictionary<string, int>> GetEmployeeCountsByStatusAsync();
        Task<Dictionary<string, int>> GetLeaveRequestCountsByStatusAsync();
        Task<decimal> GetTotalGrossPayrollThisMonthAsync();
        Task<int> GetTotalEmployeesCountAsync();
        Task<int> GetTotalDepartmentsCountAsync();
        Task<int> GetTotalPositionsCountAsync();
    }
}
