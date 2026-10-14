using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestingPart2_Official.Data;
using TestingPart2_Official.DTOs;
using TestingPart2_Official.Helpers;
using TestingPart2_Official.Models;

namespace TestingPart2_Official.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // POST /api/auth/register  → creates a Participant
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.FirstName) ||
                string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest(new { message = "All fields are required." });
            }

            bool emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                return BadRequest(new { message = "Email already exists." });
            }

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Participant" // register always creates Participant
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = JwtTokenGenerator.GenerateToken(user, _config);

            return StatusCode(201, new AuthResponseDto
            {
                UserID = user.UserID,
                Email = user.Email,
                Role = user.Role,
                Token = token
            });
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest(new { message = "Email and password required." });
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            bool valid;
            try
            {
                valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            }
            catch
            {
                // Fallback: seed users use plain-text passwords from the SQL script
                valid = dto.Password == user.PasswordHash;
            }

            if (!valid)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = JwtTokenGenerator.GenerateToken(user, _config);

            return Ok(new AuthResponseDto
            {
                UserID = user.UserID,
                Email = user.Email,
                Role = user.Role,
                Token = token
            });
        }
    }
}