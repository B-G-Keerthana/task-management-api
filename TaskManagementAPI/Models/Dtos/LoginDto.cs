namespace TaskManagementAPI.Models.Dtos
{
    // DTO used for user login requests
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
