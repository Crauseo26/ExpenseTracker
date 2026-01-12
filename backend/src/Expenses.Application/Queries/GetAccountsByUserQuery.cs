using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetAccountsByUserQuery
{
    public Guid UserId { get; init; }
    public Guid? ExpenseGroupId { get; init; }
}

public record GetAccountsByUserResult
{
    public bool Success { get; init; }
    public IEnumerable<AccountDto> Accounts { get; init; } = Array.Empty<AccountDto>();
    public string? ErrorMessage { get; init; }
}
