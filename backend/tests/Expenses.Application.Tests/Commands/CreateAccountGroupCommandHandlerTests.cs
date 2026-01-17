using Expenses.Application.Commands;
using Expenses.Domain.Aggregates.ExpenseGroup;
using Expenses.Domain.Interfaces;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class CreateAccountGroupCommandHandlerTests
{
    private readonly Mock<IExpenseGroupRepository> _expenseGroupRepositoryMock;
    private readonly CreateAccountGroupCommandHandler _handler;
    private readonly Guid _validUserId = Guid.NewGuid();

    public CreateAccountGroupCommandHandlerTests()
    {
        _expenseGroupRepositoryMock = new Mock<IExpenseGroupRepository>();
        _handler = new CreateAccountGroupCommandHandler(_expenseGroupRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidCommand_ShouldCreateAccountGroup()
    {
        var command = new CreateAccountGroupCommand
        {
            UserId = _validUserId,
            Name = "Food & Dining"
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(result.AccountGroup);
        Assert.Equal(command.Name, result.AccountGroup.Name);
        _expenseGroupRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExpenseGroup>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyUserId_ShouldReturnFailure()
    {
        var command = new CreateAccountGroupCommand
        {
            UserId = Guid.Empty,
            Name = "Food & Dining"
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _expenseGroupRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExpenseGroup>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyName_ShouldReturnFailure()
    {
        var command = new CreateAccountGroupCommand
        {
            UserId = _validUserId,
            Name = ""
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _expenseGroupRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ExpenseGroup>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
