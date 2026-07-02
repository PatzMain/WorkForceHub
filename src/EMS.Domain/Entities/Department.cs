using System.Collections.Generic;
using EMS.Domain.Common;

namespace EMS.Domain.Entities
{
    public class Department : BaseEntity, ISoftDelete
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        
        // Cyclical reference: Department has a Manager who is an Employee
        public int? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        
        public bool IsDeleted { get; set; }
    }
}
