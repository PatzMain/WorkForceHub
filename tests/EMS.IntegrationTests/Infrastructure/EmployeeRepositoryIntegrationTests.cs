using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Infrastructure.Data;
using EMS.Infrastructure.Repositories;

namespace EMS.IntegrationTests.Infrastructure
{
    public class EmployeeRepositoryIntegrationTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public EmployeeRepositoryIntegrationTests()
        {
            _dbPath = $"employee_repo_test_{Guid.NewGuid()}.db";
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={_dbPath}")
                .Options;

            // Ensure database is clean
            using var context = new ApplicationDbContext(_options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            // Clean up the SQLite file
            if (File.Exists(_dbPath))
            {
                try
                {
                    File.Delete(_dbPath);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Fact]
        public async Task AddAsync_ThenGetByIdAsync_ReturnsEmployee()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repo = new EmployeeRepository(context);

            var dept = new Department { Name = "R&D", Code = "RD" };
            var pos = new Position { Title = "Scientist", BaseSalary = 6000, SalaryGrade = "7" };
            context.Departments.Add(dept);
            context.Positions.Add(pos);
            await context.SaveChangesAsync();

            var employee = new Employee
            {
                FirstName = "David",
                LastName = "Miller",
                Email = "david.miller@workforcehub.com",
                EmployeeNumber = "EMP-99999",
                DateOfBirth = DateTime.Today.AddYears(-30),
                DateJoined = DateTime.Today,
                Status = EmploymentStatus.Active,
                DepartmentId = dept.Id,
                PositionId = pos.Id
            };

            // Act
            await repo.AddAsync(employee);
            await repo.SaveChangesAsync();

            // Clear context tracking to force reload from DB
            context.ChangeTracker.Clear();

            using var verifyContext = new ApplicationDbContext(_options);
            var verifyRepo = new EmployeeRepository(verifyContext);
            var retrieved = await verifyRepo.GetByIdAsync(employee.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("David", retrieved.FirstName);
            Assert.Equal("Miller", retrieved.LastName);
            Assert.Equal("EMP-99999", retrieved.EmployeeNumber);
        }

        [Fact]
        public async Task SoftDelete_HidesEmployee_FromGetById()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repo = new EmployeeRepository(context);

            var dept = new Department { Name = "R&D", Code = "RD" };
            var pos = new Position { Title = "Scientist", BaseSalary = 6000, SalaryGrade = "7" };
            context.Departments.Add(dept);
            context.Positions.Add(pos);
            await context.SaveChangesAsync();

            var employee = new Employee
            {
                FirstName = "David",
                LastName = "Miller",
                Email = "david.miller@workforcehub.com",
                EmployeeNumber = "EMP-99999",
                DateOfBirth = DateTime.Today.AddYears(-30),
                DateJoined = DateTime.Today,
                Status = EmploymentStatus.Active,
                DepartmentId = dept.Id,
                PositionId = pos.Id
            };

            await repo.AddAsync(employee);
            await repo.SaveChangesAsync();

            // Act: Mark as deleted
            employee.IsDeleted = true;
            await repo.UpdateAsync(employee);
            await repo.SaveChangesAsync();

            // Clear context tracking
            context.ChangeTracker.Clear();

            // Assert: Query again - Global Query Filter should filter it out
            using var verifyContext = new ApplicationDbContext(_options);
            var verifyRepo = new EmployeeRepository(verifyContext);
            
            // Try fetching by ID
            var retrieved = await verifyRepo.GetByIdAsync(employee.Id);
            Assert.Null(retrieved);
        }
    }
}
