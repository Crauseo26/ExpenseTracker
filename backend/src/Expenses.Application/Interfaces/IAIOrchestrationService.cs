using Expenses.Application.DTOs;

namespace Expenses.Application.Interfaces;

public interface IAIOrchestrationService
{
    Task<AIProposalResponseDto> ProcessInputAsync(
        Guid userId,
        string inputType,
        string normalizedContent,
        List<string> availableAccounts,
        CancellationToken cancellationToken = default);
}
