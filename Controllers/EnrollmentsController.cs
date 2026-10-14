using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPart2_Official.Data;
using TestingPart2_Official.DTOs;
using TestingPart2_Official.Models;

namespace TestingPart2_Official.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EnrollmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST /api/enrollments  (Participant)
        [HttpPost("enrollments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentCreateDto dto)
        {
            var category = await _db.Categories.FindAsync(dto.CategoryID);
            if (category == null) return NotFound(new { message = "Category not found." });

            bool already = await _db.Enrollments.AnyAsync(e =>
                e.ParticipantID == CurrentUserId && e.CategoryID == dto.CategoryID);
            if (already)
                return Conflict(new { message = "Already enrolled in this category." });

            var enrollment = new Enrollment
            {
                ParticipantID = CurrentUserId,
                CategoryID = dto.CategoryID,
                EnrollmentDate = DateTime.Today,
                Status = "Confirmed"
            };

            _db.Enrollments.Add(enrollment);
            await _db.SaveChangesAsync();

            return StatusCode(201, new EnrollmentResponseDto
            {
                EnrollmentID = enrollment.EnrollmentID,
                ParticipantID = enrollment.ParticipantID,
                CategoryID = enrollment.CategoryID,
                EnrollmentDate = enrollment.EnrollmentDate,
                Status = enrollment.Status
            });
        }

        // GET /api/enrollments/me  (Participant)
        [HttpGet("enrollments/me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyEnrollments()
        {
            var list = await _db.Enrollments
                .Where(e => e.ParticipantID == CurrentUserId)
                .Select(e => new EnrollmentResponseDto
                {
                    EnrollmentID = e.EnrollmentID,
                    ParticipantID = e.ParticipantID,
                    CategoryID = e.CategoryID,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET /api/events/{eventId}/enrollments  (Organiser who owns event)
        [HttpGet("events/{eventId:int}/enrollments")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> ForEvent(int eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            if (ev.OrganiserID != CurrentUserId) return Forbid();

            var list = await _db.Enrollments
                .Where(e => e.Category.EventID == eventId)
                .Select(e => new EnrollmentResponseDto
                {
                    EnrollmentID = e.EnrollmentID,
                    ParticipantID = e.ParticipantID,
                    CategoryID = e.CategoryID,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        // DELETE /api/enrollments/{id}  (Participant owns it)
        [HttpDelete("enrollments/{id:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Cancel(int id)
        {
            var enr = await _db.Enrollments.FindAsync(id);
            if (enr == null) return NotFound();

            if (enr.ParticipantID != CurrentUserId) return Forbid();

            _db.Enrollments.Remove(enr);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}