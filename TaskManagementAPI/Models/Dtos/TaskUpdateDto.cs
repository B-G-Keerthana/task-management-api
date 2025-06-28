namespace TaskManagementAPI.Models.Dtos
{
    // DTO used for user Updating the Task requests
    public class TaskUpdateDto
    {
        public string? TaskName { get; set; }     // Admin only
        public string? Description { get; set; }  // Admin only
        public required string Status { get; set; }        // Required for both
    }

}
