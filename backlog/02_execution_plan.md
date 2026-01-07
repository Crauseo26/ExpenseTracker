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
None yet - MVP development starting

### In Progress 🔄
None

### Ready for Review 📋
- F01-T01: Define User aggregate (Backend-Domain-Agent) - Branch: feature/F01-T01-user-aggregate

### Blocked ⛔
None

---

## Pending Tasks (Priority Order) 📝

### Feature 01 — User Authentication & Scoping

**Goal**: Ensure all domain entities are scoped to a User.

**Tasks:**
- [ ] F01-T01: Define User aggregate (Domain Agent)
- [ ] F01-T02: Implement authentication mechanism (API Agent)
- [ ] F01-T03: Enforce user scoping in repositories (Infrastructure Agent)

**Agent Assignments:**
- F01-T01: Backend-Domain-Agent
- F01-T02: Backend-API-Agent
- F01-T03: Backend-Infrastructure-Agent

**Dependencies:**
- F01-T02 depends on F01-T01
- F01-T03 depends on F01-T01

**Parallelization:**
- None in F01 (sequential execution required)

---

### Feature 02 — Expense Core Lifecycle

**Goal**: Support manual creation and editing of Expenses.

**Tasks:**
- [ ] F02-T01: Implement Expense aggregate lifecycle (Domain Agent)
- [ ] F02-T02: Implement expense use cases (Application Agent)

**Agent Assignments:**
- F02-T01: Backend-Domain-Agent
- F02-T02: Backend-Application-Agent

**Dependencies:**
- F02 depends on F01-T01 (User aggregate must exist)
- F02-T02 depends on F02-T01

**Parallelization:**
- F02-T01 can start after F01-T01 is merged (doesn't need F01-T02 or F01-T03)

---

### Feature 03 — Account & Account Group Management

**Goal**: Enable expense categorization via Accounts.

**Tasks:**
- [ ] F03-T01: Implement Account and AccountGroup aggregates (Domain Agent)
- [ ] F03-T02: Implement account management use cases (Application Agent)

**Agent Assignments:**
- F03-T01: Backend-Domain-Agent
- F03-T02: Backend-Application-Agent

**Dependencies:**
- F03 depends on F01-T01 (User aggregate)
- F03-T02 depends on F03-T01

**Parallelization:**
- F03-T01 can run in parallel with F02-T02 (different aggregates)

---

### Feature 04 — ExpenseInput & AI Processing Pipeline

**Goal**: Convert unstructured text into proposed Expenses.

**Tasks:**
- [ ] F04-T01: Implement ExpenseInput aggregate (Domain Agent)
- [ ] F04-T02: Implement AI integration interfaces (Application Agent)
- [ ] F04-T03: Implement AI service client (Application Agent)

**Agent Assignments:**
- F04-T01: Backend-Domain-Agent
- F04-T02: Backend-Application-Agent
- F04-T03: Backend-Application-Agent

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

