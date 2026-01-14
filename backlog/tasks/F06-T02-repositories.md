# Task: F06-T02 — Implement repositories

## Feature Reference
Feature 06 — Persistence & Migrations

## Objective
Implement the concrete infrastructure repositories for all domain aggregates, ensuring they correctly use the AppDbContext and follow the established patterns (user scoping, soft deletes where applicable).

## Agent Assignment
Backend-Infrastructure-Agent

## Branch
feature/F06-T02-repositories

## Authoritative Inputs
- specs/03_domain_model.md
- specs/05_persistence_model.md
- specs/11_engineering_guardrails.md
- specs/14_technical_conventions.md

## Expected Outputs
- backend/src/Expenses.Infrastructure/Repositories/ExpenseRepository.cs
- backend/src/Expenses.Infrastructure/Repositories/AccountRepository.cs
- backend/src/Expenses.Infrastructure/Repositories/ExpenseGroupRepository.cs
- backend/src/Expenses.Infrastructure/Repositories/ExpenseInputRepository.cs
- backend/src/Expenses.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs: Updated to register repositories.

## Acceptance Criteria
- [ ] Implement `ExpenseRepository` for `IExpenseRepository`
- [ ] Implement `AccountRepository` for `IAccountRepository`
- [ ] Implement `ExpenseGroupRepository` for `IExpenseGroupRepository`
- [ ] Implement `ExpenseInputRepository` for `IExpenseInputRepository`
- [ ] All repositories must enforce User Scoping (where applicable).
- [ ] Use `AppDbContext` via constructor injection.
- [ ] Project builds successfully.
- [ ] Commits follow Git workflow conventions.

## Dependencies
- Depends on: F06-T01 (DbContext and Configurations)
- Requires: All Domain Interfaces

## Technical Constraints
- Must use EF Core.
- Follow the pattern established in `UserRepository.cs`.
- Ensure async/await is used throughout.

## Status
⏳ Pending

## Execution Log

### 2026-01-14 - 20:20
**Action**: Task created
**Agent**: Project-Orchestrator
**Outcome**: Task file generated, ready for execution

## Notes
- User scoping is critical. Ensure queries include `.Where(x => x.UserId == userId)`.
- Some entities might not have a UserId directly (check specs).
