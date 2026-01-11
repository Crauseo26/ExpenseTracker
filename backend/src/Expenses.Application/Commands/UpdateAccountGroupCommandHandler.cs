using Expenses.Application.DTOs;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class UpdateAccountGroupCommandHandler
{
    private readonly IExpenseGroupRepository _expenseGroupRepository;

    public UpdateAccountGroupCommandHandler(IExpenseGroupRepository expenseGroupRepository)
    {
        _expenseGroupRepository = expenseGroupRepository;
    }

    public async Task<UpdateAccountGroupResult> HandleAsync(UpdateAccountGroupCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expenseGroup = await _expenseGroupRepository.GetByIdAsync(command.AccountGroupId, command.UserId, cancellationToken);

            if (expenseGroup == null)
            {
                return new UpdateAccountGroupResult
                {
                    Success = false,
                    ErrorMessage = "Account group not found"
                };
            }

            expenseGroup.UpdateName(command.Name);

            await _expenseGroupRepository.UpdateAsync(expenseGroup, cancellationToken);

            return new UpdateAccountGroupResult
            {
                Success = true,
                AccountGroup = AccountGroupDto.FromDomain(expenseGroup)
            };
        }
        catch (DomainException ex)
        {
            return new UpdateAccountGroupResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
