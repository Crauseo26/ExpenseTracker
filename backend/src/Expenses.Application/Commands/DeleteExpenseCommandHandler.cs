using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class DeleteExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public DeleteExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<DeleteExpenseResult> HandleAsync(DeleteExpenseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expense = await _expenseRepository.GetByIdAsync(command.ExpenseId, command.UserId, cancellationToken);

            if (expense == null)
            {
                return new DeleteExpenseResult
                {
                    Success = false,
                    ErrorMessage = "Expense not found"
                };
            }

            expense.SoftDelete();

            await _expenseRepository.DeleteAsync(expense, cancellationToken);

            return new DeleteExpenseResult
            {
                Success = true
            };
        }
        catch (DomainException ex)
        {
            return new DeleteExpenseResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
