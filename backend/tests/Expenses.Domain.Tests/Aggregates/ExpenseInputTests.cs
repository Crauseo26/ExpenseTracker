using Expenses.Domain.Aggregates.ExpenseInput;
using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Tests.Aggregates;

public class ExpenseInputTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private const string ValidRawContent = "Bought groceries for $50";

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);

        Assert.NotEqual(Guid.Empty, expenseInput.Id);
        Assert.Equal(_validUserId, expenseInput.UserId);
        Assert.Equal(InputType.TEXT, expenseInput.InputType);
        Assert.Equal(ValidRawContent, expenseInput.RawContent);
        Assert.Equal(ProcessingStatus.PENDING, expenseInput.Status);
        Assert.Null(expenseInput.NormalizedContent);
        Assert.Null(expenseInput.ErrorMessage);
        Assert.True(expenseInput.CreatedAt <= DateTime.UtcNow);
        Assert.Null(expenseInput.ProcessedAt);
        Assert.Null(expenseInput.DeletedAt);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseInput.Create(Guid.Empty, InputType.TEXT, ValidRawContent));

        Assert.Equal(ErrorCodes.ExpenseInput.InvalidUserId, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyRawContent_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseInput.Create(_validUserId, InputType.TEXT, ""));

        Assert.Equal(ErrorCodes.ExpenseInput.InvalidRawContent, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithWhitespaceRawContent_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseInput.Create(_validUserId, InputType.TEXT, "   "));

        Assert.Equal(ErrorCodes.ExpenseInput.InvalidRawContent, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithRawContentTooLong_ShouldThrowDomainException()
    {
        var longContent = new string('a', 10001);

        var exception = Assert.Throws<DomainException>(() =>
            ExpenseInput.Create(_validUserId, InputType.TEXT, longContent));

        Assert.Equal(ErrorCodes.ExpenseInput.RawContentTooLong, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithMaxLengthRawContent_ShouldSucceed()
    {
        var maxContent = new string('a', 10000);

        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, maxContent);

        Assert.Equal(maxContent, expenseInput.RawContent);
    }

    [Fact]
    public void SetNormalizedContent_OnPendingInput_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        const string normalizedContent = "Groceries - $50.00";

        expenseInput.SetNormalizedContent(normalizedContent);

        Assert.Equal(normalizedContent, expenseInput.NormalizedContent);
    }

    [Fact]
    public void SetNormalizedContent_WithEmptyContent_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);

        var exception = Assert.Throws<DomainException>(() => expenseInput.SetNormalizedContent(""));

        Assert.Equal(ErrorCodes.ExpenseInput.InvalidNormalizedContent, exception.ErrorCode);
    }

    [Fact]
    public void SetNormalizedContent_WithContentTooLong_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        var longContent = new string('a', 10001);

        var exception = Assert.Throws<DomainException>(() => expenseInput.SetNormalizedContent(longContent));

        Assert.Equal(ErrorCodes.ExpenseInput.NormalizedContentTooLong, exception.ErrorCode);
    }

    [Fact]
    public void SetNormalizedContent_OnProcessedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsProcessed();

        var exception = Assert.Throws<DomainException>(() => expenseInput.SetNormalizedContent("New content"));

        Assert.Equal(ErrorCodes.ExpenseInput.CannotModifyProcessedInput, exception.ErrorCode);
    }

    [Fact]
    public void SetNormalizedContent_OnErrorInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsError("Processing failed");

        var exception = Assert.Throws<DomainException>(() => expenseInput.SetNormalizedContent("New content"));

        Assert.Equal(ErrorCodes.ExpenseInput.CannotModifyProcessedInput, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsProcessed_OnPendingInput_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);

        expenseInput.MarkAsProcessed();

        Assert.Equal(ProcessingStatus.PROCESSED, expenseInput.Status);
        Assert.NotNull(expenseInput.ProcessedAt);
        Assert.True(expenseInput.ProcessedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsProcessed_OnAlreadyProcessedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsProcessed();

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsProcessed());

        Assert.Equal(ErrorCodes.ExpenseInput.AlreadyProcessed, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsProcessed_OnErrorInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsError("Error occurred");

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsProcessed());

        Assert.Equal(ErrorCodes.ExpenseInput.CannotProcessErrorInput, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsProcessed_OnDeletedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsProcessed());

        Assert.Equal(ErrorCodes.ExpenseInput.CannotProcessDeletedInput, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsError_OnPendingInput_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        const string errorMessage = "AI service unavailable";

        expenseInput.MarkAsError(errorMessage);

        Assert.Equal(ProcessingStatus.ERROR, expenseInput.Status);
        Assert.Equal(errorMessage, expenseInput.ErrorMessage);
        Assert.NotNull(expenseInput.ProcessedAt);
        Assert.True(expenseInput.ProcessedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void MarkAsError_WithEmptyErrorMessage_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsError(""));

        Assert.Equal(ErrorCodes.ExpenseInput.InvalidErrorMessage, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsError_OnProcessedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsProcessed();

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsError("Error"));

        Assert.Equal(ErrorCodes.ExpenseInput.CannotMarkProcessedAsError, exception.ErrorCode);
    }

    [Fact]
    public void MarkAsError_OnDeletedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => expenseInput.MarkAsError("Error"));

        Assert.Equal(ErrorCodes.ExpenseInput.CannotMarkDeletedAsError, exception.ErrorCode);
    }

    [Fact]
    public void SoftDelete_OnActiveInput_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);

        expenseInput.SoftDelete();

        Assert.NotNull(expenseInput.DeletedAt);
        Assert.True(expenseInput.DeletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void SoftDelete_OnAlreadyDeletedInput_ShouldThrowDomainException()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => expenseInput.SoftDelete());

        Assert.Equal(ErrorCodes.ExpenseInput.AlreadyDeleted, exception.ErrorCode);
    }

    [Fact]
    public void SoftDelete_OnProcessedInput_ShouldSucceed()
    {
        var expenseInput = ExpenseInput.Create(_validUserId, InputType.TEXT, ValidRawContent);
        expenseInput.MarkAsProcessed();

        expenseInput.SoftDelete();

        Assert.NotNull(expenseInput.DeletedAt);
    }
}
