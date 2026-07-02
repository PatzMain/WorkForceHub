using System;
using Xunit;
using EMS.Application.DTOs.Employee;
using EMS.Application.Validators.Employee;

namespace EMS.UnitTests.Validators
{
    public class EmployeeCreateDtoValidatorTests
    {
        private readonly EmployeeCreateDtoValidator _validator;

        public EmployeeCreateDtoValidatorTests()
        {
            _validator = new EmployeeCreateDtoValidator();
        }

        [Fact]
        public void Should_Fail_When_FirstName_Is_Empty()
        {
            var dto = new EmployeeCreateDto
            {
                FirstName = "",
                LastName = "Doe",
                Email = "john.doe@workforcehub.com",
                EmployeeNumber = "EMP-00101",
                DateOfBirth = DateTime.Today.AddYears(-20),
                DateJoined = DateTime.Today,
                DepartmentId = 1,
                PositionId = 1
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void Should_Fail_When_Email_Is_Invalid()
        {
            var dto = new EmployeeCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "invalid-email",
                EmployeeNumber = "EMP-00101",
                DateOfBirth = DateTime.Today.AddYears(-20),
                DateJoined = DateTime.Today,
                DepartmentId = 1,
                PositionId = 1
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void Should_Fail_When_Underage()
        {
            var dto = new EmployeeCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@workforcehub.com",
                EmployeeNumber = "EMP-00101",
                DateOfBirth = DateTime.Today.AddYears(-15), // Under 18
                DateJoined = DateTime.Today,
                DepartmentId = 1,
                PositionId = 1
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "DateOfBirth");
        }

        [Fact]
        public void Should_Succeed_When_Dto_Is_Valid()
        {
            var dto = new EmployeeCreateDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@workforcehub.com",
                EmployeeNumber = "EMP-00101",
                DateOfBirth = DateTime.Today.AddYears(-25),
                DateJoined = DateTime.Today,
                DepartmentId = 1,
                PositionId = 1
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }
    }
}
