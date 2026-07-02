using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Application.Interfaces;
using EMS.Infrastructure.Data;

namespace EMS.Infrastructure.Repositories
{
    public class PayrollRepository : Repository<PayrollRecord>, IPayrollRepository
    {
        public PayrollRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PayrollRecord?> GetPayrollWithDetailsAsync(int id)
        {
            return await _context.PayrollRecords
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<PayrollRecord>> GetPayrollByEmployeeIdAsync(int employeeId)
        {
            return await _context.PayrollRecords
                .Include(p => p.Employee)
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.PayPeriodStart)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PayrollRecord>> GetAllPayrollWithDetailsAsync()
        {
            return await _context.PayrollRecords
                .Include(p => p.Employee)
                .OrderByDescending(p => p.PayPeriodStart)
                .ToListAsync();
        }
    }
}
