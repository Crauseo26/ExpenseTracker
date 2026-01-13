using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetExpenseInputByIdQuery
{
    public Guid ExpenseInputId { get; init; }
    public Guid UserId { get; init; }
}

public record GetExpenseInputByIdResult
{
    public bool Success { get; init; }
    public ExpenseInputDto? ExpenseInput { get; init; }
    public string? ErrorMessage { get; init; }
}
