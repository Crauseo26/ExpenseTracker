using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Aggregates.ExpenseGroup;

public class ExpenseGroup
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private ExpenseGroup()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Name = null!;
    }

    public static ExpenseGroup Create(Guid userId, string name)
    {
        if (userId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidUserId, "UserId cannot be empty");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ErrorCodes.InvalidExpenseGroupName, "ExpenseGroup name cannot be empty");

        var expenseGroup = new ExpenseGroup
        {
            UserId = userId,
            Name = name.Trim()
        };

        return expenseGroup;
    }

    public void UpdateName(string name)
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.ExpenseGroupAlreadyDeleted, "Cannot update a deleted expense group");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException(ErrorCodes.InvalidExpenseGroupName, "ExpenseGroup name cannot be empty");

        Name = name.Trim();
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.ExpenseGroupAlreadyDeleted, "ExpenseGroup is already deleted");

        DeletedAt = DateTime.UtcNow;
    }

    public bool IsDeleted => DeletedAt.HasValue;
}
