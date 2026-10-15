
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.UserID,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Role
            });
        }

        public class UpdateProfileRequest
        {
            public string FullName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string? NewPassword { get; set; }
        }

        // PUT: api/Profile
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(
            UpdateProfileRequest request)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("Please log in first.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return BadRequest("Full name is required.");
            }

            user.FullName = request.FullName.Trim();
            user.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                if (request.NewPassword.Length < 8)
                {
                    return BadRequest(
                        "New password must contain at least 8 characters.");
                }

                user.PasswordHash = _passwordHasher.HashPassword(
                    user, request.NewPassword);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully.",
                user.UserID,
                user.FullName,
                user.Email,
                user.PhoneNumber,
                user.Role
            });
        }
    }
}
