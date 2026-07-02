using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using EMS.Application.Common;
using EMS.Application.Interfaces;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.UnitTests.Services
{
    public class EmployeeServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IEmployeeRepository> _mockRepo;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<EmployeeProfile>());
            _mapper = config.CreateMapper();
            _mockRepo = new Mock<IEmployeeRepository>();
            _service = new EmployeeService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsEmployeeDto()
        {
            // Arrange
            var emp = new Employee { Id = 1, FirstName = "Jane", LastName = "Doe", EmployeeNumber = "EMP-001" };
            _mockRepo.Setup(r => r.GetEmployeeWithDetailsAsync(1)).ReturnsAsync(emp);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("Jane Doe", result.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetEmployeeWithDetailsAsync(99)).ReturnsAsync((Employee?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsPagedResult()
        {
            // Arrange
            var list = new List<Employee> { new Employee { Id = 1, FirstName = "Jane" } };
            _mockRepo.Setup(r => r.GetEmployeesPagedAsync(null, null, null, null, null, false, 1, 10)).ReturnsAsync(list);
            _mockRepo.Setup(r => r.GetEmployeesCountAsync(null, null, null, null)).ReturnsAsync(1);

            // Act
            var result = await _service.GetPagedAsync(null, null, null, null, null, false, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }
    }
}
