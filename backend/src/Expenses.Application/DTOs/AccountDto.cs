namespace Expenses.Application.DTOs;

public record AccountDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = null!;
    public Guid ExpenseGroupId { get; init; }
    public DateTime CreatedAt { get; init; }

    public static AccountDto FromDomain(Domain.Aggregates.Account.Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,
            ExpenseGroupId = account.ExpenseGroupId,
            CreatedAt = account.CreatedAt
        };
    }
}
