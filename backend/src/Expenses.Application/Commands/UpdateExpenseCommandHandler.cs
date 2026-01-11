using Expenses.Application.DTOs;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.Commands;

public class UpdateExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public UpdateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<UpdateExpenseResult> HandleAsync(UpdateExpenseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var expense = await _expenseRepository.GetByIdAsync(command.ExpenseId, command.UserId, cancellationToken);

            if (expense == null)
            {
                return new UpdateExpenseResult
                {
                    Success = false,
                    ErrorMessage = "Expense not found"
                };
            }

            if (!Enum.TryParse<Currency>(command.Currency, out var currency))
            {
                return new UpdateExpenseResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid currency: {command.Currency}"
                };
            }

            if (!Enum.TryParse<ExpenseType>(command.ExpenseType, out var expenseType))
            {
                return new UpdateExpenseResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid expense type: {command.ExpenseType}"
                };
            }

            var money = new Money(command.Amount, currency);

            expense.Update(
                command.AccountId,
                money,
                command.Description,
                expenseType,
                command.PurchaseDate
            );

            await _expenseRepository.UpdateAsync(expense, cancellationToken);

            return new UpdateExpenseResult
            {
                Success = true,
                Expense = ExpenseDto.FromDomain(expense)
            };
        }
        catch (DomainException ex)
        {
            return new UpdateExpenseResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
