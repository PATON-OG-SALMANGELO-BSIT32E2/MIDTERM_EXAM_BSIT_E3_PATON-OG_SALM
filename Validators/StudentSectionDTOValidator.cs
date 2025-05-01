using FluentValidation;
using StudentAPI.DTOs;
using StudentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentAPI.Validators
{
    public class StudentSectionDTOValidator : AbstractValidator<StudentSectionDTO>
    {
        private readonly ApplicationDbContext _context;

        public StudentSectionDTOValidator(ApplicationDbContext context)
        {
            _context = context;

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Student ID must be greater than 0")
                .MustAsync(async (id, cancellation) => {
                    return await _context.Students.AnyAsync(s => s.Id == id);
                }).WithMessage("Student does not exist");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Section ID must be greater than 0")
                .MustAsync(async (id, cancellation) => {
                    return await _context.Sections.AnyAsync(s => s.Id == id);
                }).WithMessage("Section does not exist");

            RuleFor(x => x)
                .MustAsync(async (dto, cancellation) => {
                    // Get the subject ID of the section
                    var section = await _context.Sections.FindAsync(dto.SectionId);
                    if (section == null) return true; // Will be caught by the section validation

                    // Check if the student is already enrolled in another section with the same subject
                    var studentSections = await _context.StudentSections
                        .Where(ss => ss.StudentId == dto.StudentId)
                        .Select(ss => ss.SectionId)
                        .ToListAsync();

                    var sectionsWithSameSubject = await _context.Sections
                        .Where(s => s.SubjectId == section.SubjectId && studentSections.Contains(s.Id))
                        .AnyAsync();

                    return !sectionsWithSameSubject;
                }).WithMessage("Student cannot be enrolled in multiple sections with the same subject");
        }
    }
}