using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.Attributes;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _service;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService service, ILogger<TasksController> logger)
        {
            _service = service;
            _logger = logger;

            //Log controller initialization
            _logger.LogInformation("TasksController initialized.");
        }

        // POST: api/tasks
        [HttpPost]
        [RequiredRoles("Admin")]
        public IActionResult Create(TaskItem task)
        {
            _logger.LogInformation("Admin creating a new task: {@Task}", task);

            try
            {
                _service.Create(task);

                _logger.LogInformation("Task created successfully.");

                return Ok(new
                {
                    success = true,
                    message = "Task created successfully.",
                    taskId = task.Id
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Task creation failed due to invalid input.");

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating task.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred. Please try again later."
                });
            }
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        [RequiredRoles("Admin", "User")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation("Fetching task with ID: {TaskId}", id);

            var task = _service.Get(id);
            if (task != null)
            {
                _logger.LogInformation("Task found: {@Task}", task);
                return Ok(task);
            }

            _logger.LogWarning("Task with ID {TaskId} not found.", id);
            return NotFound("Id not found!");
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
            {
                _logger.LogWarning("Invalid update request for task ID {TaskId}.", id);
                return BadRequest("Invalid or missing status.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "user1";

            _logger.LogInformation("User {UserId} with role {Role} attempting to update task ID {TaskId}.", userId, role, id);

            try
            {
                if (role == "Admin")
                {
                    _service.UpdateAsAdmin(id, dto);
                    _logger.LogInformation("Admin updated task ID {TaskId}.", id);
                }
                else if (role == "User")
                {
                    _service.UpdateAsUser(id, dto, userId);
                    _logger.LogInformation("User {UserId} updated their task ID {TaskId}.", userId, id);
                }
                else
                {
                    _logger.LogWarning("Unauthorized role {Role} attempted to update task ID {TaskId}.", role, id);
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task ID {TaskId}.", id);
                return BadRequest(ex.Message);
            }

            return Ok("Task Updated!");
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        [RequiredRoles("Admin")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Admin attempting to delete task ID {TaskId}.", id);

            try
            {
                _service.Delete(id);
                _logger.LogInformation("Task ID {TaskId} deleted successfully.", id);
                return Ok(new
                {
                    success = true,
                    message = $"Task with ID {id} was successfully deleted."
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Task ID {TaskId} not found.", id);
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // GET: api/tasks
        [HttpGet]
        [RequiredRoles("Admin")]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Fetching all tasks.");
            var tasks = _service.GetAll();
            return Ok(tasks);
        }
    }
}
