namespace Expenses.Api.DTOs;

public record CreateAccountGroupRequest
{
    public string Name { get; init; } = null!;
}
