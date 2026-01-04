# 01 — MVP Backlog (Agent-Oriented)

## Purpose
This document defines the **initial MVP backlog**, explicitly designed to be:
- Executable by AI agents
- Composed of **atomic, reviewable units of work**
- Independent from UI/UX narratives

This backlog is the **input source** for the Lead Agent.

---

## Scope Boundaries

**Included**:
- Backend (Domain, Application, Infrastructure, API)
- Authentication & user scoping
- AI pipeline integration
- Minimal mobile support contracts

**Explicitly excluded**:
- Budgeting
- Analytics
- Coaching / recommendations
- Notifications
- Multi-user sharing

---

## Feature 01 — User Authentication & Scoping

**Goal**: Ensure all domain entities are scoped to a User.

### Tasks
- Define User aggregate (Domain)
- Implement authentication mechanism (API)
- Ensure `UserId` is mandatory in all aggregates
- Enforce user scoping in repositories

### Assigned Agents
- Domain Agent
- Application Agent
- Infrastructure Agent
- API Agent

### Acceptance Criteria
- No entity can exist without `UserId`
- Authenticated user context is available end-to-end

---

## Feature 02 — Expense Core Lifecycle

**Goal**: Support manual creation and editing of Expenses.

### Tasks
- Implement Expense aggregate lifecycle
- Support create, edit, soft delete
- Enforce date immutability (month/year) on confirmed expenses

### Assigned Agents
- Domain Agent
- Application Agent

### Acceptance Criteria
- Confirmed expenses editable except month/year
- Soft delete applied consistently

---

## Feature 03 — Account & Account Group Management

**Goal**: Enable expense categorization via Accounts.

### Tasks
- Implement Account and AccountGroup aggregates
- Support reassignment of Accounts between groups
- Prevent Account change on confirmed Expense

### Assigned Agents
- Domain Agent
- Application Agent

### Acceptance Criteria
- AccountGroup changes do not affect existing Expenses

---

## Feature 04 — ExpenseInput & AI Processing Pipeline

**Goal**: Convert unstructured text into proposed Expenses.

### Tasks
- Implement ExpenseInput aggregate
- Integrate AI pipeline service
- Support 0..N proposed Expenses per input
- Persist AI confidence scoring and metadata

### Assigned Agents
- Application Agent
- Infrastructure Agent

### Acceptance Criteria
- Inputs with errors are marked failed
- Confidence scoring persisted (0–100)

---

## Feature 05 — Expense Confirmation Rules

**Goal**: Automate or defer confirmation based on confidence.

### Tasks
- Implement scoring threshold logic
- Auto-confirm expenses above threshold
- Mark low-confidence expenses as pending review

### Assigned Agents
- Domain Agent
- Application Agent

### Acceptance Criteria
- Threshold configurable
- No reprocessing of confirmed expenses

---

## Feature 06 — Persistence & Migrations

**Goal**: Durable, auditable data storage.

### Tasks
- EF Core DbContext
- Migrations for all aggregates
- Soft delete filters

### Assigned Agents
- Infrastructure Agent

### Acceptance Criteria
- Clean migration history
- Build succeeds after migration

---

## Feature 07 — API Endpoints (MVP)

**Goal**: Expose backend capabilities.

### Tasks
- Expense CRUD endpoints
- ExpenseInput submission endpoint
- Query endpoints with filters

### Assigned Agents
- API Agent

### Acceptance Criteria
- API aligns with `06_api_and_contracts.md`

---

## Feature 08 — Build, Tests & Validation

**Goal**: Ensure production readiness.

### Tasks
- Add unit tests where applicable
- Enforce build execution per agent
- CI-ready project structure

### Assigned Agents
- Application Agent
- Infrastructure Agent

### Acceptance Criteria
- Full solution builds successfully
- Tests pass or are explicitly skipped

---

## Execution Model

- Features may be executed **sequentially or in parallel**
- Lead Agent is responsible for:
  - Decomposing features into tasks
  - Delegating to the correct agent
  - Validating outputs before merge

---

## Notes

This backlog is **not static**.
New features must:
- Be added as new Feature blocks
- Avoid modifying completed features
- Remain atomic and agent-friendly

