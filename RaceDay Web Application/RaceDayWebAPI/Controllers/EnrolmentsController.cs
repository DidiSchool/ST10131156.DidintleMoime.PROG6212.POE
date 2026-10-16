
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Data;
using RaceDayWebAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayWebAPI.Controllers
{
    [Route("api/enrolments")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Enrol([FromBody] EnrolmentDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null) return Unauthorized(new { message = "Authentication required." });
            if (role != "Participant") return StatusCode(403, new { message = "Only Participants can enrol in events." });

            if (!await _context.Categories.AnyAsync(c => c.CategoryID == dto.CategoryID))
                return NotFound(new { message = "Category not found." });

            if (await _context.Enrolments.AnyAsync(e => e.ParticipantID == userId && e.CategoryID == dto.CategoryID))
                return Conflict(new { message = "Participant is already enrolled in this category." });

            var enrolment = new Enrolment
            {
                ParticipantID = userId.Value,
                CategoryID = dto.CategoryID,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Successfully enrolled in event.", enrolmentId = enrolment.EnrolmentID });
        }
    }

    public class EnrolmentDto
    {
        [Required] public int CategoryID { get; set; }
    }
}
