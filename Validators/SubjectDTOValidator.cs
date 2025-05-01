using FluentValidation;
using StudentAPI.DTOs;

namespace StudentAPI.Validators
{
    public class SubjectDTOValidator : AbstractValidator<SubjectDTO>
    {
        public SubjectDTOValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Subject code is required")
                .MaximumLength(10).WithMessage("Subject code cannot exceed 10 characters");
            
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(100).WithMessage("Description cannot exceed 100 characters");
        }
    }
}