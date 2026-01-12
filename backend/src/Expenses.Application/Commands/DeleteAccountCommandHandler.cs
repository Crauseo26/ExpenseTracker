using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class DeleteAccountCommandHandler
{
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<DeleteAccountResult> HandleAsync(DeleteAccountCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(command.AccountId, command.UserId, cancellationToken);

            if (account == null)
            {
                return new DeleteAccountResult
                {
                    Success = false,
                    ErrorMessage = "Account not found"
                };
            }

            account.SoftDelete();

            await _accountRepository.DeleteAsync(account, cancellationToken);

            return new DeleteAccountResult
            {
                Success = true
            };
        }
        catch (DomainException ex)
        {
            return new DeleteAccountResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
