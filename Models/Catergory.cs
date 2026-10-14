using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingPart2_Official.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        [Required, MaxLength(100)]
        public string CategoryName { get; set; }

        public decimal DistanceKm { get; set; }   // DECIMAL(5,2)

        public decimal EntryFee { get; set; }     // DECIMAL(10,2)

        public Event Event { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}