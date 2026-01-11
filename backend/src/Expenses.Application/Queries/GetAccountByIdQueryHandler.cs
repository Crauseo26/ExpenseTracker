using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetAccountByIdQueryHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountByIdResult> HandleAsync(GetAccountByIdQuery query, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(query.AccountId, query.UserId, cancellationToken);

        if (account == null)
        {
            return new GetAccountByIdResult
            {
                Success = false,
                ErrorMessage = "Account not found"
            };
        }

        return new GetAccountByIdResult
        {
            Success = true,
            Account = AccountDto.FromDomain(account)
        };
    }
}
