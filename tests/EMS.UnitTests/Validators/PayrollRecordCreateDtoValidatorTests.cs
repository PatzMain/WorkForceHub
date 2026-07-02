using System;
using Xunit;
using EMS.Application.DTOs.Payroll;
using EMS.Application.Validators.Payroll;

namespace EMS.UnitTests.Validators
{
    public class PayrollRecordCreateDtoValidatorTests
    {
        private readonly PayrollRecordCreateDtoValidator _validator;

        public PayrollRecordCreateDtoValidatorTests()
        {
            _validator = new PayrollRecordCreateDtoValidator();
        }

        [Fact]
        public void Should_Fail_When_GrossPay_Is_Negative()
        {
            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = 1,
                PayPeriodStart = DateTime.Today.AddDays(-30),
                PayPeriodEnd = DateTime.Today,
                GrossPay = -500, // Negative gross
                Deductions = 100,
                Notes = "Monthly pay"
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "GrossPay");
        }

        [Fact]
        public void Should_Fail_When_Deductions_Are_Negative()
        {
            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = 1,
                PayPeriodStart = DateTime.Today.AddDays(-30),
                PayPeriodEnd = DateTime.Today,
                GrossPay = 2000,
                Deductions = -50, // Negative deductions
                Notes = "Monthly pay"
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Deductions");
        }

        [Fact]
        public void Should_Fail_When_PayPeriodEnd_Is_Before_Start()
        {
            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = 1,
                PayPeriodStart = DateTime.Today,
                PayPeriodEnd = DateTime.Today.AddDays(-5), // End before Start
                GrossPay = 2000,
                Deductions = 100,
                Notes = "Monthly pay"
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "PayPeriodStart");
        }

        [Fact]
        public void Should_Succeed_When_Dto_Is_Valid()
        {
            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = 1,
                PayPeriodStart = DateTime.Today.AddDays(-30),
                PayPeriodEnd = DateTime.Today,
                GrossPay = 3000,
                Deductions = 200,
                Notes = "Standard pay."
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }
    }
}
