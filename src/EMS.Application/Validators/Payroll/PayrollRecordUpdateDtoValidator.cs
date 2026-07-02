using FluentValidation;
using EMS.Application.DTOs.Payroll;

namespace EMS.Application.Validators.Payroll
{
    public class PayrollRecordUpdateDtoValidator : AbstractValidator<PayrollRecordUpdateDto>
    {
        public PayrollRecordUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Payroll Record ID is required.");

            RuleFor(x => x.GrossPay)
                .GreaterThanOrEqualTo(0).WithMessage("Gross Pay must be positive.");

            RuleFor(x => x.Deductions)
                .GreaterThanOrEqualTo(0).WithMessage("Deductions must be positive.");
        }
    }
}
