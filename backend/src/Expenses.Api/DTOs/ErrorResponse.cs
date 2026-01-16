namespace Expenses.Api.DTOs;

public record ErrorResponse
{
    public string Message { get; init; } = null!;
    public string? ErrorCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
