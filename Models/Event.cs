using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestingPart2_Official.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        public int OrganiserID { get; set; }

        [Required, MaxLength(150)]
        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        [Required, MaxLength(150)]
        public string Location { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }   // NULL allowed per SQL

        public User Organiser { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<EventRoute> Routes { get; set; }
    }
}