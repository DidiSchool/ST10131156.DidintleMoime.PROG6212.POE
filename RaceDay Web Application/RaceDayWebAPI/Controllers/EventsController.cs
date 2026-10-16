// Controllers/EventsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Data;
using RaceDayWebAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayWebAPI.Controllers
{
    [Route("api/events")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.Include(e => e.Organiser).ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var ev = await _context.Events.Include(e => e.Categories).Include(e => e.Organiser).FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null) return NotFound(new { message = "Event not found." });
            return Ok(ev);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null) return Unauthorized(new { message = "Authentication required." });
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can create events." });

            var ev = new Event
            {
                OrganiserID = userId.Value,
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Event created successfully", eventId = ev.EventID });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can update events." });

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            ev.Name = dto.Name;
            ev.Description = dto.Description;
            ev.Date = dto.Date;
            ev.Location = dto.Location;
            ev.Distance = dto.Distance;
            ev.EventType = dto.EventType;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Event updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can delete events." });

            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound(new { message = "Event not found." });

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Event deleted successfully." });
        }

        [HttpGet("{eventId}/categories")]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var categories = await _context.Categories.Where(c => c.EventID == eventId).ToListAsync();
            return Ok(categories);
        }

        [HttpPost("{eventId}/categories")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] CategoryDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can add categories." });

            if (!await _context.Events.AnyAsync(e => e.EventID == eventId))
                return NotFound(new { message = "Event not found." });

            var category = new Category
            {
                EventID = eventId,
                CategoryName = dto.CategoryName,
                MinAge = dto.MinAge,
                MaxAge = dto.MaxAge
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Category added successfully", categoryId = category.CategoryID });
        }

        [HttpGet("{eventId}/enrolments")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can view event enrolments." });

            var enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.Category != null && e.Category.EventID == eventId)
                .ToListAsync();

            return Ok(enrolments);
        }
    }

    public class CreateEventDto
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public DateTime Date { get; set; }
        [Required] public string Location { get; set; } = string.Empty;
        [Required] public decimal Distance { get; set; }
        [Required] public string EventType { get; set; } = string.Empty;
    }

    public class CategoryDto
    {
        [Required] public string CategoryName { get; set; } = string.Empty;
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
    }
}
