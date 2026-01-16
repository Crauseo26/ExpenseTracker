using Expenses.Domain.Aggregates.ExpenseGroup;
using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Tests.Aggregates;

public class ExpenseGroupTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private const string ValidName = "Food & Dining";

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);

        Assert.NotEqual(Guid.Empty, expenseGroup.Id);
        Assert.Equal(_validUserId, expenseGroup.UserId);
        Assert.Equal(ValidName, expenseGroup.Name);
        Assert.True(expenseGroup.CreatedAt <= DateTime.UtcNow);
        Assert.Null(expenseGroup.DeletedAt);
        Assert.False(expenseGroup.IsDeleted);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseGroup.Create(Guid.Empty, ValidName));

        Assert.Equal(ErrorCodes.InvalidUserId, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseGroup.Create(_validUserId, ""));

        Assert.Equal(ErrorCodes.InvalidExpenseGroupName, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            ExpenseGroup.Create(_validUserId, "   "));

        Assert.Equal(ErrorCodes.InvalidExpenseGroupName, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, "  Transportation  ");

        Assert.Equal("Transportation", expenseGroup.Name);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldSucceed()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);
        const string newName = "Updated Group";

        expenseGroup.UpdateName(newName);

        Assert.Equal(newName, expenseGroup.Name);
    }

    [Fact]
    public void UpdateName_WithWhitespace_ShouldTrimName()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);

        expenseGroup.UpdateName("  New Name  ");

        Assert.Equal("New Name", expenseGroup.Name);
    }

    [Fact]
    public void UpdateName_WithEmptyName_ShouldThrowDomainException()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);

        var exception = Assert.Throws<DomainException>(() => expenseGroup.UpdateName(""));

        Assert.Equal(ErrorCodes.InvalidExpenseGroupName, exception.ErrorCode);
    }

    [Fact]
    public void UpdateName_OnDeletedExpenseGroup_ShouldThrowDomainException()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);
        expenseGroup.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => expenseGroup.UpdateName("New Name"));

        Assert.Equal(ErrorCodes.ExpenseGroupAlreadyDeleted, exception.ErrorCode);
    }

    [Fact]
    public void SoftDelete_OnActiveExpenseGroup_ShouldSucceed()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);

        expenseGroup.SoftDelete();

        Assert.NotNull(expenseGroup.DeletedAt);
        Assert.True(expenseGroup.DeletedAt <= DateTime.UtcNow);
        Assert.True(expenseGroup.IsDeleted);
    }

    [Fact]
    public void SoftDelete_OnAlreadyDeletedExpenseGroup_ShouldThrowDomainException()
    {
        var expenseGroup = ExpenseGroup.Create(_validUserId, ValidName);
        expenseGroup.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => expenseGroup.SoftDelete());

        Assert.Equal(ErrorCodes.ExpenseGroupAlreadyDeleted, exception.ErrorCode);
    }
}
