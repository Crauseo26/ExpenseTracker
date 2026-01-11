using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetExpenseByIdQueryHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<GetExpenseByIdResult> HandleAsync(GetExpenseByIdQuery query, CancellationToken cancellationToken = default)
    {
        var expense = await _expenseRepository.GetByIdAsync(query.ExpenseId, query.UserId, cancellationToken);

        if (expense == null)
        {
            return new GetExpenseByIdResult
            {
                Success = false,
                ErrorMessage = "Expense not found"
            };
        }

        return new GetExpenseByIdResult
        {
            Success = true,
            Expense = ExpenseDto.FromDomain(expense)
        };
    }
}
