namespace EMS.Application.DTOs.Department
{
    public class DepartmentUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? ManagerId { get; set; }
    }
}
