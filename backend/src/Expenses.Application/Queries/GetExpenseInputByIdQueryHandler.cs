using Expenses.Application.DTOs;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Queries;

public class GetExpenseInputByIdQueryHandler
{
    private readonly IExpenseInputRepository _expenseInputRepository;

    public GetExpenseInputByIdQueryHandler(IExpenseInputRepository expenseInputRepository)
    {
        _expenseInputRepository = expenseInputRepository;
    }

    public async Task<GetExpenseInputByIdResult> HandleAsync(
        GetExpenseInputByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var expenseInput = await _expenseInputRepository.GetByIdAsync(
            query.ExpenseInputId,
            query.UserId,
            cancellationToken);

        if (expenseInput == null)
        {
            return new GetExpenseInputByIdResult
            {
                Success = false,
                ErrorMessage = "Expense input not found"
            };
        }

        return new GetExpenseInputByIdResult
        {
            Success = true,
            ExpenseInput = ExpenseInputDto.FromDomain(expenseInput)
        };
    }
}
