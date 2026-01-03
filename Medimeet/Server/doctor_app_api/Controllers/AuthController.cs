using doctor_app_api.Data;
using doctor_app_api.Dtos;
using doctor_app_api.Models;
using doctor_app_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace doctor_app_api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AuthController(AppDbContext db, IConfiguration cfg, IJwtService jwt) : Controller
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Name, email and password are required.");

            dto = dto with { Email = dto.Email.Trim().ToLower() };

            if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                return Conflict("Email already registered.");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var token = jwt.CreateToken(user, cfg);
            return Ok(new AuthResult(token, user.Id, user.FullName, user.Email, user.Role.ToString()));

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResult>> Login(LoginDto dto)
        {
            var email = dto.Email.Trim().ToLower();
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) return Unauthorized("Invalid credentials.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = jwt.CreateToken(user, cfg);
            return new AuthResult(token, user.Id, user.FullName, user.Email, user.Role.ToString());

        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue(ClaimTypes.Name) // fallback
                            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            // More reliable:
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            if (!Guid.TryParse(sub, out var userId))
                return Unauthorized();

            var user = await db.Users.FindAsync(userId);
            if (user is null) return Unauthorized();

            return Ok(new { user.Id, user.FullName, user.Email, Role = user.Role.ToString() });
        }
        // Path: Controllers/AuthController.cs
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await db.Users.Select(a=>a.Role).Distinct().ToListAsync();
            return Ok(roles);
        }

        public record RegisterDto(string FullName, string Email, string Password, UserRole Role);
        public record LoginDto(string Email, string Password);
        public record LoginResult(string Token, string Name, string Role);
    }
}
