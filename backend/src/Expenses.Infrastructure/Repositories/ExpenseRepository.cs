using Microsoft.EntityFrameworkCore;
using Expenses.Domain.Aggregates.Expense;
using Expenses.Domain.Interfaces;
using Expenses.Infrastructure.Persistence;

namespace Expenses.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(Guid expenseId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Expense>()
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Expense>()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Expense>()
            .Where(e => e.UserId == userId && e.PurchaseDate >= startDate && e.PurchaseDate <= endDate)
            .OrderByDescending(e => e.PurchaseDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await _context.Set<Expense>().AddAsync(expense, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return expense;
    }

    public async Task UpdateAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _context.Set<Expense>().Update(expense);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        _context.Set<Expense>().Update(expense);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
