using Expenses.Application.DTOs;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class UpdateAccountCommandHandler
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<UpdateAccountResult> HandleAsync(UpdateAccountCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(command.AccountId, command.UserId, cancellationToken);

            if (account == null)
            {
                return new UpdateAccountResult
                {
                    Success = false,
                    ErrorMessage = "Account not found"
                };
            }

            account.UpdateName(command.Name);
            account.ReassignToExpenseGroup(command.ExpenseGroupId);

            await _accountRepository.UpdateAsync(account, cancellationToken);

            return new UpdateAccountResult
            {
                Success = true,
                Account = AccountDto.FromDomain(account)
            };
        }
        catch (DomainException ex)
        {
            return new UpdateAccountResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
