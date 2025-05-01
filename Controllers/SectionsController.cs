using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAPI.DTOs;
using StudentAPI.Models;

namespace StudentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Sections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Section>>> GetSections()
        {
            return await _context.Sections
                .Include(s => s.Subject)
                .ToListAsync();
        }

        // GET: api/Sections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Section>> GetSection(int id)
        {
            var section = await _context.Sections
                .Include(s => s.Subject)
                .Include(s => s.StudentSections)
                    .ThenInclude(ss => ss.Student)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (section == null)
                return NotFound();

            return section;
        }

        // POST: api/Sections
        // Accepts SectionDTO { Name, SubjectId } but if SubjectId==0 will read subjectCode from JSON body
        [HttpPost]
        public async Task<ActionResult<Section>> CreateSection([FromBody] SectionDTO sectionDto)
        {
            int subjectId = sectionDto.SubjectId;

            // if no SubjectId provided, try to read subjectCode from raw JSON
            if (subjectId == 0)
            {
                using var doc = JsonDocument.Parse(Request.Body);
                if (doc.RootElement.TryGetProperty("subjectCode", out var codeProp))
                {
                    var code = codeProp.GetString();
                    var subj = await _context.Subjects.FirstOrDefaultAsync(s => s.Code == code);
                    if (subj == null) return BadRequest($"Subject code '{code}' not found.");
                    subjectId = subj.Id;
                }
            }

            if (!await _context.Subjects.AnyAsync(s => s.Id == subjectId))
                return BadRequest("Invalid subject.");

            var section = new Section
            {
                Name = sectionDto.Name,
                SubjectId = subjectId
            };

            _context.Sections.Add(section);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSection), new { id = section.Id }, section);
        }

        // PUT: api/Sections/5
        // Same dual-mode: uses sectionDto.SubjectId if set, otherwise reads subjectCode
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSection(int id, [FromBody] SectionDTO sectionDto)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            int subjectId = sectionDto.SubjectId;
            if (subjectId == 0)
            {
                using var doc = JsonDocument.Parse(Request.Body);
                if (doc.RootElement.TryGetProperty("subjectCode", out var codeProp))
                {
                    var code = codeProp.GetString();
                    var subj = await _context.Subjects.FirstOrDefaultAsync(s => s.Code == code);
                    if (subj == null) return BadRequest($"Subject code '{code}' not found.");
                    subjectId = subj.Id;
                }
            }

            if (!await _context.Subjects.AnyAsync(s => s.Id == subjectId))
                return BadRequest("Invalid subject.");

            section.Name = sectionDto.Name;
            section.SubjectId = subjectId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Sections/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
