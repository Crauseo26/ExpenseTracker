namespace Expenses.Api.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthenticationResult> RegisterAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Guid?> GetUserIdFromTokenAsync(string token, CancellationToken cancellationToken = default);
}

public record AuthenticationResult(bool Success, string? Token, Guid? UserId, string? ErrorMessage);
