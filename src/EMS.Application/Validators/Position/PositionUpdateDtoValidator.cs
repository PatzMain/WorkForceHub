using FluentValidation;
using EMS.Application.DTOs.Position;

namespace EMS.Application.Validators.Position
{
    public class PositionUpdateDtoValidator : AbstractValidator<PositionUpdateDto>
    {
        public PositionUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Position ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Position Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.BaseSalary)
                .GreaterThan(0).WithMessage("Base Salary must be greater than 0.");

            RuleFor(x => x.SalaryGrade)
                .NotEmpty().WithMessage("Salary Grade is required.")
                .MaximumLength(10).WithMessage("Salary Grade must not exceed 10 characters.");
        }
    }
}
