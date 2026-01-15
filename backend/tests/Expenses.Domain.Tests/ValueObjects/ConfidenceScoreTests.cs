using Expenses.Domain.Exceptions;
using Expenses.Domain.ValueObjects;

namespace Expenses.Domain.Tests.ValueObjects;

public class ConfidenceScoreTests
{
    [Theory]
    [InlineData(0.0)]
    [InlineData(0.5)]
    [InlineData(0.87)]
    [InlineData(1.0)]
    public void Constructor_WithValidValue_ShouldCreateInstance(double value)
    {
        var score = new ConfidenceScore(value);

        Assert.Equal(value, score.Value);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1.0)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void Constructor_WithInvalidValue_ShouldThrowDomainException(double value)
    {
        var exception = Assert.Throws<DomainException>(() => new ConfidenceScore(value));

        Assert.Equal(ErrorCodes.Confidence.InvalidScore, exception.ErrorCode);
        Assert.Contains("between 0.0 and 1.0", exception.Message);
    }

    [Theory]
    [InlineData(0, 0.0)]
    [InlineData(50, 0.5)]
    [InlineData(87, 0.87)]
    [InlineData(100, 1.0)]
    public void FromPercentage_WithValidPercentage_ShouldCreateInstance(int percentage, double expectedValue)
    {
        var score = ConfidenceScore.FromPercentage(percentage);

        Assert.Equal(expectedValue, score.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void FromPercentage_WithInvalidPercentage_ShouldThrowDomainException(int percentage)
    {
        var exception = Assert.Throws<DomainException>(() => ConfidenceScore.FromPercentage(percentage));

        Assert.Equal(ErrorCodes.Confidence.InvalidScore, exception.ErrorCode);
        Assert.Contains("between 0 and 100", exception.Message);
    }

    [Theory]
    [InlineData(0.9, 0.87, true)]
    [InlineData(0.87, 0.87, true)]
    [InlineData(0.86, 0.87, false)]
    [InlineData(0.5, 0.87, false)]
    public void MeetsThreshold_ShouldReturnCorrectResult(double scoreValue, double thresholdValue, bool expected)
    {
        var score = new ConfidenceScore(scoreValue);
        var threshold = new ConfidenceScore(thresholdValue);

        var result = score.MeetsThreshold(threshold);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void ImplicitConversion_ToDouble_ShouldReturnValue()
    {
        var score = new ConfidenceScore(0.87);

        double value = score;

        Assert.Equal(0.87, value);
    }

    [Fact]
    public void RecordEquality_WithSameValue_ShouldBeEqual()
    {
        var score1 = new ConfidenceScore(0.87);
        var score2 = new ConfidenceScore(0.87);

        Assert.Equal(score1, score2);
    }

    [Fact]
    public void RecordEquality_WithDifferentValue_ShouldNotBeEqual()
    {
        var score1 = new ConfidenceScore(0.87);
        var score2 = new ConfidenceScore(0.86);

        Assert.NotEqual(score1, score2);
    }
}
