
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Data;
using RaceDayWebAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayWebAPI.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CaptureResult([FromBody] ResultDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Organiser") return StatusCode(403, new { message = "Only Organisers can capture results." });

            if (!await _context.Enrolments.AnyAsync(e => e.EnrolmentID == dto.EnrolmentID))
                return NotFound(new { message = "Enrolment record not found." });

            if (await _context.Results.AnyAsync(r => r.EnrolmentID == dto.EnrolmentID))
                return Conflict(new { message = "Result already recorded for this enrolment." });

            var result = new Result
            {
                EnrolmentID = dto.EnrolmentID,
                FinishTime = TimeSpan.Parse(dto.FinishTime),
                OverallPosition = dto.OverallPosition,
                CategoryPosition = dto.CategoryPosition
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Result recorded successfully.", resultId = result.ResultID });
        }

        [HttpGet("my-results")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("UserRole");

            if (userId == null) return Unauthorized(new { message = "Authentication required." });
            if (role != "Participant") return StatusCode(403, new { message = "Only Participants can view their own results." });

            var results = await _context.Results
                .Include(r => r.Enrolment)
                .ThenInclude(e => e!.Category)
                .ThenInclude(c => c!.Event)
                .Where(r => r.Enrolment != null && r.Enrolment.ParticipantID == userId)
                .ToListAsync();

            return Ok(results);
        }
    }

    public class ResultDto
    {
        [Required] public int EnrolmentID { get; set; }
        [Required] public string FinishTime { get; set; } = "00:00:00"; // format HH:mm:ss
        [Required] public int OverallPosition { get; set; }
        [Required] public int CategoryPosition { get; set; }
    }
}