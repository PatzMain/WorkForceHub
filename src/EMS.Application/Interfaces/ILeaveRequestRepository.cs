using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Domain.Entities;

namespace EMS.Application.Interfaces
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task<LeaveRequest?> GetLeaveRequestWithDetailsAsync(int id);
        Task<IReadOnlyList<LeaveRequest>> GetLeaveRequestsByEmployeeIdAsync(int employeeId);
        Task<IReadOnlyList<LeaveRequest>> GetAllLeaveRequestsWithDetailsAsync();
        Task<bool> CheckOverlappingApprovedRequestsAsync(int employeeId, DateTime start, DateTime end);
    }
}
