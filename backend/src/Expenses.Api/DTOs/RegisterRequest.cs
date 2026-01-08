namespace Expenses.Api.DTOs;

public record RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
