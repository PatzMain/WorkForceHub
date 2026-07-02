using System;
using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Leave
{
    public class LeaveRequestUpdateDto
    {
        public int Id { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
