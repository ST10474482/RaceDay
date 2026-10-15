using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // Organisers can view all results.
        // Participants can view only their own results.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
            {
                return Unauthorized("Please log in first.");
            }

            IQueryable<Result> query = _context.Results
                .Include(r => r.Enrollment)
                .Include(r => r.User)
                .Include(r => r.Organizer);

            if (role == "Participant")
            {
                query = query.Where(r => r.UserID == userId.Value);
            }
            else if (role != "Organizer")
            {
                return StatusCode(403, "You do not have permission to view results.");
            }

            return await query.ToListAsync();
        }

        // GET: api/Results/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
            {
                return Unauthorized("Please log in first.");
            }

            var result = await _context.Results
                .Include(r => r.Enrollment)
                .Include(r => r.User)
                .Include(r => r.Organizer)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound("Result not found.");
            }

            if (role == "Participant" && result.UserID != userId.Value)
            {
                return StatusCode(403, "You can only view your own results.");
            }

            if (role != "Participant" && role != "Organizer")
            {
                return StatusCode(403, "You do not have permission to view this result.");
            }

            return result;
        }

        // POST: api/Results
        // Only Organisers can create results.
        [HttpPost]
        public async Task<ActionResult<Result>> CreateResult(Result result)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
            {
                return Unauthorized("Please log in first.");
            }

            if (role != "Organizer")
            {
                return StatusCode(403, "Only Organisers can record results.");
            }

            var organizer = await _context.Organizers
                .FirstOrDefaultAsync(o => o.UserID == userId.Value);

            if (organizer == null)
            {
                return BadRequest("Your organiser profile was not found.");
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EnrollmentID == result.EnrollmentID);

            if (enrollment == null)
            {
                return BadRequest("The selected enrollment does not exist.");
            }

            bool resultExists = await _context.Results
                .AnyAsync(r => r.EnrollmentID == result.EnrollmentID);

            if (resultExists)
            {
                return Conflict("A result has already been recorded for this enrollment.");
            }

            // Derive these values from trusted server-side records.
            result.ResultID = 0;
            result.UserID = enrollment.UserID;
            result.OrganizerID = organizer.OrganizerID;
            result.Enrollment = null;
            result.User = null;
            result.Organizer = null;

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultID },
                result);
        }
    }
}