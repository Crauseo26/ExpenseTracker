using Expenses.Application.Commands;
using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Interfaces;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class DeleteAccountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly DeleteAccountCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validExpenseGroupId = Guid.NewGuid();

    public DeleteAccountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new DeleteAccountCommandHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldDeleteAccount()
    {
        var account = Account.Create(_validUserId, "Test Account", _validExpenseGroupId);
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id, _validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new DeleteAccountCommand
        {
            AccountId = account.Id,
            UserId = _validUserId
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.True(account.IsDeleted);
        _accountRepositoryMock.Verify(r => r.DeleteAsync(account, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentAccount_ShouldReturnFailure()
    {
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account?)null);

        var command = new DeleteAccountCommand
        {
            AccountId = Guid.NewGuid(),
            UserId = _validUserId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _accountRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithAlreadyDeletedAccount_ShouldReturnFailure()
    {
        var account = Account.Create(_validUserId, "Test Account", _validExpenseGroupId);
        account.SoftDelete();
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id, _validUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var command = new DeleteAccountCommand
        {
            AccountId = account.Id,
            UserId = _validUserId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
    }
}
