using System.Text.Json.Serialization;

namespace Expenses.Infrastructure.Services.AI;

public class AIProcessingRequest
{
    [JsonPropertyName("rawText")]
    public string RawText { get; set; } = string.Empty;
    
    [JsonPropertyName("inputType")]
    public string InputType { get; set; } = string.Empty;
    
    [JsonPropertyName("availableAccounts")]
    public List<string> AvailableAccounts { get; set; } = new();
    
    [JsonPropertyName("metadata")]
    public AIRequestMetadata Metadata { get; set; } = new();
}

public class AIRequestMetadata
{
    [JsonPropertyName("receivedAt")]
    public string ReceivedAt { get; set; } = string.Empty;
}
