using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingPart2_Official.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Participant";

        public ICollection<Event> Events { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}