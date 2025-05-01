using System.Collections.Generic;


namespace StudentAPI.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SubjectId { get; set; }

        // Navigation properties
        public Subject Subject { get; set; } = null!;
        public ICollection<StudentSection> StudentSections { get; set; } = new List<StudentSection>();
    }
}