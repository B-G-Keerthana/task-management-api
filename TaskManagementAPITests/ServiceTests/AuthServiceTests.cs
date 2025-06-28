using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;
namespace TaskManagementAPITests.ServiceTests
{

    public class AuthServiceTests
    {
        private ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"AuthServiceTestDb_{Guid.NewGuid()}")
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
        public void Authenticate_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var userId = Guid.NewGuid();

            context.Users.Add(new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "testpass",
                UserEmail = "test@example.com",
                Phone = "1234567890",
                Role = "User"
            });
            context.SaveChanges();

            var config = CreateFakeJwtConfig();
            var authService = new AuthService(context, config);

            // Act
            var token = authService.Authenticate("testuser", "testpass");

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            // Optional: decode and inspect token
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Equal("testuser", jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.Equal("User", jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value);
            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Equal("TestAudience", jwt.Audiences.First());
        }

        [Fact]
        public void Authenticate_ReturnsNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var context = CreateInMemoryDbContext(); // no users
            var config = CreateFakeJwtConfig();
            var authService = new AuthService(context, config);

            // Act
            var token = authService.Authenticate("wronguser", "wrongpass");

            // Assert
            Assert.Null(token);
        }
    }

}
