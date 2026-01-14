using Microsoft.EntityFrameworkCore;
using Expenses.Domain.Aggregates.ExpenseGroup;
using Expenses.Domain.Interfaces;
using Expenses.Infrastructure.Persistence;

namespace Expenses.Infrastructure.Repositories;

public class ExpenseGroupRepository : IExpenseGroupRepository
{
    private readonly AppDbContext _context;

    public ExpenseGroupRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseGroup?> GetByIdAsync(Guid expenseGroupId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ExpenseGroup>()
            .FirstOrDefaultAsync(eg => eg.Id == expenseGroupId && eg.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<ExpenseGroup>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ExpenseGroup>()
            .Where(eg => eg.UserId == userId)
            .OrderBy(eg => eg.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExpenseGroup> AddAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default)
    {
        await _context.Set<ExpenseGroup>().AddAsync(expenseGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return expenseGroup;
    }

    public async Task UpdateAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default)
    {
        _context.Set<ExpenseGroup>().Update(expenseGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ExpenseGroup expenseGroup, CancellationToken cancellationToken = default)
    {
        _context.Set<ExpenseGroup>().Update(expenseGroup);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
