using System;
using FluentValidation;
using EMS.Application.DTOs.Payroll;

namespace EMS.Application.Validators.Payroll
{
    public class PayrollRecordCreateDtoValidator : AbstractValidator<PayrollRecordCreateDto>
    {
        public PayrollRecordCreateDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Valid Employee ID is required.");

            RuleFor(x => x.PayPeriodStart)
                .NotEmpty().WithMessage("Start Date is required.")
                .LessThanOrEqualTo(x => x.PayPeriodEnd).WithMessage("Start Date must be before or equal to End Date.");

            RuleFor(x => x.PayPeriodEnd)
                .NotEmpty().WithMessage("End Date is required.");

            RuleFor(x => x.GrossPay)
                .GreaterThanOrEqualTo(0).WithMessage("Gross Pay must be positive.");

            RuleFor(x => x.Deductions)
                .GreaterThanOrEqualTo(0).WithMessage("Deductions must be positive.");
        }
    }
}
