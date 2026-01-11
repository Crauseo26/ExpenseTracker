using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetExpensesByUserQuery
{
    public Guid UserId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public record GetExpensesByUserResult
{
    public bool Success { get; init; }
    public IEnumerable<ExpenseDto> Expenses { get; init; } = Array.Empty<ExpenseDto>();
    public string? ErrorMessage { get; init; }
}
