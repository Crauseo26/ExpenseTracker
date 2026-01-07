using System.Security.Claims;
using Expenses.Api.Services;

namespace Expenses.Api.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuthenticationService authenticationService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        
        if (path.Contains("/auth/") || path.Contains("/weatherforecast"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Missing or invalid authorization header" });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var isValid = await authenticationService.ValidateTokenAsync(token);

        if (!isValid)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid or expired token" });
            return;
        }

        var userId = await authenticationService.GetUserIdFromTokenAsync(token);
        if (userId == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid token claims" });
            return;
        }

        context.Items["UserId"] = userId.Value;
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Bearer");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }
}
