namespace TaskManagementAPI.Models.Entities
{
    // Entity for TaskItem table
    public class TaskItem
    {
        // Unique identifier for the task
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Pending";
        public required string userId { get; set; }
    }
}
