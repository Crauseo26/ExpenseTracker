namespace Expenses.Application.Commands;

public record DeleteExpenseCommand
{
    public Guid ExpenseId { get; init; }
    public Guid UserId { get; init; }
}

public record DeleteExpenseResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
