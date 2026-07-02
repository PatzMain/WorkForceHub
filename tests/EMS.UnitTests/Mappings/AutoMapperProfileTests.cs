using AutoMapper;
using Xunit;
using EMS.Application.Mappings;

namespace EMS.UnitTests.Mappings
{
    public class AutoMapperProfileTests
    {
        [Fact]
        public void Assert_All_Profiles_Configuration_Is_Valid()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DepartmentProfile>();
                cfg.AddProfile<EmployeeProfile>();
                cfg.AddProfile<LeaveRequestProfile>();
                cfg.AddProfile<PayrollProfile>();
                cfg.AddProfile<PositionProfile>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}
