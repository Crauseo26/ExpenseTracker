using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetExpenseByIdQuery
{
    public Guid ExpenseId { get; init; }
    public Guid UserId { get; init; }
}

public record GetExpenseByIdResult
{
    public bool Success { get; init; }
    public ExpenseDto? Expense { get; init; }
    public string? ErrorMessage { get; init; }
}
