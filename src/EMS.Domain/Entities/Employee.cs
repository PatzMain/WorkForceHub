using System;
using System.Collections.Generic;
using EMS.Domain.Common;
using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{
    public class Employee : BaseEntity, ISoftDelete
    {
        // Personal info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; } = string.Empty;

        // Employment info
        public string EmployeeNumber { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; }
        public DateTime? DateTerminated { get; set; }
        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;
        public string? ProfilePictureUrl { get; set; }

        // Foreign keys
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int PositionId { get; set; }
        public Position? Position { get; set; }

        // Clean Architecture: Store identity user ID link without direct ASP.NET Core dependency
        public string? ApplicationUserId { get; set; }

        // Navigation Collections
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<PayrollRecord> PayrollRecords { get; set; } = new List<PayrollRecord>();

        public bool IsDeleted { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
