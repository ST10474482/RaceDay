using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Organizer
    {
        [Key]
        public int OrganizerID { get; set; }

        [Required]
        [StringLength(100)]
        public string OrganizationName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        public int UserID { get; set; }

        public User? User { get; set; }

        public ICollection<Event> Events { get; set; }
            = new List<Event>();

        public ICollection<Result> Results { get; set; }
            = new List<Result>();
    }
}