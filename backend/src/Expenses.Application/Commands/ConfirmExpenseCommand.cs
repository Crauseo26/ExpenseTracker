using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record ConfirmExpenseCommand
{
    public Guid ExpenseId { get; init; }
    public Guid UserId { get; init; }
}

public record ConfirmExpenseResult
{
    public bool Success { get; init; }
    public ExpenseDto? Expense { get; init; }
    public string? ErrorMessage { get; init; }
}
