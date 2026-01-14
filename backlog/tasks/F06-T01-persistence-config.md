# F06-T01: Configure DbContext and Entity Mappings

## Task Metadata
- **Feature**: F06 — Persistence & Migrations
- **Task ID**: F06-T01
- **Agent**: Backend-Infrastructure-Agent
- **Status**: In Progress
- **Branch**: feature/F06-T01-persistence-config
- **Dependencies**: F01-T01, F02-T01, F03-T01, F04-T01 (all domain aggregates)

---

## Objective
Configure Entity Framework Core DbContext and create entity type configurations for all domain aggregates, mapping them to relational tables per the persistence model specification.

---

## Context
- All domain aggregates are implemented: User, Expense, Account, AccountGroup, ExpenseInput
- AppDbContext already exists with Identity configuration
- Need to add EF Core configurations for domain entities
- Must handle Value Objects (Money) correctly
- Must configure soft delete, indexes, and foreign keys

---

## Requirements

### 1. Entity Type Configurations

Create `IEntityTypeConfiguration<T>` for each aggregate:

#### Expense Configuration
- Table name: `Expenses`
- Primary key: `Id` (UUID)
- Properties:
  - `UserId` (UUID, required, FK, indexed)
  - `AccountId` (UUID, required, FK)
  - `ExpenseInputId` (UUID, nullable, FK)
  - `Amount` (decimal(12,2), required, > 0)
  - `Currency` (string, required, max 3 chars)
  - `Description` (string, required)
  - `ExpenseType` (string, required)
  - `PurchaseDate` (DateTime, required)
  - `Status` (string, required)
  - `CreatedAt` (DateTime, required)
  - `UpdatedAt` (DateTime, required)
  - `DeletedAt` (DateTime, nullable)
- Value Object: `Money` → map to `Amount` and `Currency` columns using `OwnsOne`
- Enums: Convert to string storage
- Indexes: `UserId`, `AccountId`, `PurchaseDate`, `Status`
- Foreign keys: `DeleteBehavior.Restrict`
- Query filter: Exclude soft-deleted records

#### Account Configuration
- Table name: `Accounts`
- Primary key: `Id` (UUID)
- Properties:
  - `UserId` (UUID, required, FK, indexed)
  - `ExpenseGroupId` (UUID, required, FK)
  - `Name` (string, required, max 200)
  - `CreatedAt` (DateTime, required)
  - `UpdatedAt` (DateTime, required)
  - `DeletedAt` (DateTime, nullable)
- Indexes: `UserId`, `ExpenseGroupId`
- Foreign keys: `DeleteBehavior.Restrict`
- Query filter: Exclude soft-deleted records

#### AccountGroup (ExpenseGroup) Configuration
- Table name: `ExpenseGroups`
- Primary key: `Id` (UUID)
- Properties:
  - `UserId` (UUID, required, FK, indexed)
  - `Name` (string, required, max 200)
  - `CreatedAt` (DateTime, required)
  - `UpdatedAt` (DateTime, required)
  - `DeletedAt` (DateTime, nullable)
- Indexes: `UserId`
- Query filter: Exclude soft-deleted records

#### ExpenseInput Configuration
- Table name: `ExpenseInputs`
- Primary key: `Id` (UUID)
- Properties:
  - `UserId` (UUID, required, FK, indexed)
  - `InputType` (string, required)
  - `RawContent` (string, required, text)
  - `NormalizedContent` (string, nullable, text)
  - `ProcessingStatus` (string, required)
  - `ErrorMessage` (string, nullable)
  - `CreatedAt` (DateTime, required)
  - `ProcessedAt` (DateTime, nullable)
  - `DeletedAt` (DateTime, nullable)
- Indexes: `UserId`, `CreatedAt`
- Query filter: Exclude soft-deleted records

### 2. AppDbContext Updates
- Register all entity configurations in `OnModelCreating`
- Keep existing Identity configuration
- Apply configurations using `modelBuilder.ApplyConfiguration(new XxxConfiguration())`

### 3. Value Object Mapping
For `Money` value object in Expense:
```csharp
builder.OwnsOne(e => e.Amount, money =>
{
    money.Property(m => m.Amount)
        .HasColumnName("Amount")
        .HasColumnType("decimal(12,2)")
        .IsRequired();
    
    money.Property(m => m.Currency)
        .HasColumnName("Currency")
        .HasMaxLength(3)
        .IsRequired()
        .HasConversion<string>();
});
```

### 4. Enum Conversions
Convert all enums to string storage:
```csharp
.HasConversion<string>()
```

### 5. Soft Delete Query Filters
Apply global query filter for each entity:
```csharp
builder.HasQueryFilter(e => e.DeletedAt == null);
```

### 6. Foreign Key Configuration
- Use `DeleteBehavior.Restrict` to prevent accidental cascade deletes
- Configure relationships explicitly

---

## Implementation Checklist

- [ ] Create `Persistence/Configurations/ExpenseConfiguration.cs`
- [ ] Create `Persistence/Configurations/AccountConfiguration.cs`
- [ ] Create `Persistence/Configurations/ExpenseGroupConfiguration.cs`
- [ ] Create `Persistence/Configurations/ExpenseInputConfiguration.cs`
- [ ] Update `AppDbContext.OnModelCreating` to apply configurations
- [ ] Configure Money value object with OwnsOne
- [ ] Configure all enums with string conversion
- [ ] Add query filters for soft delete
- [ ] Add indexes for UserId on all entities
- [ ] Add indexes for foreign keys and query columns
- [ ] Verify build succeeds

---

## Acceptance Criteria

1. ✅ All domain aggregates have `IEntityTypeConfiguration<T>` implementations
2. ✅ Money value object correctly mapped to Amount/Currency columns
3. ✅ All enums stored as strings
4. ✅ Soft delete query filters applied to all entities
5. ✅ UserId indexed on all entities for user scoping
6. ✅ Foreign keys configured with DeleteBehavior.Restrict
7. ✅ Table names follow convention (plural PascalCase)
8. ✅ Build succeeds with no warnings or errors
9. ✅ Code follows technical conventions (14_technical_conventions.md)

---

## Technical Notes

### EF Core Configuration Best Practices
- Use `IEntityTypeConfiguration<T>` for separation of concerns
- Keep configurations in separate files per entity
- Use fluent API for all mappings (avoid data annotations on domain entities)
- Configure value objects with `OwnsOne` or property conversions
- Always specify column types explicitly for decimals

### Value Object Handling
- `Money` is a value object, not a separate table
- Use `OwnsOne` to map to columns in parent table
- Configure column names explicitly to match persistence spec

### Soft Delete Pattern
- Query filters automatically exclude deleted records
- Use `IgnoreQueryFilters()` when needed to include deleted records
- DeletedAt is nullable DateTime

### User Scoping
- All entities have UserId foreign key
- Index on UserId for efficient filtering
- Repositories will use UserId in all queries

---

## References
- `specs/05_persistence_model.md` - Persistence model specification
- `specs/03_domain_model.md` - Domain model definition
- `specs/14_technical_conventions.md` - Code style and naming
- `Expenses.Infrastructure.Persistence.AppDbContext` - Existing DbContext

---

## Agent Notes
- This is Infrastructure layer work (persistence configuration)
- Do not modify domain entities (no data annotations)
- Use fluent API exclusively
- Each configuration should be in its own file
- Commit each configuration separately for atomic changes
