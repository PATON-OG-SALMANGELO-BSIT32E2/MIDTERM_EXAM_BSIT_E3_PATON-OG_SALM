using FluentValidation;
using StudentAPI.DTOs;

namespace StudentAPI.Validators
{
    public class SectionDTOValidator : AbstractValidator<SectionDTO>
    {
        public SectionDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Section name is required")
                .MaximumLength(50).WithMessage("Section name cannot exceed 50 characters");
            
            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID must be greater than 0");
        }
    }
}