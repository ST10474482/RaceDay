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
    public class AuthControllerTests
    {
        private static RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        private static AuthController CreateController(
            RaceDayDbContext context)
        {
            var controller = new AuthController(context);

            var httpContext = new DefaultHttpContext
            {
                Session = new TestSession()
            };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public async Task Register_WithValidParticipant_Returns201()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "Test Participant",
                Email = "participant@example.com",
                Password = "Password123",
                PhoneNumber = "0712345678",
                Role = "Participant"
            };

            var result = await controller.Register(request);

            var response = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, response.StatusCode);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task Register_WithMissingName_Returns400()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "",
                Email = "participant@example.com",
                Password = "Password123",
                Role = "Participant"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(await context.Users.ToListAsync());
        }

        [Fact]
        public async Task Register_WithShortPassword_Returns400()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "Test Participant",
                Email = "participant@example.com",
                Password = "short",
                Role = "Participant"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithInvalidRole_Returns400()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "Test User",
                Email = "test@example.com",
                Password = "Password123",
                Role = "Admin"
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_Returns409()
        {
            using var context = CreateContext();

            context.Users.Add(new User
            {
                FullName = "Existing User",
                Email = "existing@example.com",
                PasswordHash = "existing-hash",
                PhoneNumber = "0711111111",
                Role = "Participant"
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "New User",
                Email = "existing@example.com",
                Password = "Password123",
                Role = "Participant"
            };

            var result = await controller.Register(request);

            Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(1, await context.Users.CountAsync());
        }

        [Fact]
        public async Task Register_OrganizerWithoutOrganization_Returns400()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.RegisterRequest
            {
                FullName = "Test Organizer",
                Email = "organizer@example.com",
                Password = "Password123",
                PhoneNumber = "0712345678",
                Role = "Organizer",
                OrganizationName = ""
            };

            var result = await controller.Register(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithValidCredentials_Returns200AndSetsSession()
        {
            using var context = CreateContext();

            var user = new User
            {
                FullName = "Test Participant",
                Email = "participant@example.com",
                PhoneNumber = "0712345678",
                Role = "Participant"
            };

            var hasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

            user.PasswordHash = hasher.HashPassword(
                user, "Password123");

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var request = new AuthController.LoginRequest
            {
                Email = "participant@example.com",
                Password = "Password123"
            };

            var result = await controller.Login(request);

            Assert.IsType<OkObjectResult>(result);

            Assert.Equal(
                user.UserID,
                controller.HttpContext.Session.GetInt32("UserID"));

            Assert.Equal(
                "Participant",
                controller.HttpContext.Session.GetString("Role"));

            Assert.Equal(
                "Test Participant",
                controller.HttpContext.Session.GetString("FullName"));
        }

        [Fact]
        public async Task Login_WithWrongPassword_Returns401()
        {
            using var context = CreateContext();

            var user = new User
            {
                FullName = "Test Participant",
                Email = "participant@example.com",
                PhoneNumber = "0712345678",
                Role = "Participant"
            };

            var hasher =
                new Microsoft.AspNetCore.Identity.PasswordHasher<User>();

            user.PasswordHash = hasher.HashPassword(
                user, "Password123");

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = CreateController(context);

            var request = new AuthController.LoginRequest
            {
                Email = "participant@example.com",
                Password = "WrongPassword123"
            };

            var result = await controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithUnknownEmail_Returns401()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            var request = new AuthController.LoginRequest
            {
                Email = "unknown@example.com",
                Password = "Password123"
            };

            var result = await controller.Login(request);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Logout_ClearsSessionAndReturns200()
        {
            using var context = CreateContext();
            var controller = CreateController(context);

            controller.HttpContext.Session.SetInt32("UserID", 1);
            controller.HttpContext.Session.SetString("Role", "Participant");

            var result = controller.Logout();

            Assert.IsType<OkObjectResult>(result);
            Assert.Null(
                controller.HttpContext.Session.GetInt32("UserID"));
            Assert.Null(
                controller.HttpContext.Session.GetString("Role"));
        }
    }
}