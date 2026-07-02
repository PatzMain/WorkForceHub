namespace EMS.Application.DTOs.Department
{
    public class DepartmentCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
    }
}
