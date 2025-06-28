using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    // Route: localhost:xxxx/api/users
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public string UserName { get; private set; }

        public UsersController(ApplicationDbContext dbContext, IUserService userService, ILogger<UsersController> logger)
        {
            this.dbContext = dbContext;
            _userService = userService;
            _logger = logger;

            _logger.LogInformation("UsersController initialized.");
        }

        // GET: api/users
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            _logger.LogInformation("Fetching all users.");
            var users = dbContext.Users.ToList();
            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public IActionResult AddUser([FromBody] AddUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddUser.");
                return BadRequest(ModelState);
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = dto.UserName,
                Password = dto.Password,
                UserEmail = dto.UserEmail,
                Phone = dto.Phone,
                Role = dto.Role
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            _logger.LogInformation("User created: {UserId}", user.Id);

            var result = new
            {
                Message = "User successfully created.",
                CreatedUser = user
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, result);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public IActionResult GetUserById(Guid id)
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);

            var user = dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateUser.");
                return BadRequest(ModelState);
            }

            var user = dbContext.Users.Find(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", id);
                return NotFound(new { error = "User not found." });
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                _logger.LogWarning("Unauthorized update attempt.");
                return Unauthorized(new { error = "Unauthorized access." });
            }

            try
            {
                _logger.LogInformation("User {CurrentUserId} attempting to update user {TargetUserId}.", currentUserId, id);

                _userService.UpdateUser(user, dto, currentUserId);
                dbContext.SaveChanges();

                _logger.LogInformation("User {TargetUserId} updated successfully.", id);

                return Ok(new { message = "User updated successfully.", updatedUser = user });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating user {UserId}.", id);
                return BadRequest(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access while updating user {UserId}.", id);
                return StatusCode(403, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user {UserId}.", id);
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

            var user = dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
                return NotFound(new { error = $"User with ID {id} not found." });
            }

            dbContext.Users.Remove(user);
            dbContext.SaveChanges();

            _logger.LogInformation("User with ID {UserId} deleted successfully.", id);
            return Ok(new { message = "User deleted!" });
        }
    }
}
