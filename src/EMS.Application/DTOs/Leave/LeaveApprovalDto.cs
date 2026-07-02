using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Leave
{
    public class LeaveApprovalDto
    {
        public int Id { get; set; }
        public LeaveStatus Status { get; set; }
        public int ReviewedById { get; set; }
        public string? ReviewNotes { get; set; }
    }
}
