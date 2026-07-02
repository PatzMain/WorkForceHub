using System;
using Xunit;
using EMS.Application.DTOs.Leave;
using EMS.Application.Validators.Leave;
using EMS.Domain.Enums;

namespace EMS.UnitTests.Validators
{
    public class LeaveRequestCreateDtoValidatorTests
    {
        private readonly LeaveRequestCreateDtoValidator _validator;

        public LeaveRequestCreateDtoValidatorTests()
        {
            _validator = new LeaveRequestCreateDtoValidator();
        }

        [Fact]
        public void Should_Fail_When_EndDate_Is_Before_StartDate()
        {
            var dto = new LeaveRequestCreateDto
            {
                EmployeeId = 1,
                LeaveType = LeaveType.Vacation,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(2), // End before Start
                Reason = "Vacation request"
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "StartDate");
        }

        [Fact]
        public void Should_Fail_When_Reason_Is_Empty()
        {
            var dto = new LeaveRequestCreateDto
            {
                EmployeeId = 1,
                LeaveType = LeaveType.Vacation,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                Reason = ""
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Reason");
        }

        [Fact]
        public void Should_Fail_When_EmployeeId_Is_Zero()
        {
            var dto = new LeaveRequestCreateDto
            {
                EmployeeId = 0, // Invalid
                LeaveType = LeaveType.Vacation,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                Reason = "Need rest."
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EmployeeId");
        }

        [Fact]
        public void Should_Succeed_When_Dto_Is_Valid()
        {
            var dto = new LeaveRequestCreateDto
            {
                EmployeeId = 1,
                LeaveType = LeaveType.Vacation,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                Reason = "Need rest."
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }
    }
}
