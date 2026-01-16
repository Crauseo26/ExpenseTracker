namespace Expenses.Api.DTOs;

public record UpdateAccountGroupRequest
{
    public string Name { get; init; } = null!;
}
