namespace Expenses.Application.DTOs;

public record ExpenseProposalDto
{
    public Guid AccountId { get; init; }
    public Guid ExpenseGroupId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string ExpenseType { get; init; } = null!;
    public DateTime PurchaseDate { get; init; }
    public double Confidence { get; init; }
}

public record AIProposalResponseDto
{
    public IEnumerable<ExpenseProposalDto> Proposals { get; init; } = Array.Empty<ExpenseProposalDto>();
    public double OverallConfidence { get; init; }
}
