using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace StudentAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Navigation property
        public ICollection<StudentSection> StudentSections { get; set; } = new List<StudentSection>();
    }
}