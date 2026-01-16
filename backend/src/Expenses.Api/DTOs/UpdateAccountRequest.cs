namespace Expenses.Api.DTOs;

public record UpdateAccountRequest
{
    public string Name { get; init; } = null!;
    public Guid ExpenseGroupId { get; init; }
}
