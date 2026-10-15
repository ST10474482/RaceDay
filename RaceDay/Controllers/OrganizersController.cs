using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public OrganizersController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Organizers
        // Only logged-in Organisers can view organiser profiles.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organizer>>> GetOrganizers()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            if (role != "Organizer")
                return Forbid();

            return await _context.Organizers
                .Include(o => o.User)
                .ToListAsync();
        }

        // GET: api/Organizers/5
        // An Organiser can view only their own profile.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Organizer>> GetOrganizer(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            if (role != "Organizer")
                return Forbid();

            var organizer = await _context.Organizers
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrganizerID == id);

            if (organizer == null)
                return NotFound("Organizer profile not found.");

            if (organizer.UserID != userId.Value)
                return Forbid();

            return organizer;
        }

        // POST: api/Organizers
        // Registration already creates an organiser profile.
        // This endpoint prevents duplicate profile creation.
        [HttpPost]
        public async Task<ActionResult<Organizer>> CreateOrganizer(
            Organizer organizer)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            if (role != "Organizer")
                return Forbid();

            var existingOrganizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.UserID == userId.Value);

            if (existingOrganizer != null)
                return Conflict("Your organiser profile already exists.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userId.Value);

            if (user == null)
                return NotFound("User account not found.");

            organizer.OrganizerID = 0;
            organizer.UserID = userId.Value;
            organizer.User = null;
            organizer.Events = new List<Event>();
            organizer.Results = new List<Result>();

            _context.Organizers.Add(organizer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOrganizer),
                new { id = organizer.OrganizerID },
                organizer);
        }
    }
}