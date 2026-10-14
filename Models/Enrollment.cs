using System;
using System.ComponentModel.DataAnnotations;

namespace TestingPart2_Official.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        public int ParticipantID { get; set; }

        public int CategoryID { get; set; }

        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Confirmed"; // Pending|Confirmed|Cancelled

        public User Participant { get; set; }

        public Category Category { get; set; }

        public Result? Result { get; set; }   // nullable one-to-one
    }
}