using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetAccountGroupsByUserQuery
{
    public Guid UserId { get; init; }
}

public record GetAccountGroupsByUserResult
{
    public bool Success { get; init; }
    public IEnumerable<AccountGroupDto> AccountGroups { get; init; } = Array.Empty<AccountGroupDto>();
    public string? ErrorMessage { get; init; }
}
