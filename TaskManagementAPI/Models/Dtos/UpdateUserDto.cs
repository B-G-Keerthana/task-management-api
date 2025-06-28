using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models.Dtos
{
    // DTO used for updating the user  details
    public class UpdateUserDto
    {
        public string? UserName { get; set; }     // Admin only
        public string? Role { get; set; }         // Admin only
        public string? Password { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? UserEmail { get; set; }

        public string? Phone { get; set; }

    }
}
