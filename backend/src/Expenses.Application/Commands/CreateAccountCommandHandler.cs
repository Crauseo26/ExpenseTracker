using Expenses.Application.DTOs;
using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class CreateAccountCommandHandler
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<CreateAccountResult> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var account = Account.Create(command.UserId, command.Name, command.ExpenseGroupId);

            await _accountRepository.AddAsync(account, cancellationToken);

            return new CreateAccountResult
            {
                Success = true,
                Account = AccountDto.FromDomain(account)
            };
        }
        catch (DomainException ex)
        {
            return new CreateAccountResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
