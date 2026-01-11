using Expenses.Domain.Aggregates.ExpenseGroup;

namespace Expenses.Domain.Interfaces;

public interface IExpenseGroupRepository
{
    Task<ExpenseGroup?> GetByIdAsync(Guid expenseGroupId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseGroup>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ExpenseGroup> AddAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default);
    Task DeleteAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default);
}
