namespace Expenses.Application.Commands;

public record DeleteAccountCommand
{
    public Guid AccountId { get; init; }
    public Guid UserId { get; init; }
}

public record DeleteAccountResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
