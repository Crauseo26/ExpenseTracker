using Expenses.Application.Commands;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;
using Expenses.Domain.ValueObjects;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class ConfirmExpenseCommandHandlerTests
{
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly ConfirmExpenseCommandHandler _handler;

    public ConfirmExpenseCommandHandlerTests()
    {
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _handler = new ConfirmExpenseCommandHandler(_expenseRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithPendingReviewExpense_ShouldConfirmExpense()
    {
        var userId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var expenseInputId = Guid.NewGuid();

        var expense = Expense.CreateFromAI(
            userId,
            accountId,
            new Money(100.00m, Currency.UYU),
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            expenseInputId,
            new ConfidenceScore(0.75));

        Assert.Equal(ExpenseStatus.PendingReview, expense.Status);

        _expenseRepositoryMock
            .Setup(x => x.GetByIdAsync(expenseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var command = new ConfirmExpenseCommand
        {
            ExpenseId = expenseId,
            UserId = userId
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(result.Expense);
        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);
        _expenseRepositoryMock.Verify(x => x.UpdateAsync(expense, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithAlreadyConfirmedExpense_ShouldRemainConfirmed()
    {
        var userId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var expense = Expense.CreateManual(
            userId,
            accountId,
            new Money(100.00m, Currency.UYU),
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow);

        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);

        _expenseRepositoryMock
            .Setup(x => x.GetByIdAsync(expenseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var command = new ConfirmExpenseCommand
        {
            ExpenseId = expenseId,
            UserId = userId
        };

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);
        _expenseRepositoryMock.Verify(x => x.UpdateAsync(expense, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentExpense_ShouldReturnError()
    {
        var userId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();

        _expenseRepositoryMock
            .Setup(x => x.GetByIdAsync(expenseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expense?)null);

        var command = new ConfirmExpenseCommand
        {
            ExpenseId = expenseId,
            UserId = userId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Equal("Expense not found", result.ErrorMessage);
        _expenseRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WithDeletedExpense_ShouldReturnDomainError()
    {
        var userId = Guid.NewGuid();
        var expenseId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var expense = Expense.CreateManual(
            userId,
            accountId,
            new Money(100.00m, Currency.UYU),
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow);

        expense.SoftDelete();

        _expenseRepositoryMock
            .Setup(x => x.GetByIdAsync(expenseId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expense);

        var command = new ConfirmExpenseCommand
        {
            ExpenseId = expenseId,
            UserId = userId
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Contains("Cannot confirm a deleted expense", result.ErrorMessage);
    }
}
