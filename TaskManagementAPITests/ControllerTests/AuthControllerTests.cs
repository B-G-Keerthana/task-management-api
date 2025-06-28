using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPITests.ControllerTests
{
    public class AuthControllerTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"AuthTestDb_{System.Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        private IConfiguration CreateFakeJwtConfig()
        {
            var config = new Dictionary<string, string>
        {
            { "Jwt:Key", "Qwerty@12345@qwerty@12345qwerty@1234567" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();
        }

        [Fact]
        public void Login_ReturnsOk_WhenCredentialsAreValid()
        {
            var context = CreateInMemoryDbContext();
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Password = "testpass",
                UserEmail = "test@example.com",
                Phone = "1234567890",
                Role = "User"
            });
            context.SaveChanges();

            var config = CreateFakeJwtConfig();
            var authService = new AuthService(context, config);
            var controller = new AuthController(authService);

            var dto = new LoginDto { Username = "testuser", Password = "testpass" };

            var result = controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var tokenProperty = ok.Value.GetType().GetProperty("token");
            Assert.NotNull(tokenProperty); // ensure the property exists

            var token = tokenProperty.GetValue(ok.Value)?.ToString();
            Assert.False(string.IsNullOrWhiteSpace(token));

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

    }
}