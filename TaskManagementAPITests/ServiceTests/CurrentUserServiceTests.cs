using System.Security.Claims;
using TaskManagementAPI.Services;
namespace TaskManagementAPITests.ServiceTests
{
    public class CurrentUserServiceTests
    {
        [Fact]
        public void CurrentUserService_ReturnsUserIdAndRole_FromHttpContext()
        {
            // Arrange
            var userId = "user123";
            var role = "Admin";

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = httpContext
            };

            var currentUserService = new CurrentUserService(httpContextAccessor);

            // Act
            var actualUserId = currentUserService.UserId;
            var actualRole = currentUserService.Role;

            // Assert
            Assert.Equal(userId, actualUserId);
            Assert.Equal(role, actualRole);
        }
    }

}
