using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using EMS.Application.Common;
using EMS.Application.DTOs.Department;
using EMS.Application.Interfaces;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Entities;

namespace EMS.UnitTests.Services
{
    public class DepartmentServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IDepartmentRepository> _mockRepo;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DepartmentProfile>());
            _mapper = config.CreateMapper();
            _mockRepo = new Mock<IDepartmentRepository>();
            _service = new DepartmentService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsDepartmentDto()
        {
            // Arrange
            var dept = new Department { Id = 1, Name = "HR", Code = "HR" };
            _mockRepo.Setup(r => r.GetDepartmentWithDetailsAsync(1)).ReturnsAsync(dept);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("HR", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetDepartmentWithDetailsAsync(99)).ReturnsAsync((Department?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_WithEmployees_ThrowsException()
        {
            // Arrange
            var dept = new Department 
            { 
                Id = 1, 
                Name = "HR", 
                Code = "HR", 
                Employees = new List<Employee> { new Employee { Id = 1, FirstName = "John" } } 
            };
            _mockRepo.Setup(r => r.GetDepartmentWithDetailsAsync(1)).ReturnsAsync(dept);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(1));
            Assert.Equal("Cannot delete a department that still has active employees.", ex.Message);
        }
    }
}
