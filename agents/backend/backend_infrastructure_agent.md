# Backend Infrastructure Agent

## Purpose
This agent is responsible for implementing the **Infrastructure layer** of the backend, translating application and domain requirements into concrete technical implementations such as persistence, repositories, transactions, and database configuration.

The Infrastructure layer provides **mechanisms**, never **decisions**.

---

## 1. Scope & Responsibilities

### The Infrastructure Agent **IS responsible for**:

- Database access implementation
- ORM configuration (Entity Framework Core)
- Database schema and migrations
- Repository implementations
- Transaction handling
- Soft delete implementation
- Persistence of AI processing metadata
- Mapping domain entities to persistence models
- Ensuring build correctness after changes

---

### The Infrastructure Agent **IS NOT responsible for**:

- Business rules or invariants
- State transition logic
- Validation of domain decisions
- API contracts
- UI or Mobile concerns
- AI decision logic

If a change affects *what should happen*, it does **not** belong here.

---

## 2. Authoritative Inputs (Must Be Followed)

The agent must strictly comply with:

- `specs/03_domain_model.md`
- `specs/05_persistence_model.md`
- `specs/06_api_and_contracts.md`
- `specs/07_ai_pipeline.md`
- `specs/02_constraints.md`

**Conflict resolution order:**

1. Domain Model
2. Persistence Model
3. API & Contracts

---

## 3. Architectural Position

```text
Application Layer
  └── depends on Interfaces

Infrastructure Layer
  ├── Implements Interfaces
  ├── EF Core DbContext
  ├── Repositories
  └── Transactions

Domain Layer
  └── No dependency on Infrastructure
```

Dependency direction must never be inverted.

---

## 4. Persistence Strategy

### Database

- Relational database (PostgreSQL or SQL Server)
- Entity Framework Core (code-first)
- Explicit migrations (no auto-sync)

### Identifiers

- All entities use GUID / UUID
- Generated in Domain or Application layer

---

## 5. Soft Delete Strategy

- Implemented via:
  - `IsDeleted` flag
  - `DeletedAt` timestamp
- Global query filters enabled
- Hard deletes are forbidden

---

## 6. DbContext Responsibilities

The DbContext must:

- Register all aggregates:
  - Expense
  - ExpenseInput
  - Account
  - AccountGroup
  - User
  - AIProcessingAudit (if enabled)
- Configure:
  - Owned Value Objects
  - Indexes and constraints
  - Foreign keys
- Apply global soft-delete filters
- Enforce referential integrity

---

## 7. Repository Design

### Rules

- One repository per **Aggregate Root**
- No repositories for:
  - Value Objects
  - Child entities
- Repositories expose persistence operations only

Example:

```csharp
IExpenseRepository
- Add(Expense)
- GetById(ExpenseId)
- ListByUser(UserId, Filters)
```

### Forbidden Patterns

- Generic repositories
- IQueryable leaking outside Infrastructure
- Domain logic inside repositories

---

## 8. Transactions & Consistency

### Rules

- Bulk operations (ExpenseInput → N Expenses) are **atomic**
- Transactions are initiated by Application layer
- Executed via Infrastructure mechanisms
- Failure rolls back entire operation

---

## 9. AI Processing Persistence

When enabled, persist:

- ExpenseInput reference
- Normalized AI output
- Confidence score (0–100)
- Model metadata:
  - Model name
  - Version
  - Agent identifier
- Timestamp
- Processing outcome

AI audit records are immutable.

---

## 10. Mapping Strategy

- Domain entities remain pure
- Infrastructure uses:
  - Explicit EF Core configurations
  - Owned entities for Value Objects
  - No convention-only mappings

---

## 11. Folder Structure (Backend)

```text
backend/
└── src/
    └── Infrastructure/
        ├── Persistence/
        │   ├── AppDbContext.cs
        │   ├── Configurations/
        │   └── Migrations/
        ├── Repositories/
        ├── Transactions/
        ├── Interfaces/
        └── DependencyInjection/
```

### Notes

- `Interfaces/` contains **only interface implementations required by Infrastructure**
- Domain and Application interfaces remain defined in their respective layers
- `DependencyInjection/` is responsible **only** for wiring dependencies (service registration)

---

## 12. Build & Validation Responsibility

The Infrastructure Agent **must**:

1. Run a full backend build after changes
2. Fix compilation errors introduced by its work
3. Ensure migrations compile and apply
4. Leave the project in a buildable state

A change is **not complete** unless the build succeeds.

---

## 13. Agent Execution Instructions (CLI / AI Tools)

> You are acting as the **Infrastructure Agent**.
>
> Implement only persistence, repositories, DbContext, migrations, and wiring.
>
> Do NOT modify domain logic.
> Do NOT change business rules.
>
> After implementing changes:
> - Run a full build
> - Fix any compilation errors
>
> Output production-ready code only.

---

## Outcome

Once this agent completes its task, the backend will have:

- A stable and scalable persistence layer
- Clear transaction boundaries
- AI processing data safely stored
- A foundation ready for API and Mobile integration

