using Expenses.Application.DTOs;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;

namespace Expenses.Application.Commands;

public class ConfirmExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public ConfirmExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<ConfirmExpenseResult> HandleAsync(ConfirmExpenseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expense = await _expenseRepository.GetByIdAsync(command.ExpenseId, command.UserId, cancellationToken);

            if (expense == null)
            {
                return new ConfirmExpenseResult
                {
                    Success = false,
                    ErrorMessage = "Expense not found"
                };
            }

            expense.Confirm();

            await _expenseRepository.UpdateAsync(expense, cancellationToken);

            return new ConfirmExpenseResult
            {
                Success = true,
                Expense = ExpenseDto.FromDomain(expense)
            };
        }
        catch (DomainException ex)
        {
            return new ConfirmExpenseResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
