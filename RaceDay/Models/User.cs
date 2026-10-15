using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organizer|Participant)$")]
        public string Role { get; set; } = "Participant";

        public ICollection<Enrollment> Enrollments { get; set; }
            = new List<Enrollment>();
    }
}