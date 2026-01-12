namespace Expenses.Application.Commands;

public record DeleteAccountGroupCommand
{
    public Guid AccountGroupId { get; init; }
    public Guid UserId { get; init; }
}

public record DeleteAccountGroupResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}
