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
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EventsController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int? CurrentUserId =>
            User.Identity?.IsAuthenticated == true
                ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                : (int?)null;

        private bool IsOrganiser =>
            User.IsInRole("Organiser");

        // GET /api/events  (public)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events
                .OrderBy(e => e.EventDate)
                .Select(e => new EventResponseDto
                {
                    EventID = e.EventID,
                    OrganiserID = e.OrganiserID,
                    EventName = e.EventName,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Description = e.Description
                })
                .ToListAsync();

            return Ok(events);
        }

        // GET /api/events/{id}  (public)
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _db.Events.FindAsync(id);
            if (e == null) return NotFound();

            return Ok(new EventResponseDto
            {
                EventID = e.EventID,
                OrganiserID = e.OrganiserID,
                EventName = e.EventName,
                EventDate = e.EventDate,
                Location = e.Location,
                Description = e.Description
            });
        }

        // POST /api/events  (Organiser only)
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] EventCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EventName) ||
                string.IsNullOrWhiteSpace(dto.Location))
            {
                return BadRequest(new { message = "EventName and Location required." });
            }

            var newEvent = new Event
            {
                OrganiserID = CurrentUserId!.Value,
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Description = dto.Description
            };

            _db.Events.Add(newEvent);
            await _db.SaveChangesAsync();

            return StatusCode(201, new EventResponseDto
            {
                EventID = newEvent.EventID,
                OrganiserID = newEvent.OrganiserID,
                EventName = newEvent.EventName,
                EventDate = newEvent.EventDate,
                Location = newEvent.Location,
                Description = newEvent.Description
            });
        }

        // PUT /api/events/{id}  (Organiser, must own the event)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
        {
            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return NotFound();

            if (existing.OrganiserID != CurrentUserId!.Value)
                return Forbid();

            if (string.IsNullOrWhiteSpace(dto.EventName) ||
                string.IsNullOrWhiteSpace(dto.Location))
            {
                return BadRequest(new { message = "EventName and Location required." });
            }

            existing.EventName = dto.EventName;
            existing.EventDate = dto.EventDate;
            existing.Location = dto.Location;
            existing.Description = dto.Description;

            await _db.SaveChangesAsync();

            return Ok(new EventResponseDto
            {
                EventID = existing.EventID,
                OrganiserID = existing.OrganiserID,
                EventName = existing.EventName,
                EventDate = existing.EventDate,
                Location = existing.Location,
                Description = existing.Description
            });
        }

        // DELETE /api/events/{id}  (Organiser, must own the event)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return NotFound();

            if (existing.OrganiserID != CurrentUserId!.Value)
                return Forbid();

            _db.Events.Remove(existing);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}