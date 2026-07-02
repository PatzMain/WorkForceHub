using System.Collections.Generic;
using EMS.Domain.Common;

namespace EMS.Domain.Entities
{
    public class Position : BaseEntity, ISoftDelete
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public string SalaryGrade { get; set; } = string.Empty;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public bool IsDeleted { get; set; }
    }
}
