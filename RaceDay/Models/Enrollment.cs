using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        public DateTime EnrollmentDate { get; set; }
            = DateTime.Today;

        public int UserID { get; set; }

        public int EventID { get; set; }

        public User? User { get; set; }

        public Event? Event { get; set; }

        public Result? Result { get; set; }
    }
}