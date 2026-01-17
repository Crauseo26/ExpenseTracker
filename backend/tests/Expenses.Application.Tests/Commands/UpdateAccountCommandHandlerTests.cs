using Expenses.Application.Commands;
using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Interfaces;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class UpdateAccountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly UpdateAccountCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validExpenseGroupId = Guid.NewGuid();

    public UpdateAccountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new UpdateAccountCommandHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldUpdateAccount()
    {
        var account = Account.Create(_validUserId, "Old Name", _validExpenseGroupId);
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id, _validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new UpdateAccountCommand
        {
            AccountId = account.Id,
            UserId = _validUserId,
            Name = "New Name",
            ExpenseGroupId = Guid.NewGuid()
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Account);
        Assert.Equal(command.Name, result.Account.Name);
        Assert.Equal(command.ExpenseGroupId, result.Account.ExpenseGroupId);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(account, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentAccount_ShouldReturnFailure()
    {
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var command = new UpdateAccountCommand
        {
            AccountId = Guid.NewGuid(),
            UserId = _validUserId,
            Name = "New Name",
            ExpenseGroupId = _validExpenseGroupId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithDeletedAccount_ShouldReturnFailure()
    {
        var account = Account.Create(_validUserId, "Old Name", _validExpenseGroupId);
        account.SoftDelete();
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id, _validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new UpdateAccountCommand
        {
            AccountId = account.Id,
            UserId = _validUserId,
            Name = "New Name",
            ExpenseGroupId = _validExpenseGroupId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }
}
