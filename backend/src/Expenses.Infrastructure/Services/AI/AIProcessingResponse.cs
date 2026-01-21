using System.Text.Json.Serialization;

namespace Expenses.Infrastructure.Services.AI;

public class AIProcessingResponse
{
    [JsonPropertyName("proposals")]
    public List<AIExpenseProposal> Proposals { get; set; } = new();
    
    [JsonPropertyName("overallConfidence")]
    public double OverallConfidence { get; set; }
    
    [JsonPropertyName("processingTimeMs")]
    public int ProcessingTimeMs { get; set; }
}

public class AIExpenseProposal
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
    
    [JsonPropertyName("purchaseDate")]
    public string PurchaseDate { get; set; } = string.Empty;
    
    [JsonPropertyName("expenseType")]
    public string ExpenseType { get; set; } = string.Empty;
    
    [JsonPropertyName("confidence")]
    public double Confidence { get; set; }
    
    [JsonPropertyName("metadata")]
    public AIProposalMetadata Metadata { get; set; } = new();
}

public class AIProposalMetadata
{
    [JsonPropertyName("merchant")]
    public string Merchant { get; set; } = string.Empty;
    
    [JsonPropertyName("rawExtraction")]
    public string RawExtraction { get; set; } = string.Empty;
    
    [JsonPropertyName("suggestedAccount")]
    public string SuggestedAccount { get; set; } = string.Empty;
}
