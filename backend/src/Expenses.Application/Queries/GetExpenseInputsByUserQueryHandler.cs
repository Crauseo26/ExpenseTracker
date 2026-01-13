using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetExpenseInputsByUserQueryHandler
{
    private readonly IExpenseInputRepository _expenseInputRepository;

    public GetExpenseInputsByUserQueryHandler(IExpenseInputRepository expenseInputRepository)
    {
        _expenseInputRepository = expenseInputRepository;
    }

    public async Task<GetExpenseInputsByUserResult> HandleAsync(
        GetExpenseInputsByUserQuery query,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Domain.Aggregates.ExpenseInput.ExpenseInput> expenseInputs;

        if (query.PendingOnly)
        {
            expenseInputs = await _expenseInputRepository.GetPendingByUserIdAsync(
                query.UserId,
                cancellationToken);
        }
        else
        {
            expenseInputs = await _expenseInputRepository.GetByUserIdAsync(
                query.UserId,
                cancellationToken);
        }

        return new GetExpenseInputsByUserResult
        {
            Success = true,
            ExpenseInputs = expenseInputs.Select(ExpenseInputDto.FromDomain)
        };
    }
}
