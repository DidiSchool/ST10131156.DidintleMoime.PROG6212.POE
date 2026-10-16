// RaceDayWebAPI.Tests/AuthAndEventUnitTests.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayWebAPI.Controllers;
using RaceDayWebAPI.Data;
using RaceDayWebAPI.Models;
using Xunit;

namespace RaceDayWebAPI.Tests
{
    public class AuthAndEventUnitTests
    {
        private RaceDayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new RaceDayDbContext(options);
            context.Roles.Add(new Role { RoleID = 1, RoleName = "Organiser" });
            context.Roles.Add(new Role { RoleID = 2, RoleName = "Participant" });
            context.SaveChanges();
            return context;
        }

        [Fact]
        public async Task Register_UserSuccessfully_Returns201Created()
        {
            var context = GetInMemoryDbContext();
            var controller = new AuthController(context);

            var dto = new RegisterDto
            {
                FirstName = "Thabo",
                LastName = "Mokoena",
                Email = "thabo@gmail.com",
                Password = "Password123",
                RoleID = 2
            };

            var result = await controller.Register(dto);
            var createdResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task CreateEvent_AsParticipant_Returns403Forbidden()
        {
            var context = GetInMemoryDbContext();
            var controller = new EventsController(context);

            // Mock session as Participant
            var httpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            httpContext.Session = new TestSession();
            httpContext.Session.SetInt32("UserID", 1);
            httpContext.Session.SetString("UserRole", "Participant");
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            var eventDto = new CreateEventDto
            {
                Name = "Soweto Marathon",
                Description = "Run through Soweto",
                Date = DateTime.Now.AddDays(10),
                Location = "Soweto",
                Distance = 42.2m,
                EventType = "Run"
            };

            var result = await controller.CreateEvent(eventDto);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
        }
    }

    // Helper for session mocking in unit tests
    public class TestSession : Microsoft.AspNetCore.Http.ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStore = new();
        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _sessionStore.Keys;
        public void Clear() => _sessionStore.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStore.Remove(key);
        public void Set(string key, byte[] value) => _sessionStore[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _sessionStore.TryGetValue(key, out value!);
    }
}
