using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services
{
    // Interface defining task-related operations
    public interface ITaskService
    {
        IEnumerable<TaskItem> GetAll();
        TaskItem? Get(int id);
        void Create(TaskItem item);
        void UpdateAsAdmin(int taskId, TaskUpdateDto dto);
        void UpdateAsUser(int taskId, TaskUpdateDto dto, string userId);
        void Delete(int id);
    }

    // Implementation of task-related operations
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _user;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ApplicationDbContext db, ICurrentUserService user, ILogger<TaskService> logger)
        {
            _db = db;
            _user = user;
            _logger = logger;
            _logger.LogInformation("TaskService initialized for user {UserId} with role {Role}.", _user.UserId, _user.Role);
        }

        // Returns all tasks (Admin only)
        public IEnumerable<TaskItem> GetAll()
        {
            _logger.LogInformation("Fetching all tasks.");
            return _db.Tasks.ToList();
        }

        // Returns a task by ID if the user is Admin or the task owner
        public TaskItem? Get(int id)
        {
            var task = _db.Tasks.Find(id);
            if (_user.Role == "Admin" || task?.UserId == _user.UserId)
            {
                _logger.LogInformation("Task {TaskId} retrieved by user {UserId}.", id, _user.UserId);
                return task;
            }

            _logger.LogWarning("Access denied to task {TaskId} for user {UserId}.", id, _user.UserId);
            return null;
        }

        // Allows Admin to update all fields of a task
        public void UpdateAsAdmin(int taskId, TaskUpdateDto dto)
        {
            var task = _db.Tasks.Find(taskId);
            if (task == null)
            {
                _logger.LogWarning("Admin attempted to update non-existent task {TaskId}.", taskId);
                throw new KeyNotFoundException("Task not found");
            }

            task.TaskName = dto.TaskName;
            task.Description = dto.Description;
            task.Status = dto.Status;

            _db.SaveChanges();
            _logger.LogInformation("Admin updated task {TaskId}.", taskId);
        }

        // Allows a user to update only the status of their own task
        public void UpdateAsUser(int taskId, TaskUpdateDto dto, string userId)
        {
            var task = _db.Tasks.Find(taskId);
            if (task == null)
            {
                _logger.LogWarning("User {UserId} attempted to update non-existent task {TaskId}.", userId, taskId);
                throw new KeyNotFoundException("Task not found");
            }

            if (task.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to update task {TaskId} they do not own.", userId, taskId);
                throw new UnauthorizedAccessException("You do not own this task.");
            }

            if (!string.IsNullOrWhiteSpace(dto.TaskName) || !string.IsNullOrWhiteSpace(dto.Description))
            {
                _logger.LogWarning("User {UserId} attempted to update restricted fields on task {TaskId}.", userId, taskId);
                throw new InvalidOperationException("You are not allowed to update task name or description.");
            }

            task.Status = dto.Status;

            _db.SaveChanges();
            _logger.LogInformation("User {UserId} updated status of task {TaskId}.", userId, taskId);
        }

        // Creates a new task
        public void Create(TaskItem item)
        {
            if (string.IsNullOrWhiteSpace(item.TaskName))
            {
                _logger.LogWarning("Task creation failed: TaskName is required.");
                throw new ArgumentException("TaskName is required.");
            }

            if (string.IsNullOrWhiteSpace(item.Status))
            {
                _logger.LogWarning("Task creation failed: Status is required.");
                throw new ArgumentException("Status is required.");
            }

            if (string.IsNullOrWhiteSpace(item.UserId))
            {
                _logger.LogWarning("Task creation failed: Valid UserId is required.");
                throw new ArgumentException("Valid UserId is required.");
            }

            _db.Tasks.Add(item);
            _db.SaveChanges();
            _logger.LogInformation("Task created with ID {TaskId} by user {UserId}.", item.Id, item.UserId);
        }

        // Deletes a task by ID
        public void Delete(int id)
        {
            var task = _db.Tasks.Find(id);
            if (task != null)
            {
                _db.Tasks.Remove(task);
                _db.SaveChanges();
                _logger.LogInformation("Task {TaskId} deleted.", id);
            }
            else
            {
                _logger.LogWarning("Attempted to delete non-existent task {TaskId}.", id);
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
        }

    }
}
