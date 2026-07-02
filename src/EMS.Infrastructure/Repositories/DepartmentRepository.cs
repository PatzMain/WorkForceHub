using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Application.Interfaces;
using EMS.Infrastructure.Data;

namespace EMS.Infrastructure.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Department?> GetDepartmentWithDetailsAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IReadOnlyList<Department>> GetAllDepartmentsWithDetailsAsync()
        {
            return await _context.Departments
                .Include(d => d.Manager)
                .Include(d => d.Employees)
                .ToListAsync();
        }
    }
}
