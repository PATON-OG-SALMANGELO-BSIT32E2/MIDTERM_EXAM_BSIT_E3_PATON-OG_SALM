using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.DTOs;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Subjects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            return await _context.Subjects.ToListAsync();
        }

        // GET: api/Subjects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await _context.Subjects
                .Include(s => s.Sections)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                return NotFound();

            return subject;
        }

        // POST: api/Subjects
        [HttpPost]
        public async Task<ActionResult<Subject>> CreateSubject(SubjectDTO subjectDto)
        {
            if (await _context.Subjects.AnyAsync(s => s.Code == subjectDto.Code))
            {
                return BadRequest("Subject with this code already exists.");
            }

            var subject = new Subject
            {
                Code = subjectDto.Code,
                Description = subjectDto.Description
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
        }

        // PUT: api/Subjects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, SubjectDTO subjectDto)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            // Check if code is being changed and if it already exists
            if (subject.Code != subjectDto.Code && 
                await _context.Subjects.AnyAsync(s => s.Code == subjectDto.Code))
            {
                return BadRequest("This code is already in use by another subject.");
            }

            subject.Code = subjectDto.Code;
            subject.Description = subjectDto.Description;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Subjects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}