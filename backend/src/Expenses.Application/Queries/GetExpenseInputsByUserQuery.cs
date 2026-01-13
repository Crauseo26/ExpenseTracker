using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetExpenseInputsByUserQuery
{
    public Guid UserId { get; init; }
    public bool PendingOnly { get; init; }
}

public record GetExpenseInputsByUserResult
{
    public bool Success { get; init; }
    public IEnumerable<ExpenseInputDto> ExpenseInputs { get; init; } = Array.Empty<ExpenseInputDto>();
    public string? ErrorMessage { get; init; }
}
