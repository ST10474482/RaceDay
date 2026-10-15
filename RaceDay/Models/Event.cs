using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [StringLength(150)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int OrganizerID { get; set; }

        public int CategoryID { get; set; }

        public Organizer? Organizer { get; set; }

        public Category? Category { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();
    }
}