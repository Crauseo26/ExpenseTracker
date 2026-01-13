# Execution Plan

## Purpose
This document tracks the **current execution state** of the MVP development. It is the single source of truth for:
- What has been completed
- What is currently in progress
- What is ready for human review
- What is blocked
- What is pending in priority order

This document is **managed by the Lead Agent** and updated after each merge.

---

## Current Phase
**Phase 1 — Backend Core**

Target: Implement domain, application, infrastructure, and API layers for MVP features.

---

## Task Status Legend

- ✅ **Completed**: Merged to `develop`
- 🔄 **In Progress**: Agent currently working
- 📋 **Ready for Review**: Branch pushed, awaiting human review
- ⛔ **Blocked**: Cannot proceed due to dependency
- 📝 **Pending**: Not yet started, in priority order

---

## Status Dashboard

### Completed ✅
- [x] F01-T01: Define User aggregate (Domain Agent)
- [x] F01-T02: Migrate to ASP.NET Core Identity
- [x] F01-T02.1: Complete Swagger UI Integration for Authentication
- [x] F01-T03: Enforce user scoping in repositories (Infrastructure Agent)
- [x] F02-T01: Implement Expense aggregate lifecycle (Domain Agent)
- [x] F02-T02: Implement expense use cases (Application Agent)
- [x] F03-T01: Implement Account and AccountGroup aggregates (Domain Agent)
- [x] F03-T02: Implement account management use cases (Application Agent)
- [x] F04-T01: Implement ExpenseInput aggregate (Domain Agent)
- [x] F04-T02: Implement AI integration interfaces (Application Agent)
- [x] F04-T03: Implement AI service client (Infrastructure Agent)

### In Progress 🔄
None

### Ready for Review 📋
- [ ] F04-T02: Implement AI integration interfaces (Application Agent) - Branch: feature/F04-T02-ai-integration-interfaces
- [ ] F04-T03: Implement AI service client (Infrastructure Agent) - Branch: feature/F04-T03-ai-client

### Blocked ⛔
None

---

## Pending Tasks (Priority Order) 📝

### Feature 04 — ExpenseInput & AI Processing Pipeline

**Goal**: Convert unstructured text into proposed Expenses.

**Tasks:**
- [x] F04-T01: Implement ExpenseInput aggregate (Domain Agent)
- [x] F04-T02: Implement AI integration interfaces (Application Agent)
- [x] F04-T03: Implement AI service client (Infrastructure Agent)

**Agent Assignments:**
- F04-T01: Backend-Domain-Agent
- F04-T02: Backend-Application-Agent
- F04-T03: Backend-Infrastructure-Agent

**Dependencies:**
- F04 depends on F02 (Expense aggregate) and F03 (Account aggregate)
- F04-T02 depends on F04-T01
- F04-T03 depends on F04-T02

**Parallelization:**
- Limited (sequential within F04)

---

### Feature 05 — Expense Confirmation Rules

**Goal**: Automate or defer confirmation based on confidence.

**Tasks:**
- [ ] F05-T01: Implement confidence scoring logic (Domain Agent)
- [ ] F05-T02: Implement confirmation workflow (Application Agent)

**Agent Assignments:**
- F05-T01: Backend-Domain-Agent
- F05-T02: Backend-Application-Agent

**Dependencies:**
- F05 depends on F04 (AI pipeline must exist)
- F05-T02 depends on F05-T01

**Parallelization:**
- None (sequential)

---

### Feature 06 — Persistence & Migrations

**Goal**: Durable, auditable data storage.

**Tasks:**
- [ ] F06-T01: Create DbContext and entity configurations (Infrastructure Agent)
- [ ] F06-T02: Implement repositories (Infrastructure Agent)
- [ ] F06-T03: Create initial migration (Infrastructure Agent)

**Agent Assignments:**
- F06-T01: Backend-Infrastructure-Agent
- F06-T02: Backend-Infrastructure-Agent
- F06-T03: Backend-Infrastructure-Agent

**Dependencies:**
- F06 depends on all Domain work (F01-T01, F02-T01, F03-T01, F04-T01, F05-T01)
- F06-T02 depends on F06-T01
- F06-T03 depends on F06-T02

**Parallelization:**
- F06 can start once all domain aggregates are defined
- Tasks within F06 are sequential

---

### Feature 07 — API Endpoints (MVP)

**Goal**: Expose backend capabilities.

**Tasks:**
- [ ] F07-T01: Implement Expense CRUD endpoints (API Agent)
- [ ] F07-T02: Implement ExpenseInput submission endpoint (API Agent)
- [ ] F07-T03: Implement query endpoints with filters (API Agent)

**Agent Assignments:**
- F07-T01: Backend-API-Agent
- F07-T02: Backend-API-Agent
- F07-T03: Backend-API-Agent

**Dependencies:**
- F07 depends on F06 (persistence must exist) and all Application use cases
- F07-T02 depends on F07-T01
- F07-T03 depends on F07-T01

**Parallelization:**
- F07-T02 and F07-T03 can run in parallel after F07-T01

---

### Feature 08 — Build, Tests & Validation

**Goal**: Ensure production readiness.

**Tasks:**
- [ ] F08-T01: Add unit tests for domain (Application Agent)
- [ ] F08-T02: Add integration tests for API (Application Agent)
- [ ] F08-T03: Configure CI pipeline (Manual / DevOps)

**Agent Assignments:**
- F08-T01: Backend-Application-Agent
- F08-T02: Backend-Application-Agent
- F08-T03: Human

**Dependencies:**
- F08-T01 can start after F02, F03, F04, F05 are complete
- F08-T02 depends on F07
- F08-T03 is manual

**Parallelization:**
- F08-T01 and F08-T02 can run in parallel if scoped properly

---

## Execution Strategy

### Phase 1 Batch (Sequential Foundation)
1. F01-T01 → F01-T02 → F01-T03
   - Must complete before other features

### Phase 2 Batch (Parallel Domain Work)
2. F02-T01 (can start after F01-T01)
3. F03-T01 (can run parallel with F02-T02)

### Phase 3 Batch (Application Layer)
4. F02-T02 → F04-T01 → F04-T02 → F04-T03 → F05-T01 → F05-T02

### Phase 4 Batch (Persistence)
5. F06-T01 → F06-T02 → F06-T03

### Phase 5 Batch (API)
6. F07-T01 → (F07-T02 || F07-T03)

### Phase 6 Batch (Testing)
7. F08-T01 || F08-T02 → F08-T03 (manual)

---

## Notes & Decisions

### 2026-01-13 (Later)
- F04-T03 completed: AI service client implementation
- Implemented AIOrchestrationService in Infrastructure layer
- Created HTTP contract DTOs (AIProcessingRequest, AIProcessingResponse)
- Uses IHttpClientFactory with typed client pattern for proper lifecycle management
- Configured base URL (http://localhost:5000) and timeout (30s) in appsettings.json
- Comprehensive error handling: connection failures, timeouts, deserialization errors
- Returns empty proposal list on errors (no exceptions thrown to caller)
- Extensive logging for debugging and observability
- Added project reference from Infrastructure to Application layer
- Registered service in DI container
- Build verified successfully (0 warnings, 0 errors)
- Branch pushed: feature/F04-T03-ai-client
- **Feature F04 (ExpenseInput & AI Processing Pipeline) is now fully complete**

### 2026-01-13
- F04-T02 completed: AI integration interfaces implementation
- Implemented ExpenseProposalDto and AIProposalResponseDto for AI output format
- Implemented ExpenseInputDto for data transfer with FromDomain mapping
- Implemented IAIOrchestrationService interface for external AI service integration
- Implemented ProcessExpenseInputCommand and handler for AI processing workflow orchestration
- Handler creates ExpenseInput, calls AI service, creates Expense aggregates from proposals
- Applies confidence threshold (0.87) for auto-confirmation (CONFIRMED vs PENDING_REVIEW)
- Handles AI failures with error tracking and marks ExpenseInput as ERROR
- Skips invalid proposals gracefully (continues processing valid ones)
- Implemented GetExpenseInputByIdQuery and handler with user scoping
- Implemented GetExpenseInputsByUserQuery and handler with PendingOnly filter
- Updated DI configuration for Application layer
- Build verified successfully
- Branch pushed: feature/F04-T02-ai-integration-interfaces

### 2026-01-11
- F04-T01 completed and merged.
- ExpenseInput aggregate is now implemented.
- Ready to begin work on F04-T02.

### 2026-01-11
- F03-T02 completed: Account management use cases implementation
- Implemented AccountDto and AccountGroupDto for data transfer
- Implemented command handlers: CreateAccountGroup, UpdateAccountGroup, DeleteAccountGroup
- Implemented command handlers: CreateAccount, UpdateAccount, DeleteAccount
- Implemented query handlers: GetAccountGroupById, GetAccountGroupsByUser
- Implemented query handlers: GetAccountById, GetAccountsByUser with optional ExpenseGroup filtering
- All use cases follow CQRS pattern and enforce user scoping
- Updated DI configuration for Application layer
- Build verified successfully
- Branch pushed: feature/F03-T02-account-use-cases

- F03-T01 completed and merged.
- Account and AccountGroup aggregates are now implemented.
- **Feature F03 (Account & Account Group Management) is now fully complete.**

### 2026-01-10 (Later)
- F02-T02 completed: Expense use cases implementation
- Created Expenses.Application layer project
- Implemented IExpenseRepository interface in Domain layer
- Implemented ExpenseDto for data transfer
- Implemented command handlers: CreateExpense, UpdateExpense, ConfirmExpense, DeleteExpense
- Implemented query handlers: GetExpenseById, GetExpensesByUser with date filtering
- All use cases follow expense lifecycle rules from specs/08_expense_lifecycle.md
- User scoping enforced in all operations
- DI configuration for Application layer
- Build verified successfully
- Branch pushed: feature/F02-T02-expense-use-cases

### 2026-01-10
- F01-T03 completed and merged.
- **Feature F01 (User Authentication & Scoping) is now fully complete.**
- Ready to begin work on Feature F02.

- F02-T01 completed: Expense aggregate lifecycle implementation
- Implemented Expense aggregate root with full lifecycle rules per specs/08_expense_lifecycle.md
- Added Currency enum (UYU, USD) and Money value object with validation
- Added ExpenseStatus (PendingReview, Confirmed) and ExpenseType (Sporadic, Repetitive) enums
- Implemented factory methods for manual and AI-based expense creation
- State transitions: PendingReview → Confirmed (irreversible)
- Month/year immutable for Confirmed expenses
- Soft delete support
- Build verified successfully
- Branch pushed: feature/F02-T01-expense-aggregate

### 2026-01-08
- F01-T02 completed: ASP.NET Core Identity migration
- User aggregate now extends IdentityUser<Guid>
- Infrastructure layer configured with AppDbContext (IdentityDbContext)
- JWT authentication configured in API layer
- Authentication endpoints (Register/Login) implemented
- Build verified successfully
- Branch pushed: feature/F01-T02-aspnet-core-identity

- F01-T02.1 completed: Swagger UI Integration for Authentication
- Replaced Microsoft.AspNetCore.OpenApi with Swashbuckle.AspNetCore (v6.9.0)
- Configured Swagger with JWT Bearer security scheme
- Added authorization button to Swagger UI for testing authenticated endpoints
- Swagger endpoint configured at /swagger
- Build verified successfully
- Branch pushed: feature/F01-T02.1-swagger-authentication

### 2025-01-06
- Initial execution plan created
- MVP scope confirmed per backlog/01_mvp_backlog.md
- Parallelization opportunities identified
- Lead Agent ready to begin execution

---

## Update Protocol

**After each merge, Lead Agent must:**
1. Move completed task to ✅ Completed section
2. Update task status (check the box)
3. Identify next task(s) to execute
4. Check for parallelization opportunities
5. Update "In Progress" section
6. Commit changes to this file with message:
   ```
   docs(backlog): Update execution plan after F0X-T0Y completion
   
   Agent: Lead-Agent
   ```

---

## Human Checkpoint Protocol

**Lead Agent must request human confirmation:**
- Before starting a new phase
- After completing a feature (all tasks)
- When encountering blockers
- Before making scope changes

**Human is expected to:**
- Review completed work
- Merge approved branches
- Provide feedback or corrections
- Authorize continuation

