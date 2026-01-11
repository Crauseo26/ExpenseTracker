using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.DTOs;

public record ExpenseDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ExpenseType { get; init; } = null!;
    public DateTime PurchaseDate { get; init; }
    public string Status { get; init; } = null!;
    public Guid? ExpenseInputId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    public static ExpenseDto FromDomain(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            UserId = expense.UserId,
            AccountId = expense.AccountId,
            Amount = expense.Amount.Amount,
            Currency = expense.Amount.Currency.ToString(),
            Description = expense.Description,
            ExpenseType = expense.ExpenseType.ToString(),
            PurchaseDate = expense.PurchaseDate,
            Status = expense.Status.ToString(),
            ExpenseInputId = expense.ExpenseInputId,
            CreatedAt = expense.CreatedAt,
            UpdatedAt = expense.UpdatedAt
        };
    }
}
