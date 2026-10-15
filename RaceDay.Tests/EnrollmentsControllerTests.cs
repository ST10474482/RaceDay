using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Controllers;
using RaceDay.Data;
using RaceDay.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EnrollmentsControllerTests
    {
        private static RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private static EnrollmentsController CreateController(
            RaceDayDbContext context,
            int? userId,
            string? role)
        {
            var controller = new EnrollmentsController(context);
            var httpContext = new DefaultHttpContext
            {
                Session = new TestSession()
            };

            if (userId.HasValue)
                httpContext.Session.SetInt32("UserID", userId.Value);

            if (role != null)
                httpContext.Session.SetString("Role", role);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task CreateEnrollment_NotLoggedIn_ReturnsUnauthorized()
        {
            using var context = CreateContext();
            var controller = CreateController(context, null, null);

            var result = await controller.CreateEnrollment(
                new Enrollment { EventID = 1 });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateEnrollment_Organizer_ReturnsForbidden()
        {
            using var context = CreateContext();
            var controller = CreateController(context, 1, "Organizer");

            var result = await controller.CreateEnrollment(
                new Enrollment { EventID = 1 });

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task CreateEnrollment_EventDoesNotExist_ReturnsBadRequest()
        {
            using var context = CreateContext();
            var controller = CreateController(context, 1, "Participant");

            var result = await controller.CreateEnrollment(
                new Enrollment { EventID = 99 });

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateEnrollment_Duplicate_ReturnsConflict()
        {
            using var context = CreateContext();

            context.Events.Add(new Event { EventID = 1 });

            context.Enrollments.Add(new Enrollment
            {
                EnrollmentID = 1,
                UserID = 2,
                EventID = 1
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context, 2, "Participant");

            var result = await controller.CreateEnrollment(
                new Enrollment { EventID = 1 });

            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateEnrollment_ValidParticipant_ReturnsCreated()
        {
            using var context = CreateContext();

            context.Events.Add(new Event { EventID = 1 });
            await context.SaveChangesAsync();

            var controller = CreateController(context, 2, "Participant");

            var result = await controller.CreateEnrollment(
                new Enrollment { EventID = 1 });

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, await context.Enrollments.CountAsync());

            var savedEnrollment = await context.Enrollments
                .FirstOrDefaultAsync();

            Assert.NotNull(savedEnrollment);
            Assert.Equal(2, savedEnrollment.UserID);
            Assert.Equal(1, savedEnrollment.EventID);
        }
    }
}