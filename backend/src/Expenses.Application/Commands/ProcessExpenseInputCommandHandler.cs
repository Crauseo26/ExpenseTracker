using Expenses.Application.DTOs;
using Expenses.Application.Interfaces;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Aggregates.ExpenseInput;
using Expenses.Domain.Exceptions;
using Expenses.Domain.Interfaces;
using Expenses.Domain.Services;
using Expenses.Domain.ValueObjects;

namespace Expenses.Application.Commands;

public class ProcessExpenseInputCommandHandler
{
    private readonly IExpenseInputRepository _expenseInputRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IAIOrchestrationService _aiOrchestrationService;
    private readonly ConfidenceThresholdPolicy _confidencePolicy;

    public ProcessExpenseInputCommandHandler(
        IExpenseInputRepository expenseInputRepository,
        IExpenseRepository expenseRepository,
        IAccountRepository accountRepository,
        IAIOrchestrationService aiOrchestrationService,
        ConfidenceThresholdPolicy confidencePolicy)
    {
        _expenseInputRepository = expenseInputRepository;
        _expenseRepository = expenseRepository;
        _accountRepository = accountRepository;
        _aiOrchestrationService = aiOrchestrationService;
        _confidencePolicy = confidencePolicy;
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

            var userAccounts = await _accountRepository.GetByUserIdAsync(command.UserId, cancellationToken);
            var accountMap = userAccounts.ToDictionary(
                a => a.Name,
                a => a.Id,
                StringComparer.OrdinalIgnoreCase);
            var availableAccountNames = accountMap.Keys.ToList();

            AIProposalResponseDto aiResponse;
            try
            {
                aiResponse = await _aiOrchestrationService.ProcessInputAsync(
                    command.UserId,
                    command.InputType,
                    expenseInput.NormalizedContent!,
                    availableAccountNames,
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
                    var currency = Currency.UYU;
                    if (!Enum.TryParse<Currency>(proposal.Currency, true, out var parsedCurrency))
                    {
                        currency = Currency.UYU;
                    }
                    else
                    {
                        currency = parsedCurrency;
                    }

                    var expenseType = ExpenseType.Sporadic;
                    if (!Enum.TryParse<ExpenseType>(proposal.ExpenseType, true, out var parsedExpenseType))
                    {
                        expenseType = ExpenseType.Sporadic;
                    }
                    else
                    {
                        expenseType = parsedExpenseType;
                    }

                    Guid accountId = proposal.AccountId;
                    if (!string.IsNullOrEmpty(proposal.SuggestedAccountName) &&
                        accountMap.TryGetValue(proposal.SuggestedAccountName, out var matchedAccountId))
                    {
                        accountId = matchedAccountId;
                    }

                    var money = new Money(proposal.Amount, currency);
                    var confidenceScore = new ConfidenceScore(proposal.Confidence);
                    var expense = Expense.CreateFromAI(
                        command.UserId,
                        accountId,
                        money,
                        proposal.Description,
                        expenseType,
                        proposal.PurchaseDate,
                        expenseInput.Id,
                        confidenceScore,
                        _confidencePolicy);

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
