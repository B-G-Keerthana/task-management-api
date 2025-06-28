using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    // Route: api/auth
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        // Constructor injection of the AuthService
        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var token = _auth.Authenticate(dto.Username, dto.Password);

            if (token == null)
            {
                return Unauthorized(new { error = "Invalid Credentials!" });
            }
            return Ok(new { token });
        }
    }
}
