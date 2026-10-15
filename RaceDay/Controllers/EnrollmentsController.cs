using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrollments
        // Participants see their own enrollments.
        // Organisers can see all enrollments.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetEnrollments()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            IQueryable<Enrollment> query = _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Event);

            if (role == "Participant")
            {
                query = query.Where(e => e.UserID == userId.Value);
            }
            else if (role != "Organizer")
            {
                return Forbid();
            }

            return await query.ToListAsync();
        }

        // GET: api/Enrollments/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Enrollment>> GetEnrollment(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrollmentID == id);

            if (enrollment == null)
                return NotFound("Enrollment not found.");

            if (role == "Participant" && enrollment.UserID != userId.Value)
                return Forbid();

            if (role != "Participant" && role != "Organizer")
                return Forbid();

            return enrollment;
        }

        // POST: api/Enrollments
        // A participant can enroll only themselves.
        [HttpPost]
        public async Task<ActionResult<Enrollment>> CreateEnrollment(
            Enrollment enrollment)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            if (role != "Participant")
                return Forbid("Only Participants can enroll in events.");

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == enrollment.EventID);

            if (!eventExists)
                return BadRequest("The selected event does not exist.");

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.UserID == userId.Value &&
                    e.EventID == enrollment.EventID);

            if (alreadyEnrolled)
                return Conflict("You are already enrolled in this event.");

            enrollment.EnrollmentID = 0;
            enrollment.UserID = userId.Value;
            enrollment.EnrollmentDate = DateTime.UtcNow;
            enrollment.User = null;
            enrollment.Event = null;
            enrollment.Result = null;

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrollment),
                new { id = enrollment.EnrollmentID },
                enrollment);
        }
    }
}