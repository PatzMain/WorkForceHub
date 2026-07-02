using FluentValidation;
using EMS.Application.DTOs.Department;

namespace EMS.Application.Validators.Department
{
    public class DepartmentUpdateDtoValidator : AbstractValidator<DepartmentUpdateDto>
    {
        public DepartmentUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Department Code is required.")
                .MaximumLength(10).WithMessage("Code must not exceed 10 characters.");
        }
    }
}
