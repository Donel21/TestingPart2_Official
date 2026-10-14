using System;

namespace TestingPart2_Official.Models
{
    public class Result
    {
        public int ResultID { get; set; }

        public int EnrollmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }   // NULL allowed per SQL

        public int? Position { get; set; }          // NULL allowed per SQL

        public DateTime ResultDate { get; set; } = DateTime.Today;

        public Enrollment Enrollment { get; set; }
    }
}