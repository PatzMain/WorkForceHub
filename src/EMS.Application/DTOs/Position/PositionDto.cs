using System;

namespace EMS.Application.DTOs.Position
{
    public class PositionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string SalaryGrade { get; set; } = string.Empty;
        
        public int TotalEmployees { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
