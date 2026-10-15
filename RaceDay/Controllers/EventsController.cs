using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using RaceDay.Models;

namespace RaceDay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .AsNoTracking()
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.Organizer)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
                return NotFound();

            return eventItem;
        }

        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event eventItem)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role == null)
                return Unauthorized("Please log in first.");

            if (role != "Organizer")
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    "Only Organizers can create events.");

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                eventItem);
        }
    }
}