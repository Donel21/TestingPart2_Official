using System;

namespace TestingPart2_Official.DTOs
{
    public class ResultCreateDto
    {
        public int EnrollmentID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public DateTime? ResultDate { get; set; }
    }

    public class ResultUpdateDto
    {
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public DateTime? ResultDate { get; set; }
    }

    public class ResultResponseDto
    {
        public int ResultID { get; set; }
        public int EnrollmentID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? Position { get; set; }
        public DateTime ResultDate { get; set; }
    }
}