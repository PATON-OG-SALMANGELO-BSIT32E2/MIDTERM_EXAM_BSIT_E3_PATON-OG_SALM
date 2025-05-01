using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentSectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StudentSectionsController(ApplicationDbContext context) => _context = context;

        public class StudentSectionLinkDTO
        {
            public int StudentId   { get; set; }
            public string SectionName { get; set; } = string.Empty;
        }

        // POST: api/StudentSections
        [HttpPost]
        public async Task<IActionResult> Link([FromBody] StudentSectionLinkDTO dto)
        {
            var student = await _context.Students.FindAsync(dto.StudentId);
            if (student == null) return NotFound("Student not found.");

            var section = await _context.Sections
                               .Include(s => s.Subject)
                               .FirstOrDefaultAsync(s => s.Name == dto.SectionName);
            if (section == null) return BadRequest("Section not found.");

            bool exists = await _context.StudentSections
                .AnyAsync(ss => ss.StudentId == dto.StudentId && ss.SectionId == section.Id);
            if (exists) return BadRequest("Student already in that section.");

            _context.StudentSections.Add(new StudentSection {
                StudentId = dto.StudentId,
                SectionId = section.Id
            });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
