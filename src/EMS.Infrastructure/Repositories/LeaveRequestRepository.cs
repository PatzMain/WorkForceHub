using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Application.Interfaces;
using EMS.Infrastructure.Data;

namespace EMS.Infrastructure.Repositories
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(int id)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.ReviewedBy)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.ReviewedBy)
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<LeaveRequest>> GetAllLeaveRequestsWithDetailsAsync()
        {
            return await _context.LeaveRequests
                .Include(l => l.Employee)
                .Include(l => l.ReviewedBy)
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<bool> CheckOverlappingApprovedRequestsAsync(int employeeId, DateTime start, DateTime end)
        {
            // An overlap occurs if the requested range [start, end] intersects with an already approved leave range
            return await _context.LeaveRequests
                .AnyAsync(l => l.EmployeeId == employeeId 
                    && l.Status == LeaveStatus.Approved
                    && l.StartDate <= end 
                    && l.EndDate >= start);
        }
    }
}
