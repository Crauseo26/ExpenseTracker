using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Services;
using Expenses.Domain.ValueObjects;

namespace Expenses.Domain.Tests.Aggregates;

public class ExpenseConfidenceTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _accountId = Guid.NewGuid();
    private readonly Guid _expenseInputId = Guid.NewGuid();

    [Fact]
    public void CreateFromAI_WithHighConfidence_ShouldCreateConfirmedExpense()
    {
        var highConfidence = new ConfidenceScore(0.92);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            highConfidence);

        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);
    }

    [Fact]
    public void CreateFromAI_WithExactThresholdConfidence_ShouldCreateConfirmedExpense()
    {
        var exactThreshold = new ConfidenceScore(0.87);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            exactThreshold);

        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);
    }

    [Fact]
    public void CreateFromAI_WithLowConfidence_ShouldCreatePendingReviewExpense()
    {
        var lowConfidence = new ConfidenceScore(0.75);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            lowConfidence);

        Assert.Equal(ExpenseStatus.PendingReview, expense.Status);
    }

    [Fact]
    public void CreateFromAI_WithCustomPolicy_ShouldRespectCustomThreshold()
    {
        var customThreshold = new ConfidenceScore(0.95);
        var customPolicy = new ConfidenceThresholdPolicy(customThreshold);
        var mediumConfidence = new ConfidenceScore(0.90);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            mediumConfidence,
            customPolicy);

        Assert.Equal(ExpenseStatus.PendingReview, expense.Status);
    }

    [Fact]
    public void CreateFromAI_WithNullPolicy_ShouldUseDefaultThreshold()
    {
        var aboveDefaultThreshold = new ConfidenceScore(0.88);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            aboveDefaultThreshold,
            null);

        Assert.Equal(ExpenseStatus.Confirmed, expense.Status);
    }

    [Fact]
    public void CreateFromAI_ShouldSetExpenseInputId()
    {
        var confidence = new ConfidenceScore(0.9);
        var money = new Money(100.00m, Currency.UYU);

        var expense = Expense.CreateFromAI(
            _userId,
            _accountId,
            money,
            "Test expense",
            ExpenseType.Sporadic,
            DateTime.UtcNow,
            _expenseInputId,
            confidence);

        Assert.Equal(_expenseInputId, expense.ExpenseInputId);
    }
}
