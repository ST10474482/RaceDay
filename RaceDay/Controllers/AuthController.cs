using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        public class RegisterRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Role { get; set; } = "Participant";
            public string? OrganizationName { get; set; }
            public string? ContactNumber { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Name, email and password are required.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest("Password must contain at least 8 characters.");
            }

            if (request.Role != "Organizer" &&
                request.Role != "Participant")
            {
                return BadRequest("Role must be Organizer or Participant.");
            }

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                return Conflict("An account with this email already exists.");
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim().ToLowerInvariant(),
                PhoneNumber = request.PhoneNumber,
                Role = request.Role
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);

            if (request.Role == "Organizer")
            {
                if (string.IsNullOrWhiteSpace(request.OrganizationName))
                {
                    return BadRequest("OrganizationName is required for an Organizer.");
                }

                _context.Organizers.Add(new Organizer
                {
                    User = user,
                    OrganizationName = request.OrganizationName,
                    ContactNumber = request.ContactNumber ?? request.PhoneNumber
                });
            }

            await _context.SaveChangesAsync();

            return StatusCode(201, new
            {
                message = "Registration successful.",
                user.UserID,
                user.FullName,
                user.Email,
                user.Role
            });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var verification = _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (verification == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("FullName", user.FullName);

            return Ok(new
            {
                message = "Login successful.",
                user.UserID,
                user.FullName,
                user.Email,
                user.Role
            });
        }

        // POST: api/Auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful." });
        }
    }
}