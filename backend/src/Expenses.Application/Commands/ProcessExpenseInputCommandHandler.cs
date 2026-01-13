using Expenses.Application.DTOs;
using Expenses.Application.Interfaces;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Aggregates.ExpenseInput;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.Commands;

public class ProcessExpenseInputCommandHandler
{
    private readonly IExpenseInputRepository _expenseInputRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IAIOrchestrationService _aiOrchestrationService;
    private const double ConfidenceThreshold = 0.87;

    public ProcessExpenseInputCommandHandler(
        IExpenseInputRepository expenseInputRepository,
        IExpenseRepository expenseRepository,
        IAIOrchestrationService aiOrchestrationService)
    {
        _expenseInputRepository = expenseInputRepository;
        _expenseRepository = expenseRepository;
        _aiOrchestrationService = aiOrchestrationService;
    }

    public async Task<ProcessExpenseInputResult> HandleAsync(
        ProcessExpenseInputCommand command,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Enum.TryParse<InputType>(command.InputType, true, out var inputType))
            {
                return new ProcessExpenseInputResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid input type: {command.InputType}"
                };
            }

            var expenseInput = ExpenseInput.Create(command.UserId, inputType, command.RawContent);
            await _expenseInputRepository.AddAsync(expenseInput, cancellationToken);

            expenseInput.SetNormalizedContent(command.RawContent);
            await _expenseInputRepository.UpdateAsync(expenseInput, cancellationToken);

            AIProposalResponseDto aiResponse;
            try
            {
                aiResponse = await _aiOrchestrationService.ProcessInputAsync(
                    command.UserId,
                    command.InputType,
                    expenseInput.NormalizedContent!,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                expenseInput.MarkAsError($"AI processing failed: {ex.Message}");
                await _expenseInputRepository.UpdateAsync(expenseInput, cancellationToken);

                return new ProcessExpenseInputResult
                {
                    Success = false,
                    ExpenseInput = ExpenseInputDto.FromDomain(expenseInput),
                    ErrorMessage = $"AI processing failed: {ex.Message}"
                };
            }

            var createdExpenses = new List<ExpenseDto>();

            foreach (var proposal in aiResponse.Proposals)
            {
                try
                {
                    if (!Enum.TryParse<Currency>(proposal.Currency, true, out var currency))
                    {
                        continue;
                    }

                    if (!Enum.TryParse<ExpenseType>(proposal.ExpenseType, true, out var expenseType))
                    {
                        continue;
                    }

                    var money = new Money(proposal.Amount, currency);
                    var expense = Expense.CreateFromAI(
                        command.UserId,
                        proposal.AccountId,
                        money,
                        proposal.Description,
                        expenseType,
                        proposal.PurchaseDate,
                        expenseInput.Id,
                        proposal.Confidence,
                        ConfidenceThreshold);

                    await _expenseRepository.AddAsync(expense, cancellationToken);
                    createdExpenses.Add(ExpenseDto.FromDomain(expense));
                }
                catch (DomainException)
                {
                    continue;
                }
            }

            expenseInput.MarkAsProcessed();
            await _expenseInputRepository.UpdateAsync(expenseInput, cancellationToken);

            return new ProcessExpenseInputResult
            {
                Success = true,
                ExpenseInput = ExpenseInputDto.FromDomain(expenseInput),
                CreatedExpenses = createdExpenses
            };
        }
        catch (DomainException ex)
        {
            return new ProcessExpenseInputResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
