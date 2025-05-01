using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace StudentAPI.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}