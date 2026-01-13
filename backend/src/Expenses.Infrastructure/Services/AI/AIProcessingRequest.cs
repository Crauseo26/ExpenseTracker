namespace Expenses.Infrastructure.Services.AI;

public class AIProcessingRequest
{
    public string NormalizedText { get; set; } = string.Empty;
    public string InputType { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public AIRequestMetadata Metadata { get; set; } = new();
}

public class AIRequestMetadata
{
    public string Source { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
