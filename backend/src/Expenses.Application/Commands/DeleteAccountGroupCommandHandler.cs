using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class DeleteAccountGroupCommandHandler
{
    private readonly IExpenseGroupRepository _expenseGroupRepository;

    public DeleteAccountGroupCommandHandler(IExpenseGroupRepository expenseGroupRepository)
    {
        _expenseGroupRepository = expenseGroupRepository;
    }

    public async Task<DeleteAccountGroupResult> HandleAsync(DeleteAccountGroupCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expenseGroup = await _expenseGroupRepository.GetByIdAsync(command.AccountGroupId, command.UserId, cancellationToken);

            if (expenseGroup == null)
            {
                return new DeleteAccountGroupResult
                {
                    Success = false,
                    ErrorMessage = "Account group not found"
                };
            }

            expenseGroup.SoftDelete();

            await _expenseGroupRepository.DeleteAsync(expenseGroup, cancellationToken);

            return new DeleteAccountGroupResult
            {
                Success = true
            };
        }
        catch (DomainException ex)
        {
            return new DeleteAccountGroupResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
