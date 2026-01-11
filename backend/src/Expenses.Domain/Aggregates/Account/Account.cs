using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Aggregates.Account;

public class Account
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public Guid ExpenseGroupId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Account()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = null!;
    }

    public static Account Create(Guid userId, string name, Guid expenseGroupId)
    {
        if (userId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidUserId, "UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ErrorCodes.InvalidAccountName, "Account name cannot be empty");

        if (expenseGroupId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidExpenseGroupId, "ExpenseGroupId cannot be empty");

        var account = new Account
        {
            UserId = userId,
            Name = name.Trim(),
            ExpenseGroupId = expenseGroupId
        };

        return account;
    }

    public void UpdateName(string name)
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.AccountAlreadyDeleted, "Cannot update a deleted account");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ErrorCodes.InvalidAccountName, "Account name cannot be empty");

        Name = name.Trim();
    }

    public void ReassignToExpenseGroup(Guid expenseGroupId)
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.AccountAlreadyDeleted, "Cannot reassign a deleted account");

        if (expenseGroupId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidExpenseGroupId, "ExpenseGroupId cannot be empty");

        ExpenseGroupId = expenseGroupId;
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.AccountAlreadyDeleted, "Account is already deleted");

        DeletedAt = DateTime.UtcNow;
    }

    public bool IsDeleted => DeletedAt.HasValue;
}
