namespace Expenses.Application.DTOs;

public record ExpenseInputDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string InputType { get; init; } = null!;
    public string RawContent { get; init; } = null!;
    public string? NormalizedContent { get; init; }
    public string Status { get; init; } = null!;
    public string? ErrorMessage { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }

    public static ExpenseInputDto FromDomain(Domain.Aggregates.ExpenseInput.ExpenseInput expenseInput)
    {
        return new ExpenseInputDto
        {
            Id = expenseInput.Id,
            UserId = expenseInput.UserId,
            InputType = expenseInput.InputType.ToString(),
            RawContent = expenseInput.RawContent,
            NormalizedContent = expenseInput.NormalizedContent,
            Status = expenseInput.Status.ToString(),
            ErrorMessage = expenseInput.ErrorMessage,
            CreatedAt = expenseInput.CreatedAt,
            ProcessedAt = expenseInput.ProcessedAt
        };
    }
}
