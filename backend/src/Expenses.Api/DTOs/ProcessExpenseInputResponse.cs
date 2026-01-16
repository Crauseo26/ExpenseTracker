using Expenses.Application.DTOs;

namespace Expenses.Api.DTOs;

public record ProcessExpenseInputResponse
{
    public ExpenseInputDto ExpenseInput { get; init; } = null!;
    public IEnumerable<ExpenseDto> CreatedExpenses { get; init; } = Array.Empty<ExpenseDto>();
}
