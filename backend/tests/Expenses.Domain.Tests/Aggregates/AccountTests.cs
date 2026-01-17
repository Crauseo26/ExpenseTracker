using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Exceptions;

namespace Expenses.Domain.Tests.Aggregates;

public class AccountTests
{
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validExpenseGroupId = Guid.NewGuid();
    private const string ValidName = "Checking Account";

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);

        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.Equal(_validUserId, account.UserId);
        Assert.Equal(ValidName, account.Name);
        Assert.Equal(_validExpenseGroupId, account.ExpenseGroupId);
        Assert.True(account.CreatedAt <= DateTime.UtcNow);
        Assert.Null(account.DeletedAt);
        Assert.False(account.IsDeleted);
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Account.Create(Guid.Empty, ValidName, _validExpenseGroupId));

        Assert.Equal(ErrorCodes.InvalidUserId, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Account.Create(_validUserId, "", _validExpenseGroupId));

        Assert.Equal(ErrorCodes.InvalidAccountName, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Account.Create(_validUserId, "   ", _validExpenseGroupId));

        Assert.Equal(ErrorCodes.InvalidAccountName, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithEmptyExpenseGroupId_ShouldThrowDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Account.Create(_validUserId, ValidName, Guid.Empty));

        Assert.Equal(ErrorCodes.InvalidExpenseGroupId, exception.ErrorCode);
    }

    [Fact]
    public void Create_WithNameContainingWhitespace_ShouldTrimName()
    {
        var account = Account.Create(_validUserId, "  Savings Account  ", _validExpenseGroupId);

        Assert.Equal("Savings Account", account.Name);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldSucceed()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);
        const string newName = "Updated Account";

        account.UpdateName(newName);

        Assert.Equal(newName, account.Name);
    }

    [Fact]
    public void UpdateName_WithWhitespace_ShouldTrimName()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);

        account.UpdateName("  New Name  ");

        Assert.Equal("New Name", account.Name);
    }

    [Fact]
    public void UpdateName_WithEmptyName_ShouldThrowDomainException()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);

        var exception = Assert.Throws<DomainException>(() => account.UpdateName(""));

        Assert.Equal(ErrorCodes.InvalidAccountName, exception.ErrorCode);
    }

    [Fact]
    public void UpdateName_OnDeletedAccount_ShouldThrowDomainException()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);
        account.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => account.UpdateName("New Name"));

        Assert.Equal(ErrorCodes.AccountAlreadyDeleted, exception.ErrorCode);
    }

    [Fact]
    public void ReassignToExpenseGroup_WithValidId_ShouldSucceed()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);
        var newExpenseGroupId = Guid.NewGuid();

        account.ReassignToExpenseGroup(newExpenseGroupId);

        Assert.Equal(newExpenseGroupId, account.ExpenseGroupId);
    }

    [Fact]
    public void ReassignToExpenseGroup_WithEmptyId_ShouldThrowDomainException()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);

        var exception = Assert.Throws<DomainException>(() => account.ReassignToExpenseGroup(Guid.Empty));

        Assert.Equal(ErrorCodes.InvalidExpenseGroupId, exception.ErrorCode);
    }

    [Fact]
    public void ReassignToExpenseGroup_OnDeletedAccount_ShouldThrowDomainException()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);
        account.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => account.ReassignToExpenseGroup(Guid.NewGuid()));

        Assert.Equal(ErrorCodes.AccountAlreadyDeleted, exception.ErrorCode);
    }

    [Fact]
    public void SoftDelete_OnActiveAccount_ShouldSucceed()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);

        account.SoftDelete();

        Assert.NotNull(account.DeletedAt);
        Assert.True(account.DeletedAt <= DateTime.UtcNow);
        Assert.True(account.IsDeleted);
    }

    [Fact]
    public void SoftDelete_OnAlreadyDeletedAccount_ShouldThrowDomainException()
    {
        var account = Account.Create(_validUserId, ValidName, _validExpenseGroupId);
        account.SoftDelete();

        var exception = Assert.Throws<DomainException>(() => account.SoftDelete());

        Assert.Equal(ErrorCodes.AccountAlreadyDeleted, exception.ErrorCode);
    }
}
