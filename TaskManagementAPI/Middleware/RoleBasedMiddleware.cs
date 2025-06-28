using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using TaskManagementAPI.Attributes;

namespace TaskManagementAPI.Middleware
{
    // Middleware to enforce role-based access control using the RequiredRolesAttribute
    public class RoleBasedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleBasedMiddleware> _logger;

        public RoleBasedMiddleware(RequestDelegate next, ILogger<RoleBasedMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            _logger.LogInformation("RoleBasedMiddleware initialized.");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var requiredRoles = endpoint?.Metadata.GetMetadata<RequiredRolesAttribute>()?.Roles;

            // If the endpoint requires specific roles
            if (requiredRoles is { Length: > 0 })
            {
                // Check if the user is authenticated
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    _logger.LogWarning("Unauthenticated request to a protected endpoint.");

                    // Attempt to check if the token is expired
                    var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
                    if (authHeader?.StartsWith("Bearer ") == true)
                    {
                        var token = authHeader["Bearer ".Length..];
                        var handler = new JwtSecurityTokenHandler();

                        if (handler.CanReadToken(token))
                        {
                            var jwt = handler.ReadJwtToken(token);
                            var exp = jwt.Payload.Exp;

                            if (exp is not null && DateTimeOffset.FromUnixTimeSeconds(exp.Value) < DateTimeOffset.UtcNow)
                            {
                                _logger.LogWarning("Token has expired.");
                                await WriteJsonError(context, StatusCodes.Status401Unauthorized, "Token has expired.");
                                return;
                            }
                        }
                    }

                    await WriteJsonError(context, StatusCodes.Status401Unauthorized, "Unauthorized.");
                    return;
                }

                // Extract user roles from claims
                var userRoles = context.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value);

                // Check if user has at least one of the required roles
                if (!requiredRoles.Intersect(userRoles).Any())
                {
                    _logger.LogWarning("Access denied. User roles: {UserRoles}, Required roles: {RequiredRoles}",
                        string.Join(", ", userRoles), string.Join(", ", requiredRoles));

                    await WriteJsonError(context, StatusCodes.Status403Forbidden, "Forbidden: role mismatch, Only Admin can access!");
                    return;
                }

                _logger.LogInformation("Access granted. User roles matched required roles.");
            }

            // Proceed to the next middleware
            await _next(context);
        }

        // Helper method to write a JSON error response
        private static async Task WriteJsonError(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted) return;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(json);
        }
    }
}
