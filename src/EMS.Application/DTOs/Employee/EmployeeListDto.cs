using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? PositionTitle { get; set; }
        public EmploymentStatus Status { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public System.DateTime DateJoined { get; set; }
    }
}
