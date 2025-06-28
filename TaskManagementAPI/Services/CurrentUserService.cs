using System.Security.Claims;

namespace TaskManagementAPI.Services
{
    // Interface to abstract access to the current user's identity
    public interface ICurrentUserService
    {
        string UserId { get; }
        string Role { get; }
    }

    // Implementation that retrieves user information from the current HTTP context
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;

        // Constructor injection of IHttpContextAccessor to access the current HTTP context
        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        // Retrieves the current user's ID from the JWT claims
        public string UserId =>
            _context.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        // Retrieves the current user's role from the JWT claims
        public string Role =>
            _context.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value!;
    }
}
