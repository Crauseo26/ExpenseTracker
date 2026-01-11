using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record CreateAccountCommand
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public Guid ExpenseGroupId { get; init; }
}

public record CreateAccountResult
{
    public bool Success { get; init; }
    public AccountDto? Account { get; init; }
    public string? ErrorMessage { get; init; }
}
