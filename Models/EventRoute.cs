using System.ComponentModel.DataAnnotations;

namespace TestingPart2_Official.Models
{
    public class EventRoute
    {
        [Key]
        public int RouteID { get; set; }

        public int EventID { get; set; }   // UNIQUE per ERD

        [Required, MaxLength(150)]
        public string RouteName { get; set; }

        [Required, MaxLength(150)]
        public string StartPoint { get; set; }

        [Required, MaxLength(150)]
        public string FinishPoint { get; set; }

        public decimal RouteDistanceKm { get; set; }   // DECIMAL(5,2)

        public Event Event { get; set; }
    }
}