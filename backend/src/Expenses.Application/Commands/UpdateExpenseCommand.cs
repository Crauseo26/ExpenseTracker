using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record UpdateExpenseCommand
{
    public Guid ExpenseId { get; init; }
    public Guid UserId { get; init; }
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ExpenseType { get; init; } = null!;
    public DateTime PurchaseDate { get; init; }
}

public record UpdateExpenseResult
{
    public bool Success { get; init; }
    public ExpenseDto? Expense { get; init; }
    public string? ErrorMessage { get; init; }
}
