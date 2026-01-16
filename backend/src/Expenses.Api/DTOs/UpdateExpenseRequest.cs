namespace Expenses.Api.DTOs;

public record UpdateExpenseRequest
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ExpenseType { get; init; } = null!;
    public DateTime PurchaseDate { get; init; }
}
