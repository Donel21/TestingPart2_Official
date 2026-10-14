using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestingPart2_Official.Data;
using TestingPart2_Official.DTOs;

namespace TestingPart2_Official.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/users/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _db.Users.FindAsync(CurrentUserId);
            if (user == null) return NotFound();

            return Ok(new UserProfileDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            });
        }

        // PUT /api/users/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new { message = "All fields required." });
            }

            var user = await _db.Users.FindAsync(CurrentUserId);
            if (user == null) return NotFound();

            // Email uniqueness check (excluding self)
            bool emailTaken = await _db.Users
                .AnyAsync(u => u.Email == dto.Email && u.UserID != user.UserID);
            if (emailTaken)
                return BadRequest(new { message = "Email already in use." });

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            await _db.SaveChangesAsync();

            return Ok(new UserProfileDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}