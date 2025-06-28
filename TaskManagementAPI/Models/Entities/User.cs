namespace TaskManagementAPI.Models.Entities
{
    // Entity of User Table
    public class User
    {
        // Unique identifier for the user
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required String Password { get; set; }
        public required string UserEmail { get; set; }
        public string? Phone { get; set; }
        public required String Role { get; set; }

    }
}
