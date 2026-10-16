
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Data;
using RaceDayWebAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace RaceDayWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return Unauthorized(new { message = "User not logged in." });

            var user = await _context.Users.Include(u => u.Role)
                .Where(u => u.UserID == userId)
                .Select(u => new { u.UserID, u.FirstName, u.LastName, u.Email, u.Phone, Role = u.Role != null ? u.Role.RoleName : "" })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return Unauthorized(new { message = "User not logged in." });

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully." });
        }
    }

    public class UpdateProfileDto
    {
        [Required] public string FirstName { get; set; } = string.Empty;
        [Required] public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
