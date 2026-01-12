using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record UpdateAccountGroupCommand
{
    public Guid AccountGroupId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
}

public record UpdateAccountGroupResult
{
    public bool Success { get; init; }
    public AccountGroupDto? AccountGroup { get; init; }
    public string? ErrorMessage { get; init; }
}
