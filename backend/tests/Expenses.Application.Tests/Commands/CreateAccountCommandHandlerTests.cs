using Expenses.Application.Commands;
using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Interfaces;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly CreateAccountCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validExpenseGroupId = Guid.NewGuid();

    public CreateAccountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _handler = new CreateAccountCommandHandler(_accountRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateAccount()
    {
        var command = new CreateAccountCommand
        {
            UserId = _validUserId,
            Name = "Checking Account",
            ExpenseGroupId = _validExpenseGroupId
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Account);
        Assert.Equal(command.Name, result.Account.Name);
        Assert.Equal(command.ExpenseGroupId, result.Account.ExpenseGroupId);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyUserId_ShouldReturnFailure()
    {
        var command = new CreateAccountCommand
        {
            UserId = Guid.Empty,
            Name = "Checking Account",
            ExpenseGroupId = _validExpenseGroupId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyName_ShouldReturnFailure()
    {
        var command = new CreateAccountCommand
        {
            UserId = _validUserId,
            Name = "",
            ExpenseGroupId = _validExpenseGroupId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyExpenseGroupId_ShouldReturnFailure()
    {
        var command = new CreateAccountCommand
        {
            UserId = _validUserId,
            Name = "Checking Account",
            ExpenseGroupId = Guid.Empty
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
