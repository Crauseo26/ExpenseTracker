using Microsoft.EntityFrameworkCore;
using Expenses.Domain.Aggregates.ExpenseInput;
using Expenses.Domain.Interfaces;
using Expenses.Infrastructure.Persistence;

namespace Expenses.Infrastructure.Repositories;

public class ExpenseInputRepository : IExpenseInputRepository
{
    private readonly AppDbContext _context;

    public ExpenseInputRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseInput?> GetByIdAsync(Guid expenseInputId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ExpenseInput>()
            .FirstOrDefaultAsync(ei => ei.Id == expenseInputId && ei.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<ExpenseInput>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ExpenseInput>()
            .Where(ei => ei.UserId == userId)
            .OrderByDescending(ei => ei.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExpenseInput>> GetPendingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ExpenseInput>()
            .Where(ei => ei.UserId == userId && ei.Status == ProcessingStatus.PENDING)
            .OrderByDescending(ei => ei.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default)
    {
        await _context.Set<ExpenseInput>().AddAsync(expenseInput, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default)
    {
        _context.Set<ExpenseInput>().Update(expenseInput);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ExpenseInput expenseInput, CancellationToken cancellationToken = default)
    {
        _context.Set<ExpenseInput>().Update(expenseInput);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
