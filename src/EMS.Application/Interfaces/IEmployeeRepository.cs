using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetEmployeeWithDetailsAsync(int id);
        Task<Employee?> GetEmployeeByUserIdAsync(string userId);
        Task<IReadOnlyList<Employee>> GetEmployeesPagedAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize);

        Task<int> GetEmployeesCountAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status);
    }
}
