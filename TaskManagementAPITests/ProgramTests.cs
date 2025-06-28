using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
namespace TaskManagementAPITests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SwaggerEndpoint_ReturnsOk()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
