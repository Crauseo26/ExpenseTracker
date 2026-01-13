using Expenses.Application.DTOs;

namespace Expenses.Application.Commands;

public record ProcessExpenseInputCommand
{
    public Guid UserId { get; init; }
    public string InputType { get; init; } = null!;
    public string RawContent { get; init; } = null!;
}

public record ProcessExpenseInputResult
{
    public bool Success { get; init; }
    public ExpenseInputDto? ExpenseInput { get; init; }
    public IEnumerable<ExpenseDto> CreatedExpenses { get; init; } = Array.Empty<ExpenseDto>();
    public string? ErrorMessage { get; init; }
}
