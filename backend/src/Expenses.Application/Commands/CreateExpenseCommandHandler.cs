using Expenses.Application.DTOs;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.Commands;

public class CreateExpenseCommandHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<CreateExpenseResult> HandleAsync(CreateExpenseCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Enum.TryParse<Currency>(command.Currency, out var currency))
            {
                return new CreateExpenseResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid currency: {command.Currency}"
                };
            }

            if (!Enum.TryParse<ExpenseType>(command.ExpenseType, out var expenseType))
            {
                return new CreateExpenseResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid expense type: {command.ExpenseType}"
                };
            }

            var money = new Money(command.Amount, currency);

            var expense = Domain.Aggregates.Expense.Expense.CreateManual(
                command.UserId,
                command.AccountId,
                money,
                command.Description,
                expenseType,
                command.PurchaseDate
            );

            await _expenseRepository.AddAsync(expense, cancellationToken);

            return new CreateExpenseResult
            {
                Success = true,
                Expense = ExpenseDto.FromDomain(expense)
            };
        }
        catch (DomainException ex)
        {
            return new CreateExpenseResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
