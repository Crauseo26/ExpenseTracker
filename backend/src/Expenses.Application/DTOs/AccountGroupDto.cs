namespace Expenses.Application.DTOs;

public record AccountGroupDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public DateTime CreatedAt { get; init; }

    public static AccountGroupDto FromDomain(Domain.Aggregates.ExpenseGroup.ExpenseGroup expenseGroup)
    {
        return new AccountGroupDto
        {
            Id = expenseGroup.Id,
            UserId = expenseGroup.UserId,
            Name = expenseGroup.Name,
            CreatedAt = expenseGroup.CreatedAt
        };
    }
}
