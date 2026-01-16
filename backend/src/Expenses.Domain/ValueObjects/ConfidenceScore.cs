using Expenses.Domain.Exceptions;

namespace Expenses.Domain.ValueObjects;

public record ConfidenceScore
{
    public double Value { get; init; }

    public ConfidenceScore(double value)
    {
        if (value < 0.0 || value > 1.0)
            throw new DomainException(ErrorCodes.Confidence.InvalidScore, "Confidence score must be between 0.0 and 1.0");

        Value = value;
    }

    public static ConfidenceScore FromPercentage(int percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new DomainException(ErrorCodes.Confidence.InvalidScore, "Percentage must be between 0 and 100");

        return new ConfidenceScore(percentage / 100.0);
    }

    public bool MeetsThreshold(ConfidenceScore threshold) => Value >= threshold.Value;

    public static implicit operator double(ConfidenceScore score) => score.Value;
}
