namespace Expenses.Api.DTOs;

public record ProcessExpenseInputRequest
{
    public string InputType { get; init; } = null!;
    public string RawContent { get; init; } = null!;
}
