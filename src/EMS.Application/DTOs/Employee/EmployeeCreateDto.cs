using System;
using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;

        public string EmployeeNumber { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
        public string? ProfilePictureUrl { get; set; }

        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public string? ApplicationUserId { get; set; }
    }
}
