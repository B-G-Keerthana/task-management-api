using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.Dtos
{
    // DTO used when creating a new user
    public class AddUserDto
    {
        // UserName is required and must not be empty
        [Required(ErrorMessage = "UserName is required.")]
        [MinLength(1, ErrorMessage = "UserName cannot be empty.")]
        public required string UserName { get; set; }

        // Password is required and must not be empty
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(1, ErrorMessage = "Password cannot be empty.")]
        public required string Password { get; set; }

        // UserEmail is required but not explicitly validated here
        public required string UserEmail { get; set; }

        // Phone is optional
        public string? Phone { get; set; }

        // Role is required and must not be empty
        [Required(ErrorMessage = "Role is required.")]
        [MinLength(1, ErrorMessage = "Role cannot be empty.")]
        public required string Role { get; set; }
    }
}
