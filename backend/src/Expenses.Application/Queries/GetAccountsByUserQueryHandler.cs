using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetAccountsByUserQueryHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsByUserQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountsByUserResult> HandleAsync(GetAccountsByUserQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Aggregates.Account.Account> accounts;

        if (query.ExpenseGroupId.HasValue)
        {
            accounts = await _accountRepository.GetByExpenseGroupIdAsync(
                query.ExpenseGroupId.Value,
                query.UserId,
                cancellationToken);
        }
        else
        {
            accounts = await _accountRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        }

        return new GetAccountsByUserResult
        {
            Success = true,
            Accounts = accounts.Select(AccountDto.FromDomain)
        };
    }
}
