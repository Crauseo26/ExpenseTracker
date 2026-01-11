using Expenses.Domain.Exceptions;
using Expenses.Domain.ValueObjects;

namespace Expenses.Domain.Aggregates.Expense;

public class Expense
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AccountId { get; private set; }
    public Money Amount { get; private set; }
    public string Description { get; private set; }
    public ExpenseType ExpenseType { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public ExpenseStatus Status { get; private set; }
    public Guid? ExpenseInputId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Expense()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Amount = null!;
        Description = null!;
    }

    public static Expense CreateManual(
        Guid userId,
        Guid accountId,
        Money amount,
        string description,
        ExpenseType expenseType,
        DateTime purchaseDate)
    {
        ValidateCreationParameters(userId, accountId, description);

        var expense = new Expense
        {
            UserId = userId,
            AccountId = accountId,
            Amount = amount,
            Description = description,
            ExpenseType = expenseType,
            PurchaseDate = purchaseDate,
            Status = ExpenseStatus.Confirmed
        };

        return expense;
    }

    public static Expense CreateFromAI(
        Guid userId,
        Guid accountId,
        Money amount,
        string description,
        ExpenseType expenseType,
        DateTime purchaseDate,
        Guid expenseInputId,
        double confidenceScore,
        double confidenceThreshold = 0.87)
    {
        ValidateCreationParameters(userId, accountId, description);

        var status = confidenceScore >= confidenceThreshold
            ? ExpenseStatus.Confirmed
            : ExpenseStatus.PendingReview;

        var expense = new Expense
        {
            UserId = userId,
            AccountId = accountId,
            Amount = amount,
            Description = description,
            ExpenseType = expenseType,
            PurchaseDate = purchaseDate,
            Status = status,
            ExpenseInputId = expenseInputId
        };

        return expense;
    }

    private static void ValidateCreationParameters(Guid userId, Guid accountId, string description)
    {
        if (userId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidUserId, "UserId cannot be empty");

        if (accountId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidAccountId, "AccountId cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(ErrorCodes.InvalidExpenseDescription, "Description cannot be empty");
    }

    public void Update(
        Guid accountId,
        Money amount,
        string description,
        ExpenseType expenseType,
        DateTime purchaseDate)
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.ExpenseAlreadyDeleted, "Cannot update a deleted expense");

        if (accountId == Guid.Empty)
            throw new DomainException(ErrorCodes.InvalidAccountId, "AccountId cannot be empty");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException(ErrorCodes.InvalidExpenseDescription, "Description cannot be empty");

        if (Status == ExpenseStatus.Confirmed)
        {
            if (purchaseDate.Year != PurchaseDate.Year || purchaseDate.Month != PurchaseDate.Month)
            {
                throw new DomainException(
                    ErrorCodes.InvalidPurchaseDateEdit,
                    "Cannot change month or year of confirmed expense");
            }
        }

        AccountId = accountId;
        Amount = amount;
        Description = description;
        ExpenseType = expenseType;
        PurchaseDate = purchaseDate;
        UpdatedAt = DateTime.UtcNow;

        if (Status == ExpenseStatus.PendingReview)
        {
            Status = ExpenseStatus.Confirmed;
        }
    }

    public void Confirm()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.ExpenseAlreadyDeleted, "Cannot confirm a deleted expense");

        if (Status == ExpenseStatus.Confirmed)
            return;

        Status = ExpenseStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
            throw new DomainException(ErrorCodes.ExpenseAlreadyDeleted, "Expense is already deleted");

        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDeleted => DeletedAt.HasValue;
}
