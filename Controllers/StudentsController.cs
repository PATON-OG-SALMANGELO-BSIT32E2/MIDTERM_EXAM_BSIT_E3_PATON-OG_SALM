using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.DTOs;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StudentsController(ApplicationDbContext context) => _context = context;

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(StudentDTO dto)
        {
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email))
                return BadRequest("Student with this email already exists.");

            var student = new Student {
                FirstName = dto.FirstName,
                LastName  = dto.LastName,
                Email     = dto.Email
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetStudents()
        {
            var list = await _context.Students
                .Include(s => s.StudentSections)
                    .ThenInclude(ss => ss.Section)
                        .ThenInclude(sec => sec.Subject)
                .Select(s => new {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    sectionName = s.StudentSections.Select(ss => ss.Section.Name).FirstOrDefault(),
                    subjectCode = s.StudentSections.Select(ss => ss.Section.Subject.Code).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET: api/Students/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetStudent(int id)
        {
            var item = await _context.Students
                .Include(s => s.StudentSections)
                    .ThenInclude(ss => ss.Section)
                        .ThenInclude(sec => sec.Subject)
                .Where(s => s.Id == id)
                .Select(s => new {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    sectionName = s.StudentSections.Select(ss => ss.Section.Name).FirstOrDefault(),
                    subjectCode = s.StudentSections.Select(ss => ss.Section.Subject.Code).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (item == null) return NotFound();
            return Ok(item);
        }

        // PUT: api/Students/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDTO dto)
        {
            // Ensure the student exists
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Check for duplicate email
            if (await _context.Students.AnyAsync(s => s.Email == dto.Email && s.Id != id))
            {
                return BadRequest("Student with this email already exists.");
            }

            // Update student fields
            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Email = dto.Email;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "There was a problem updating the student.");
            }

            return NoContent();
        }

        // DELETE: api/Students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
