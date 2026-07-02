using System;
using FluentValidation;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Validators.Employee
{
    public class EmployeeCreateDtoValidator : AbstractValidator<EmployeeCreateDto>
    {
        public EmployeeCreateDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First Name is required.")
                .MaximumLength(50).WithMessage("First Name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last Name is required.")
                .MaximumLength(50).WithMessage("Last Name must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.EmployeeNumber)
                .NotEmpty().WithMessage("Employee Number is required.")
                .MaximumLength(20).WithMessage("Employee Number must not exceed 20 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of Birth is required.")
                .Must(BeAtLeast18).WithMessage("Employee must be at least 18 years old.");

            RuleFor(x => x.DateJoined)
                .NotEmpty().WithMessage("Date Joined is required.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Please select a valid Department.");

            RuleFor(x => x.PositionId)
                .GreaterThan(0).WithMessage("Please select a valid Position.");
        }

        private bool BeAtLeast18(DateTime dateOfBirth)
        {
            return dateOfBirth <= DateTime.Today.AddYears(-18);
        }
    }
}
