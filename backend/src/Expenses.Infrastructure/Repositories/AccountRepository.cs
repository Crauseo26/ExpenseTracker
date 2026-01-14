using Microsoft.EntityFrameworkCore;
using Expenses.Domain.Aggregates.Account;
using Expenses.Domain.Interfaces;
using Expenses.Infrastructure.Persistence;

namespace Expenses.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> GetByIdAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Account>()
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Account>()
            .Where(a => a.UserId == userId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Account>> GetByExpenseGroupIdAsync(Guid expenseGroupId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Account>()
            .Where(a => a.ExpenseGroupId == expenseGroupId && a.UserId == userId)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Account> AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        await _context.Set<Account>().AddAsync(account, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return account;
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Set<Account>().Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        _context.Set<Account>().Update(account);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
