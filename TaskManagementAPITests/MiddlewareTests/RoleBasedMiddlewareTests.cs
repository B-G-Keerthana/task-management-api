using Microsoft.AspNetCore.TestHost;
using System.Net;
using System.Security.Claims;
using TaskManagementAPI.Attributes;
using TaskManagementAPI.Middleware;
namespace TaskManagementAPITests.MiddlewareTests
{

    public class RoleBasedMiddlewareTests
    {
        [Fact]
        public async Task Middleware_AllowsAccess_WhenUserHasRequiredRole()
        {
            // Arrange
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddLogging();
                })
                .Configure(app =>
                {
                    app.UseRouting();

                    // Inject a fake authenticated user with the "Admin" role
                    app.Use(async (context, next) =>
                    {
                        var claims = new[]
                        {
                        new Claim(ClaimTypes.Name, "testuser"),
                        new Claim(ClaimTypes.Role, "Admin")
                        };
                        var identity = new ClaimsIdentity(claims, "TestAuth");
                        context.User = new ClaimsPrincipal(identity);

                        await next();
                    });

                    app.UseMiddleware<RoleBasedMiddleware>();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/admin", async context =>
                        {
                            await context.Response.WriteAsync("Welcome Admin!");
                        }).WithMetadata(new RequiredRolesAttribute("Admin"));
                    });
                });

            using var server = new TestServer(builder);
            var client = server.CreateClient();

            // Act
            var response = await client.GetAsync("/admin");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("Welcome Admin!", content);
        }
    }

}
