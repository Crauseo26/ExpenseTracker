using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetAccountGroupByIdQuery
{
    public Guid AccountGroupId { get; init; }
    public Guid UserId { get; init; }
}

public record GetAccountGroupByIdResult
{
    public bool Success { get; init; }
    public AccountGroupDto? AccountGroup { get; init; }
    public string? ErrorMessage { get; init; }
}
