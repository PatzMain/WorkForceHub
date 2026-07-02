using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetDepartmentWithDetailsAsync(int id);
        Task<IReadOnlyList<Department>> GetAllDepartmentsWithDetailsAsync();
    }
}
