namespace Expenses.Api.DTOs;

public record CreateAccountRequest
{
    public string Name { get; init; } = null!;
    public Guid ExpenseGroupId { get; init; }
}
