using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using EMS.Application.Common;
using EMS.Application.DTOs.Position;
using EMS.Application.Interfaces;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Entities;

namespace EMS.UnitTests.Services
{
    public class PositionServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IPositionRepository> _mockRepo;
        private readonly PositionService _service;

        public PositionServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PositionProfile>());
            _mapper = config.CreateMapper();
            _mockRepo = new Mock<IPositionRepository>();
            _service = new PositionService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsPositionDto()
        {
            // Arrange
            var position = new Position { Id = 1, Title = "Software Engineer", BaseSalary = 5000, SalaryGrade = "6" };
            _mockRepo.Setup(r => r.GetPositionWithDetailsAsync(1)).ReturnsAsync(position);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Software Engineer", result.Title);
            Assert.Equal(5000, result.BaseSalary);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ThrowsNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetPositionWithDetailsAsync(99)).ReturnsAsync((Position?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_WithActiveEmployees_ThrowsException()
        {
            // Arrange
            var position = new Position
            {
                Id = 1,
                Title = "Software Engineer",
                SalaryGrade = "6",
                Employees = new List<Employee> { new Employee { Id = 1, FirstName = "John" } }
            };
            _mockRepo.Setup(r => r.GetPositionWithDetailsAsync(1)).ReturnsAsync(position);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(1));
            Assert.Equal("Cannot delete a position that is currently assigned to employees.", ex.Message);
        }

        [Fact]
        public async Task CreateAsync_Succeeds()
        {
            var dto = new PositionCreateDto { Title = "Manager", BaseSalary = 8000, SalaryGrade = "10" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Position>())).ReturnsAsync((Position p) => p);
            _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Manager", result.Title);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllPositions()
        {
            // Arrange
            var positions = new List<Position>
            {
                new Position { Id = 1, Title = "Engineer", SalaryGrade = "6" },
                new Position { Id = 2, Title = "Manager", SalaryGrade = "9" }
            };
            _mockRepo.Setup(r => r.GetAllPositionsWithDetailsAsync()).ReturnsAsync(positions);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}
