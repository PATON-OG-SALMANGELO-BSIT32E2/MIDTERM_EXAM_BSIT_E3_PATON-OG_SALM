namespace StudentAPI.Models
{
    public class StudentSection
    {
        public int StudentId { get; set; }
        public int SectionId { get; set; }

        // Navigation properties
        public Student Student { get; set; } = null!;
        public Section Section { get; set; } = null!;
    }
}