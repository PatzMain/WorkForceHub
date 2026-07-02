using System;

namespace EMS.Application.DTOs.Department
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        public int? ManagerId { get; set; }
        public string? ManagerFullName { get; set; }

        public int TotalEmployees { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
