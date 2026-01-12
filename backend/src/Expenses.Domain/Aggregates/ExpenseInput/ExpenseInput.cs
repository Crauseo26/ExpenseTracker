using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Aggregates.ExpenseInput;

public class ExpenseInput
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public InputType InputType { get; private set; }
    public string RawContent { get; private set; } = null!;
    public string? NormalizedContent { get; private set; }
    public ProcessingStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private ExpenseInput() { }

    public static ExpenseInput Create(Guid userId, InputType inputType, string rawContent)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.InvalidUserId, "User ID cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(rawContent))
        {
            throw new DomainException(ErrorCodes.ExpenseInput.InvalidRawContent, "Raw content cannot be empty");
        }

        if (rawContent.Length > 10000)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.RawContentTooLong, "Raw content cannot exceed 10000 characters");
        }

        return new ExpenseInput
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            InputType = inputType,
            RawContent = rawContent,
            Status = ProcessingStatus.PENDING,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetNormalizedContent(string normalizedContent)
    {
        if (Status != ProcessingStatus.PENDING)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.CannotModifyProcessedInput, "Cannot modify normalized content after processing");
        }

        if (string.IsNullOrWhiteSpace(normalizedContent))
        {
            throw new DomainException(ErrorCodes.ExpenseInput.InvalidNormalizedContent, "Normalized content cannot be empty");
        }

        if (normalizedContent.Length > 10000)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.NormalizedContentTooLong, "Normalized content cannot exceed 10000 characters");
        }

        NormalizedContent = normalizedContent;
    }

    public void MarkAsProcessed()
    {
        if (Status == ProcessingStatus.PROCESSED)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.AlreadyProcessed, "ExpenseInput is already processed");
        }

        if (Status == ProcessingStatus.ERROR)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.CannotProcessErrorInput, "Cannot mark error input as processed");
        }

        if (DeletedAt.HasValue)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.CannotProcessDeletedInput, "Cannot process deleted input");
        }

        Status = ProcessingStatus.PROCESSED;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsError(string errorMessage)
    {
        if (Status == ProcessingStatus.PROCESSED)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.CannotMarkProcessedAsError, "Cannot mark processed input as error");
        }

        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new DomainException(ErrorCodes.ExpenseInput.InvalidErrorMessage, "Error message cannot be empty");
        }

        if (DeletedAt.HasValue)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.CannotMarkDeletedAsError, "Cannot mark deleted input as error");
        }

        Status = ProcessingStatus.ERROR;
        ErrorMessage = errorMessage;
        ProcessedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        if (DeletedAt.HasValue)
        {
            throw new DomainException(ErrorCodes.ExpenseInput.AlreadyDeleted, "ExpenseInput is already deleted");
        }

        DeletedAt = DateTime.UtcNow;
    }
}
