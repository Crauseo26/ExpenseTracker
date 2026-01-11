using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetAccountGroupByIdQueryHandler
{
    private readonly IExpenseGroupRepository _expenseGroupRepository;

    public GetAccountGroupByIdQueryHandler(IExpenseGroupRepository expenseGroupRepository)
    {
        _expenseGroupRepository = expenseGroupRepository;
    }

    public async Task<GetAccountGroupByIdResult> HandleAsync(GetAccountGroupByIdQuery query, CancellationToken cancellationToken = default)
    {
        var expenseGroup = await _expenseGroupRepository.GetByIdAsync(query.AccountGroupId, query.UserId, cancellationToken);

        if (expenseGroup == null)
        {
            return new GetAccountGroupByIdResult
            {
                Success = false,
                ErrorMessage = "Account group not found"
            };
        }

        return new GetAccountGroupByIdResult
        {
            Success = true,
            AccountGroup = AccountGroupDto.FromDomain(expenseGroup)
        };
    }
}
