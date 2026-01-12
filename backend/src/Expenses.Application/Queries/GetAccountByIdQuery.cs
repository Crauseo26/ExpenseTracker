using Expenses.Application.DTOs;

namespace Expenses.Application.Queries;

public record GetAccountByIdQuery
{
    public Guid AccountId { get; init; }
    public Guid UserId { get; init; }
}

public record GetAccountByIdResult
{
    public bool Success { get; init; }
    public AccountDto? Account { get; init; }
    public string? ErrorMessage { get; init; }
}
