namespace Expenses.Infrastructure.Services.AI;

public class AIProcessingResponse
{
    public List<AIExpenseProposal> Proposals { get; set; } = new();
    public double OverallConfidence { get; set; }
}

public class AIExpenseProposal
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ExpenseType { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public double Confidence { get; set; }
}
