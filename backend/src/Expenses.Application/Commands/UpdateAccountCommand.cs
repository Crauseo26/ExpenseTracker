using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record UpdateAccountCommand
{
    public Guid AccountId { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public Guid ExpenseGroupId { get; init; }
}

public record UpdateAccountResult
{
    public bool Success { get; init; }
    public AccountDto? Account { get; init; }
    public string? ErrorMessage { get; init; }
}
