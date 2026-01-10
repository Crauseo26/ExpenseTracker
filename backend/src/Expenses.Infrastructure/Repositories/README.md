# Repository Implementation Guidelines

## User Scoping Enforcement

All repositories in this system MUST enforce user scoping to ensure data isolation between users.

### Key Principles

1. **Every domain entity (except User) must have a UserId property**
2. **All repository queries MUST filter by UserId**
3. **No cross-user data access is allowed**

### Implementation Pattern

For future repositories (e.g., ExpenseRepository, AccountRepository), follow this pattern:

```csharp
public class ExpenseRepository : IExpenseRepository
{
    private readonly AppDbContext _context;

    public ExpenseRepository(AppDbContext context)
    {
        _context = context;
    }

    // CORRECT: Always filter by UserId
    public async Task<Expense?> GetByIdAsync(Guid expenseId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Expenses
            .Where(e => e.Id == expenseId && e.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // CORRECT: List operations always scoped to user
    public async Task<List<Expense>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Expenses
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}
```

### Anti-Patterns (FORBIDDEN)

```csharp
// WRONG: No user scoping
public async Task<Expense?> GetByIdAsync(Guid expenseId)
{
    return await _context.Expenses
        .FirstOrDefaultAsync(e => e.Id == expenseId);
}

// WRONG: Exposing IQueryable without user filter
public IQueryable<Expense> GetAll()
{
    return _context.Expenses.AsQueryable();
}
```

### Current Implementation

- **UserRepository**: Implements user management without additional scoping (users query themselves)
- Future repositories will require UserId parameter in all query methods

### References

- Domain Model: `/specs/03_domain_model.md` (lines 10-23)
- Persistence Model: `/specs/05_persistence_model.md`
- Engineering Guardrails: `/specs/11_engineering_guardrails.md`
