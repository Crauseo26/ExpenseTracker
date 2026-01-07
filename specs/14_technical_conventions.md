# Technical Conventions

## Purpose
This document captures **emergent technical conventions** established during development. These conventions ensure consistency across the codebase.

Unlike architectural specs (which are prescriptive), these conventions are **descriptive** — they document decisions made during implementation to maintain consistency going forward.

This is a **living document** that evolves as the project grows.

---

## Ownership & Update Protocol

### Lead Agent Responsibilities
- Document new conventions when agents make technical decisions
- Propose convention updates when patterns emerge
- Commit convention changes with proper attribution

### Human Responsibilities
- Review and validate proposed conventions during code review
- Adjust or reject conventions that don't align with project goals
- Consolidate conventions when they stabilize

### Update Format
All updates must include:
- Date of change
- What convention was added/modified
- Rationale (why this decision was made)

---

## General Code Conventions

### Language & Platform
- Backend: C# / .NET 8+
- Target Framework: net8.0
- Language Version: Latest stable (C# 12)

### Code Style
- Follow standard .NET conventions
- Use nullable reference types
- Enable warnings as errors
- Prefer explicit over implicit typing for clarity

---

## Naming Conventions

### C# Code

**Classes & Records:**
```csharp
// Domain entities
public class Expense { }
public record ExpenseCreated { }

// Value objects
public record Money { }

// Interfaces
public interface IExpenseRepository { }
```

**Methods & Properties:**
```csharp
// Public members: PascalCase
public void CreateExpense() { }
public string Description { get; set; }

// Private fields: _camelCase
private readonly IExpenseRepository _repository;

// Local variables: camelCase
var expenseId = Guid.NewGuid();
```

**Constants & Enums:**
```csharp
// Constants: PascalCase
public const int MaxDescriptionLength = 500;

// Enums: PascalCase (singular)
public enum ExpenseStatus
{
    PendingReview,
    Confirmed
}
```

### Database Naming

**Tables:**
```sql
-- snake_case, plural
expenses
expense_groups
expense_inputs
```

**Columns:**
```sql
-- snake_case
user_id
created_at
purchase_date
expense_type
```

**Foreign Keys:**
```sql
-- table_id format
account_id (references accounts.id)
user_id (references users.id)
```

### Files & Folders

**Project Structure:**
```
PascalCase for folders
PascalCase for files matching class names
One class per file (primary guideline)
```

---

## Project Structure

### Backend Solution Structure

```
backend/
└── src/
    ├── Expenses.Domain/
    │   ├── Aggregates/
    │   │   ├── Expense/
    │   │   ├── Account/
    │   │   ├── ExpenseGroup/
    │   │   └── User/
    │   ├── ValueObjects/
    │   ├── Exceptions/
    │   └── Interfaces/
    │
    ├── Expenses.Application/
    │   ├── Commands/
    │   ├── Queries/
    │   ├── DTOs/
    │   ├── Services/
    │   ├── Interfaces/
    │   └── DependencyInjection/
    │
    ├── Expenses.Infrastructure/
    │   ├── Persistence/
    │   │   ├── Configurations/
    │   │   └── Migrations/
    │   ├── Repositories/
    │   ├── Transactions/
    │   └── DependencyInjection/
    │
    └── Expenses.Api/
        ├── Controllers/
        ├── DTOs/
        ├── Filters/
        ├── Middleware/
        └── DependencyInjection/
```

### File Organization Rules

**Domain Layer:**
- Aggregate roots in `Aggregates/[AggregateName]/`
- Each aggregate folder contains related entities and enums
- Value objects in `ValueObjects/`
- Domain exceptions in `Exceptions/`

**Application Layer:**
- Commands: one file per command
- Queries: one file per query
- DTOs: grouped by feature/aggregate

**Infrastructure Layer:**
- Entity configurations in `Persistence/Configurations/`
- Repositories in `Repositories/`
- One repository per aggregate root

**API Layer:**
- One controller per resource
- Request/Response DTOs in `DTOs/`

---

## Error Handling

### Domain Layer

**Domain Exceptions:**
```csharp
public class DomainException : Exception
{
    public string ErrorCode { get; }
    
    public DomainException(string errorCode, string message) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}

// Usage
throw new DomainException(
    ErrorCodes.InvalidExpenseTransition,
    "Cannot transition from Confirmed to PendingReview"
);
```

**Error Codes:**
```csharp
public static class ErrorCodes
{
    // Domain errors: 1xxx
    public const string InvalidExpenseTransition = "DOM1001";
    public const string InvalidMoneyAmount = "DOM1002";
    
    // Application errors: 2xxx
    public const string ExpenseNotFound = "APP2001";
    
    // Infrastructure errors: 3xxx
    public const string DatabaseConnectionFailed = "INF3001";
}
```

### Application Layer

**Application Exceptions:**
```csharp
public class ApplicationException : Exception
{
    public string ErrorCode { get; }
    
    // Similar structure to DomainException
}
```

### API Layer

**Error Response Format:**
```json
{
  "errorCode": "DOM1001",
  "message": "Cannot transition from Confirmed to PendingReview",
  "details": {
    "field": "status",
    "attemptedTransition": "Confirmed -> PendingReview"
  },
  "timestamp": "2025-01-06T10:30:00Z"
}
```

---

## Dependency Injection

### Registration Pattern

**Each layer provides extension method:**

```csharp
// Domain layer
public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Register domain services here
        return services;
    }
}

// Application layer
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IExpenseService, ExpenseService>();
        return services;
    }
}

// Infrastructure layer
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        return services;
    }
}
```

**Startup/Program.cs:**
```csharp
builder.Services
    .AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);
```

---

## Entity Framework Core

### DbContext Organization

**Single DbContext:**
```csharp
public class AppDbContext : DbContext
{
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<ExpenseGroup> ExpenseGroups { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Apply global query filters (soft delete)
        ApplyGlobalQueryFilters(modelBuilder);
    }
}
```

### Entity Configurations

**Separate configuration classes:**
```csharp
public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");
        
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Id)
            .HasColumnName("id");
            
        builder.OwnsOne(e => e.Amount, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("amount")
                .HasColumnType("decimal(12,2)");
                
            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3);
        });
        
        // Soft delete
        builder.Property(e => e.DeletedAt)
            .HasColumnName("deleted_at");
            
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
```

### Migration Naming

```bash
# Format: YYYYMMDDHHMMSS_DescriptiveName
20250106_InitialCreate
20250107_AddExpenseInputs
20250108_AddConfidenceScoring
```

---

## API Conventions

### Controller Structure

```csharp
[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    
    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        // Implementation
    }
}
```

### HTTP Status Codes

- `200 OK`: Successful GET/PUT
- `201 Created`: Successful POST
- `204 No Content`: Successful DELETE
- `400 Bad Request`: Validation error
- `401 Unauthorized`: Not authenticated
- `403 Forbidden`: Not authorized
- `404 Not Found`: Resource not found
- `409 Conflict`: Business rule violation
- `500 Internal Server Error`: Unexpected error

### Request/Response Naming

```csharp
// Request DTOs: [Action][Resource]Request
public record CreateExpenseRequest;
public record UpdateExpenseRequest;

// Response DTOs: [Resource]Dto or [Resource]Response
public record ExpenseDto;
public record ExpenseListResponse;
```

---

## Testing Conventions

### Test Project Structure

```
tests/
├── Expenses.Domain.Tests/
├── Expenses.Application.Tests/
├── Expenses.Infrastructure.Tests/
└── Expenses.Api.Tests/
```

### Test Naming

```csharp
// Pattern: MethodName_Scenario_ExpectedBehavior
[Fact]
public void CreateExpense_WithValidData_ReturnsExpense()
{
    // Arrange
    // Act
    // Assert
}

[Fact]
public void TransitionToConfirmed_FromPendingReview_SuccessfullyChangesState()
{
    // Test implementation
}
```

---

## Git Conventions

### Commit Messages

Already defined in `specs/13_git_workflow_and_review_protocol.md`

Key points:
- One commit per logical change
- Include agent attribution
- Include backlog reference

### Branch Naming

Already defined in `specs/13_git_workflow_and_review_protocol.md`

Format: `feature/<backlog-id>-<description>`

---

## Documentation Conventions

### Code Comments

**When to comment:**
- Complex business rules
- Non-obvious design decisions
- Workarounds or hacks (with TODO if temporary)

**When NOT to comment:**
- Self-explanatory code
- Obvious implementations

**Format:**
```csharp
// Business Rule: Month and year cannot be changed after confirmation
// See: specs/08_expense-lifecycle.md
public void UpdatePurchaseDate(ExpenseDate newDate)
{
    if (Status == ExpenseStatus.Confirmed && 
        (newDate.Month != PurchaseDate.Month || newDate.Year != PurchaseDate.Year))
    {
        throw new DomainException(
            ErrorCodes.InvalidPurchaseDateEdit,
            "Cannot change month or year of confirmed expense"
        );
    }
    
    PurchaseDate = newDate;
}
```

### XML Documentation

**Public APIs require XML docs:**
```csharp
/// <summary>
/// Creates a new expense for the specified user.
/// </summary>
/// <param name="userId">The user who owns the expense</param>
/// <param name="amount">The monetary amount</param>
/// <returns>The created expense</returns>
/// <exception cref="DomainException">Thrown when amount is invalid</exception>
public Expense CreateExpense(Guid userId, Money amount)
{
    // Implementation
}
```

---

## Changelog

### 2025-01-06
- Initial technical conventions document created
- Established naming conventions for C#, database, and files
- Defined project structure for all layers
- Documented error handling patterns
- Defined DI registration patterns
- Established EF Core conventions
- Defined API conventions
- Defined testing structure
- Cross-referenced Git workflow conventions

### Future Updates
[Lead Agent will document new conventions here as they emerge during development]

---

## Notes for Agents

**Before writing code:**
1. Read this document
2. Follow established conventions
3. If facing ambiguity, propose a convention to Lead Agent
4. Do not create conflicting patterns

**When establishing new patterns:**
1. Document the decision
2. Notify Lead Agent
3. Lead Agent will update this document
4. Human will validate during review

