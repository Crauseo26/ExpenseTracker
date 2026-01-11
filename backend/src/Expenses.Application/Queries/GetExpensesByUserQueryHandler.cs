using Expenses.Application.DTOs;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetExpensesByUserQueryHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpensesByUserQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<GetExpensesByUserResult> HandleAsync(GetExpensesByUserQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Expense> expenses;

        if (query.StartDate.HasValue && query.EndDate.HasValue)
        {
            expenses = await _expenseRepository.GetByUserIdAndDateRangeAsync(
                query.UserId,
                query.StartDate.Value,
                query.EndDate.Value,
                cancellationToken);
        }
        else
        {
            expenses = await _expenseRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        }

        return new GetExpensesByUserResult
        {
            Success = true,
            Expenses = expenses.Select(ExpenseDto.FromDomain)
        };
    }
}
