namespace EMS.Application.DTOs.Position
{
    public class PositionCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string SalaryGrade { get; set; } = string.Empty;
    }
}
