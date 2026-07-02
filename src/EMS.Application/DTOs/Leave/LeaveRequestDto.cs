using System;
using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Leave
{
    public class LeaveRequestDto
    {
        public int Id { get; set; }
        
        public int EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }
        public string? EmployeeNumber { get; set; }

        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        
        public LeaveStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int? ReviewedById { get; set; }
        public string? ReviewedByFullName { get; set; }
        public string? ReviewNotes { get; set; }
        public DateTime? DateReviewed { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
