using Microsoft.AspNetCore.Identity;
using EMS.Domain.Entities;

namespace EMS.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
