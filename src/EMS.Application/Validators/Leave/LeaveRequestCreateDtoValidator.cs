using System;
using FluentValidation;
using EMS.Application.DTOs.Leave;

namespace EMS.Application.Validators.Leave
{
    public class LeaveRequestCreateDtoValidator : AbstractValidator<LeaveRequestCreateDto>
    {
        public LeaveRequestCreateDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Valid Employee ID is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start Date is required.")
                .Must(BeAValidDate).WithMessage("Start Date must not be default.")
                .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start Date must be before or equal to End Date.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End Date is required.")
                .Must(BeAValidDate).WithMessage("End Date must not be default.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }

        private bool BeAValidDate(DateTime date)
        {
            return date != default;
        }
    }
}
