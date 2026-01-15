using Expenses.Domain.ValueObjects;

namespace Expenses.Domain.Services;

public class ConfidenceThresholdPolicy
{
    private static readonly ConfidenceScore DefaultThreshold = new(0.87);

    public ConfidenceScore Threshold { get; }

    public ConfidenceThresholdPolicy() : this(DefaultThreshold)
    {
    }

    public ConfidenceThresholdPolicy(ConfidenceScore threshold)
    {
        Threshold = threshold;
    }

    public bool ShouldAutoConfirm(ConfidenceScore score)
    {
        return score.MeetsThreshold(Threshold);
    }

    public bool RequiresReview(ConfidenceScore score)
    {
        return !ShouldAutoConfirm(score);
    }
}
