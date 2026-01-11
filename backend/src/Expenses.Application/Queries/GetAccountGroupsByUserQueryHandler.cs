using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetAccountGroupsByUserQueryHandler
{
    private readonly IExpenseGroupRepository _expenseGroupRepository;

    public GetAccountGroupsByUserQueryHandler(IExpenseGroupRepository expenseGroupRepository)
    {
        _expenseGroupRepository = expenseGroupRepository;
    }

    public async Task<GetAccountGroupsByUserResult> HandleAsync(GetAccountGroupsByUserQuery query, CancellationToken cancellationToken = default)
    {
        var expenseGroups = await _expenseGroupRepository.GetByUserIdAsync(query.UserId, cancellationToken);

        return new GetAccountGroupsByUserResult
        {
            Success = true,
            AccountGroups = expenseGroups.Select(AccountGroupDto.FromDomain)
        };
    }
}
