using Expenses.Application.DTOs;
using Expenses.Domain.Aggregates.ExpenseGroup;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class CreateAccountGroupCommandHandler
{
    private readonly IExpenseGroupRepository _expenseGroupRepository;

    public CreateAccountGroupCommandHandler(IExpenseGroupRepository expenseGroupRepository)
    {
        _expenseGroupRepository = expenseGroupRepository;
    }

    public async Task<CreateAccountGroupResult> HandleAsync(CreateAccountGroupCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expenseGroup = ExpenseGroup.Create(command.UserId, command.Name);

            await _expenseGroupRepository.AddAsync(expenseGroup, cancellationToken);

            return new CreateAccountGroupResult
            {
                Success = true,
                AccountGroup = AccountGroupDto.FromDomain(expenseGroup)
            };
        }
        catch (DomainException ex)
        {
            return new CreateAccountGroupResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
