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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ResultsController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST /api/results  (Organiser who owns parent event)
        [HttpPost("results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] ResultCreateDto dto)
        {
            var enrollment = await _db.Enrollments
                .Include(e => e.Category)
                .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(e => e.EnrollmentID == dto.EnrollmentID);

            if (enrollment == null) return NotFound(new { message = "Enrollment not found." });
            if (enrollment.Category.Event.OrganiserID != CurrentUserId) return Forbid();

            bool exists = await _db.Results.AnyAsync(r => r.EnrollmentID == dto.EnrollmentID);
            if (exists)
                return Conflict(new { message = "Result already exists for this enrollment." });

            var result = new Result
            {
                EnrollmentID = dto.EnrollmentID,
                FinishTime = dto.FinishTime,
                Position = dto.Position,
                ResultDate = dto.ResultDate ?? DateTime.Today
            };

            _db.Results.Add(result);
            await _db.SaveChangesAsync();

            return StatusCode(201, new ResultResponseDto
            {
                ResultID = result.ResultID,
                EnrollmentID = result.EnrollmentID,
                FinishTime = result.FinishTime,
                Position = result.Position,
                ResultDate = result.ResultDate
            });
        }

        // GET /api/results/me  (Participant)
        [HttpGet("results/me")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> MyResults()
        {
            var list = await _db.Results
                .Where(r => r.Enrollment.ParticipantID == CurrentUserId)
                .Select(r => new ResultResponseDto
                {
                    ResultID = r.ResultID,
                    EnrollmentID = r.EnrollmentID,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    ResultDate = r.ResultDate
                })
                .ToListAsync();

            return Ok(list);
        }

        // GET /api/events/{eventId}/results  (Organiser who owns event)
        [HttpGet("events/{eventId:int}/results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> ForEvent(int eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });
            if (ev.OrganiserID != CurrentUserId) return Forbid();

            var list = await _db.Results
                .Where(r => r.Enrollment.Category.EventID == eventId)
                .Select(r => new ResultResponseDto
                {
                    ResultID = r.ResultID,
                    EnrollmentID = r.EnrollmentID,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    ResultDate = r.ResultDate
                })
                .ToListAsync();

            return Ok(list);
        }

        // PUT /api/results/{id}  (Organiser who owns event)
        [HttpPut("results/{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] ResultUpdateDto dto)
        {
            var result = await _db.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e.Category)
                .ThenInclude(c => c.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null) return NotFound();
            if (result.Enrollment.Category.Event.OrganiserID != CurrentUserId) return Forbid();

            result.FinishTime = dto.FinishTime;
            result.Position = dto.Position;
            if (dto.ResultDate.HasValue) result.ResultDate = dto.ResultDate.Value;

            await _db.SaveChangesAsync();

            return Ok(new ResultResponseDto
            {
                ResultID = result.ResultID,
                EnrollmentID = result.EnrollmentID,
                FinishTime = result.FinishTime,
                Position = result.Position,
                ResultDate = result.ResultDate
            });
        }
    }
}