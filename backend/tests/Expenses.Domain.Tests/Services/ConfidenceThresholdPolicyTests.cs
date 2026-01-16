using Expenses.Domain.Services;
using Expenses.Domain.ValueObjects;

namespace Expenses.Domain.Tests.Services;

public class ConfidenceThresholdPolicyTests
{
    [Fact]
    public void DefaultConstructor_ShouldUseDefaultThreshold()
    {
        var policy = new ConfidenceThresholdPolicy();

        Assert.Equal(0.87, policy.Threshold.Value);
    }

    [Fact]
    public void Constructor_WithCustomThreshold_ShouldUseProvidedThreshold()
    {
        var customThreshold = new ConfidenceScore(0.9);
        var policy = new ConfidenceThresholdPolicy(customThreshold);

        Assert.Equal(0.9, policy.Threshold.Value);
    }

    [Theory]
    [InlineData(0.87, true)]
    [InlineData(0.88, true)]
    [InlineData(0.9, true)]
    [InlineData(1.0, true)]
    public void ShouldAutoConfirm_WithScoreAboveOrEqualThreshold_ShouldReturnTrue(double scoreValue, bool expected)
    {
        var policy = new ConfidenceThresholdPolicy();
        var score = new ConfidenceScore(scoreValue);

        var result = policy.ShouldAutoConfirm(score);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.86, false)]
    [InlineData(0.5, false)]
    [InlineData(0.0, false)]
    public void ShouldAutoConfirm_WithScoreBelowThreshold_ShouldReturnFalse(double scoreValue, bool expected)
    {
        var policy = new ConfidenceThresholdPolicy();
        var score = new ConfidenceScore(scoreValue);

        var result = policy.ShouldAutoConfirm(score);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.86, true)]
    [InlineData(0.5, true)]
    [InlineData(0.0, true)]
    public void RequiresReview_WithScoreBelowThreshold_ShouldReturnTrue(double scoreValue, bool expected)
    {
        var policy = new ConfidenceThresholdPolicy();
        var score = new ConfidenceScore(scoreValue);

        var result = policy.RequiresReview(score);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0.87, false)]
    [InlineData(0.9, false)]
    [InlineData(1.0, false)]
    public void RequiresReview_WithScoreAboveOrEqualThreshold_ShouldReturnFalse(double scoreValue, bool expected)
    {
        var policy = new ConfidenceThresholdPolicy();
        var score = new ConfidenceScore(scoreValue);

        var result = policy.RequiresReview(score);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CustomThreshold_ShouldAffectDecisions()
    {
        var customThreshold = new ConfidenceScore(0.95);
        var policy = new ConfidenceThresholdPolicy(customThreshold);

        var scoreAboveDefault = new ConfidenceScore(0.9);
        var scoreBelowCustom = policy.RequiresReview(scoreAboveDefault);

        Assert.True(scoreBelowCustom);
    }
}
