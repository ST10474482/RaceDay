using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        [Required]
        public TimeSpan ResultTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Position { get; set; }

        [StringLength(255)]
        public string? Remarks { get; set; }

        public int EnrollmentID { get; set; }

        public int UserID { get; set; }

        public int OrganizerID { get; set; }

        public Enrollment? Enrollment { get; set; }

        public User? User { get; set; }

        public Organizer? Organizer { get; set; }
    }
}