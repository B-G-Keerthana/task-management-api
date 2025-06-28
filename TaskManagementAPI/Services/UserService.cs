using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;

namespace TaskManagementAPI.Services
{
    // Interface defining user-related operations
    public interface IUserService
    {
        void UpdateUser(User user, UpdateUserDto dto, string role);
    }

    // Implementation of user update logic with role-based restrictions
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Updates user fields based on the current user's role
        public void UpdateUser(User user, UpdateUserDto dto, string currentUserId)
        {
            // Retrieve the current user from the database
            var currentUser = _db.Users.FirstOrDefault(u => u.Id.ToString() == currentUserId);
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Current user not found.");
            }

            var isAdmin = currentUser.Role == "Admin";

            // Restrict regular users from updating username and password fields
            if (!isAdmin && (!string.IsNullOrWhiteSpace(dto.UserName) || !string.IsNullOrWhiteSpace(dto.Role)))
            {
                throw new InvalidOperationException("You are not allowed to update UserName or Role.");
            }

            // Admins can update UserName and Role
            if (isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(dto.UserName))
                    user.UserName = dto.UserName;

                if (!string.IsNullOrWhiteSpace(dto.Role))
                    user.Role = dto.Role;
            }

            // All users can update these fields
            if (!string.IsNullOrWhiteSpace(dto.Password))
                user.Password = dto.Password;

            if (!string.IsNullOrWhiteSpace(dto.UserEmail))
                user.UserEmail = dto.UserEmail;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                user.Phone = dto.Phone;
        }
    }
}
