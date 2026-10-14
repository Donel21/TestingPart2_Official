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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/events/{eventId}/categories  (public)
        [HttpGet("events/{eventId:int}/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            var list = await _db.Categories
                .Where(c => c.EventID == eventId)
                .Select(c => new CategoryResponseDto
                {
                    CategoryID = c.CategoryID,
                    EventID = c.EventID,
                    CategoryName = c.CategoryName,
                    DistanceKm = c.DistanceKm,
                    EntryFee = c.EntryFee
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST /api/events/{eventId}/categories  (Organiser, owns event)
        [HttpPost("events/{eventId:int}/categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> AddToEvent(int eventId, [FromBody] CategoryCreateDto dto)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound(new { message = "Event not found." });

            if (ev.OrganiserID != CurrentUserId) return Forbid();

            if (string.IsNullOrWhiteSpace(dto.CategoryName) ||
                dto.DistanceKm <= 0 || dto.EntryFee < 0)
            {
                return BadRequest(new { message = "Invalid category data." });
            }

            bool duplicate = await _db.Categories
                .AnyAsync(c => c.EventID == eventId && c.CategoryName == dto.CategoryName);
            if (duplicate)
                return BadRequest(new { message = "Category name already exists for this event." });

            var cat = new Category
            {
                EventID = eventId,
                CategoryName = dto.CategoryName,
                DistanceKm = dto.DistanceKm,
                EntryFee = dto.EntryFee
            };

            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();

            return StatusCode(201, new CategoryResponseDto
            {
                CategoryID = cat.CategoryID,
                EventID = cat.EventID,
                CategoryName = cat.CategoryName,
                DistanceKm = cat.DistanceKm,
                EntryFee = cat.EntryFee
            });
        }

        // PUT /api/categories/{id}  (Organiser who owns parent event)
        [HttpPut("categories/{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto dto)
        {
            var cat = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
            if (cat == null) return NotFound();

            if (cat.Event.OrganiserID != CurrentUserId) return Forbid();

            if (string.IsNullOrWhiteSpace(dto.CategoryName) ||
                dto.DistanceKm <= 0 || dto.EntryFee < 0)
            {
                return BadRequest(new { message = "Invalid category data." });
            }

            cat.CategoryName = dto.CategoryName;
            cat.DistanceKm = dto.DistanceKm;
            cat.EntryFee = dto.EntryFee;

            await _db.SaveChangesAsync();

            return Ok(new CategoryResponseDto
            {
                CategoryID = cat.CategoryID,
                EventID = cat.EventID,
                CategoryName = cat.CategoryName,
                DistanceKm = cat.DistanceKm,
                EntryFee = cat.EntryFee
            });
        }

        // DELETE /api/categories/{id}  (Organiser who owns parent event)
        [HttpDelete("categories/{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoryID == id);
            if (cat == null) return NotFound();

            if (cat.Event.OrganiserID != CurrentUserId) return Forbid();

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}