using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using EMS.Application.Interfaces;
using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.UnitTests.Services
{
    public class DashboardServiceTests
    {
        private readonly Mock<IRepository<Employee>> _mockEmployeeRepo;
        private readonly Mock<IDepartmentRepository> _mockDeptRepo;
        private readonly Mock<IRepository<Position>> _mockPosRepo;
        private readonly Mock<IRepository<LeaveRequest>> _mockLeaveRepo;
        private readonly Mock<IRepository<PayrollRecord>> _mockPayrollRepo;
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _mockEmployeeRepo = new Mock<IRepository<Employee>>();
            _mockDeptRepo = new Mock<IDepartmentRepository>();
            _mockPosRepo = new Mock<IRepository<Position>>();
            _mockLeaveRepo = new Mock<IRepository<LeaveRequest>>();
            _mockPayrollRepo = new Mock<IRepository<PayrollRecord>>();

            _service = new DashboardService(
                _mockEmployeeRepo.Object,
                _mockDeptRepo.Object,
                _mockPosRepo.Object,
                _mockLeaveRepo.Object,
                _mockPayrollRepo.Object
            );
        }

        [Fact]
        public async Task GetTotalEmployeesCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var list = new List<Employee>
            {
                new Employee { Id = 1, FirstName = "John" },
                new Employee { Id = 2, FirstName = "Jane" }
            };
            _mockEmployeeRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            // Act
            var count = await _service.GetTotalEmployeesCountAsync();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetTotalDepartmentsCountAsync_ReturnsCorrectCount()
        {
            // Arrange
            var list = new List<Department>
            {
                new Department { Id = 1, Name = "HR" }
            };
            _mockDeptRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            // Act
            var count = await _service.GetTotalDepartmentsCountAsync();

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetEmployeeCountsByStatusAsync_ReturnsCorrectBreakdown()
        {
            // Arrange
            var list = new List<Employee>
            {
                new Employee { Id = 1, Status = EmploymentStatus.Active },
                new Employee { Id = 2, Status = EmploymentStatus.Active },
                new Employee { Id = 3, Status = EmploymentStatus.OnLeave }
            };
            _mockEmployeeRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            // Act
            var breakdown = await _service.GetEmployeeCountsByStatusAsync();

            // Assert
            Assert.True(breakdown.ContainsKey("Active"));
            Assert.Equal(2, breakdown["Active"]);
            Assert.True(breakdown.ContainsKey("OnLeave"));
            Assert.Equal(1, breakdown["OnLeave"]);
        }
    }
}
