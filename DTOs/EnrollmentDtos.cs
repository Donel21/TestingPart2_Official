using System;

namespace TestingPart2_Official.DTOs
{
    public class EnrollmentCreateDto
    {
        public int CategoryID { get; set; }
    }

    public class EnrollmentResponseDto
    {
        public int EnrollmentID { get; set; }
        public int ParticipantID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}