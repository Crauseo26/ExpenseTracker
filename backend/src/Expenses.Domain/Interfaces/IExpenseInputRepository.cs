using Expenses.Domain.Aggregates.ExpenseInput;

namespace Expenses.Domain.Interfaces;

public interface IExpenseInputRepository
{
    Task<ExpenseInput?> GetByIdAsync(Guid expenseInputId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseInput>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExpenseInput>> GetPendingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default);
    Task DeleteAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default);
}
