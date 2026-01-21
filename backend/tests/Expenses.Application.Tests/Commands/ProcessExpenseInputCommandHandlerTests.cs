using Expenses.Application.Commands;
using Expenses.Application.DTOs;
using Expenses.Application.Interfaces;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Aggregates.ExpenseInput;
using Expenses.Domain.Interfaces;
using Expenses.Domain.Services;
using Expenses.Domain.ValueObjects;
using Moq;

namespace Expenses.Application.Tests.Commands;

public class ProcessExpenseInputCommandHandlerTests
{
    private readonly Mock<IExpenseInputRepository> _expenseInputRepositoryMock;
    private readonly Mock<IExpenseRepository> _expenseRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IAIOrchestrationService> _aiServiceMock;
    private readonly ConfidenceThresholdPolicy _confidencePolicy;
    private readonly ProcessExpenseInputCommandHandler _handler;

    public ProcessExpenseInputCommandHandlerTests()
    {
        _expenseInputRepositoryMock = new Mock<IExpenseInputRepository>();
        _expenseRepositoryMock = new Mock<IExpenseRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _aiServiceMock = new Mock<IAIOrchestrationService>();
        _confidencePolicy = new ConfidenceThresholdPolicy();

        _accountRepositoryMock
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Aggregates.Account.Account>());

        _handler = new ProcessExpenseInputCommandHandler(
            _expenseInputRepositoryMock.Object,
            _expenseRepositoryMock.Object,
            _accountRepositoryMock.Object,
            _aiServiceMock.Object,
            _confidencePolicy);
    }

    [Fact]
    public async Task HandleAsync_WithHighConfidenceProposal_ShouldCreateConfirmedExpense()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Uber 510 yesterday"
        };

        var aiResponse = new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>
            {
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 510.00m,
                    Currency = "UYU",
                    Description = "Uber",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow.AddDays(-1),
                    Confidence = 0.92
                }
            },
            OverallConfidence = 0.92
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);

        Expense? capturedExpense = null;
        _expenseRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
            .Callback<Expense, CancellationToken>((exp, ct) => capturedExpense = exp)
            .ReturnsAsync((Expense exp, CancellationToken ct) => exp);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(capturedExpense);
        Assert.Equal(ExpenseStatus.Confirmed, capturedExpense.Status);
        Assert.Single(result.CreatedExpenses);
    }

    [Fact]
    public async Task HandleAsync_WithLowConfidenceProposal_ShouldCreatePendingReviewExpense()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Coffee 310"
        };

        var aiResponse = new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>
            {
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 310.00m,
                    Currency = "UYU",
                    Description = "Coffee",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow,
                    Confidence = 0.75
                }
            },
            OverallConfidence = 0.75
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);

        Expense? capturedExpense = null;
        _expenseRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
            .Callback<Expense, CancellationToken>((exp, ct) => capturedExpense = exp)
            .ReturnsAsync((Expense exp, CancellationToken ct) => exp);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(capturedExpense);
        Assert.Equal(ExpenseStatus.PendingReview, capturedExpense.Status);
        Assert.Single(result.CreatedExpenses);
    }

    [Fact]
    public async Task HandleAsync_WithExactThresholdConfidence_ShouldCreateConfirmedExpense()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Lunch 1200"
        };

        var aiResponse = new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>
            {
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 1200.00m,
                    Currency = "UYU",
                    Description = "Lunch",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow,
                    Confidence = 0.87
                }
            },
            OverallConfidence = 0.87
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);

        Expense? capturedExpense = null;
        _expenseRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()))
            .Callback<Expense, CancellationToken>((exp, ct) => capturedExpense = exp)
            .ReturnsAsync((Expense exp, CancellationToken ct) => exp);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.NotNull(capturedExpense);
        Assert.Equal(ExpenseStatus.Confirmed, capturedExpense.Status);
    }

    [Fact]
    public async Task HandleAsync_WithAIFailure_ShouldMarkExpenseInputAsError()
    {
        var userId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Invalid input"
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("AI service unavailable"));

        ExpenseInput? capturedInput = null;
        _expenseInputRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ExpenseInput>(), It.IsAny<CancellationToken>()))
            .Callback<ExpenseInput, CancellationToken>((input, ct) => capturedInput = input)
            .Returns(Task.CompletedTask);

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Contains("AI processing failed", result.ErrorMessage);
        Assert.NotNull(capturedInput);
        Assert.Equal(ProcessingStatus.ERROR, capturedInput.Status);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleProposals_ShouldCreateMultipleExpenses()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Uber 510, Coffee 310"
        };

        var aiResponse = new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>
            {
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 510.00m,
                    Currency = "UYU",
                    Description = "Uber",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow,
                    Confidence = 0.92
                },
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 310.00m,
                    Currency = "UYU",
                    Description = "Coffee",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow,
                    Confidence = 0.85
                }
            },
            OverallConfidence = 0.88
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.Equal(2, result.CreatedExpenses.Count());
        _expenseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task HandleAsync_WithInvalidInputType_ShouldReturnError()
    {
        var command = new ProcessExpenseInputCommand
        {
            UserId = Guid.NewGuid(),
            InputType = "INVALID_TYPE",
            RawContent = "Test"
        };

        var result = await _handler.HandleAsync(command);

        Assert.False(result.Success);
        Assert.Contains("Invalid input type", result.ErrorMessage);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidCurrency_ShouldSkipProposal()
    {
        var userId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var command = new ProcessExpenseInputCommand
        {
            UserId = userId,
            InputType = "TEXT",
            RawContent = "Test"
        };

        var aiResponse = new AIProposalResponseDto
        {
            Proposals = new List<ExpenseProposalDto>
            {
                new ExpenseProposalDto
                {
                    AccountId = accountId,
                    Amount = 100.00m,
                    Currency = "INVALID",
                    Description = "Test",
                    ExpenseType = "SPORADIC",
                    PurchaseDate = DateTime.UtcNow,
                    Confidence = 0.9
                }
            },
            OverallConfidence = 0.9
        };

        _aiServiceMock
            .Setup(x => x.ProcessInputAsync(userId, "TEXT", It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);

        var result = await _handler.HandleAsync(command);

        Assert.True(result.Success);
        Assert.Empty(result.CreatedExpenses);
        _expenseRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Expense>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
