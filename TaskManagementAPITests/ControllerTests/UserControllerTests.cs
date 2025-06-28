using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;
[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace TaskManagementAPITests.ControllerTests
{
    public class UsersControllerTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private ClaimsPrincipal CreateUserPrincipal(string userId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public void AddUser_ReturnsCreatedResult_WhenValid()
        {
            using var context = CreateDbContext();
            var mockService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UsersController>>();

            var controller = new UsersController(context, mockService.Object, mockLogger.Object);

            var dto = new AddUserDto
            {
                UserName = "john",
                Password = "pass123",
                UserEmail = "john@example.com",
                Phone = "1234567890",
                Role = "User"
            };

            var result = controller.AddUser(dto) as CreatedAtActionResult;

            Assert.NotNull(result);
            Assert.Equal("GetUserById", result.ActionName);
        }

        [Fact]
        public void GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            using var context = CreateDbContext();
            var mockService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UsersController>>();

            var controller = new UsersController(context, mockService.Object, mockLogger.Object);
            //var controller = new UsersController(context, mockService.Object);

            var result = controller.GetUserById(Guid.NewGuid());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateUser_ReturnsBadRequest_WhenUserTriesToUpdateRestrictedFields()
        {
            using var context = CreateDbContext();
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "original",
                Role = "User",
                Password = "pass",
                UserEmail = "email@example.com",
                Phone = "123"
            };
            context.Users.Add(user);
            context.SaveChanges();

            var mockService = new Mock<IUserService>();
            mockService.Setup(s => s.UpdateUser(It.IsAny<User>(), It.IsAny<UpdateUserDto>(), It.IsAny<string>()))
                       .Throws(new InvalidOperationException("You are not allowed to update UserName or Role."));
            var mockLogger = new Mock<ILogger<UsersController>>();

            var controller = new UsersController(context, mockService.Object, mockLogger.Object);
            //var controller = new UsersController(context, mockService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUserPrincipal(user.Id.ToString())
                }
            };

            var dto = new UpdateUserDto { UserName = "hacker" };

            var result = controller.UpdateUser(user.Id, dto) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("not allowed", result.Value.ToString());
        }

        [Fact]
        public void DeleteUser_ReturnsOk_WhenUserExists()
        {
            using var context = CreateDbContext();
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "deleteMe",
                Role = "User",
                Password = "pass",
                UserEmail = "email@example.com",
                Phone = "123"
            };
            context.Users.Add(user);
            context.SaveChanges();

            var mockService = new Mock<IUserService>();
            var mockLogger = new Mock<ILogger<UsersController>>();

            var controller = new UsersController(context, mockService.Object, mockLogger.Object);
            //var controller = new UsersController(context, mockService.Object);

            var result = controller.DeleteUser(user.Id) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }

}
