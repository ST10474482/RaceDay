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
    public class ResultsControllerTests
    {
        private static RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private static ResultsController CreateController(
            RaceDayDbContext context,
            int? userId,
            string? role)
        {
            var controller = new ResultsController(context);

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
        public async Task CreateResult_Participant_ReturnsForbidden()
        {
            using var context = CreateContext();

            var controller = CreateController(
                context,
                1,
                "Participant");

            var result = await controller.CreateResult(new Result());

            var response = Assert.IsAssignableFrom<ObjectResult>(
                result.Result);

            Assert.Equal(403, response.StatusCode);
        }

        [Fact]
        public async Task CreateResult_UnauthenticatedUser_ReturnsUnauthorized()
        {
            using var context = CreateContext();

            var controller = CreateController(
                context,
                null,
                null);

            var result = await controller.CreateResult(new Result());

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}