using System;
using EMS.Domain.Common;
using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string Reason { get; set; } = string.Empty;

        // Review information
        public int? ReviewedById { get; set; }
        public Employee? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime? DateReviewed { get; set; }

        public int TotalDays => (EndDate - StartDate).Days + 1;
    }
}
