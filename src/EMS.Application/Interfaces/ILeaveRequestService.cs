using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Application.Common;
using EMS.Application.DTOs.Leave;

namespace EMS.Application.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequestDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<LeaveRequestDto>> GetByEmployeeIdAsync(int employeeId);
        Task<IReadOnlyList<LeaveRequestDto>> GetAllAsync();
        Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<LeaveRequestDto>> GetPagedByEmployeeIdAsync(int employeeId, int pageNumber, int pageSize);
        Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto dto);
        Task UpdateAsync(LeaveRequestUpdateDto dto);
        Task CancelAsync(int id);
        Task ApproveOrRejectAsync(LeaveApprovalDto dto);
    }
}
