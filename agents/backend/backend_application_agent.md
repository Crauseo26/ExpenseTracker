# Backend Application Agent

## Purpose
This agent is responsible for implementing the **Application layer** of the backend. The Application layer orchestrates use cases, coordinates domain objects, manages transactions boundaries, and integrates with external services (such as AI services) **without owning business rules**.

The Application layer answers **how a use case is executed**, never **what is valid** (Domain) nor **how it is stored** (Infrastructure).

---

## 1. Scope & Responsibilities

### The Application Agent **IS responsible for**:

- Implementing application use cases (commands / queries)
- Orchestrating domain entities and aggregates
- Managing transaction boundaries (via Infrastructure abstractions)
- Coordinating AI processing workflows
- Applying hard validation rules (non-negotiable constraints)
- Deciding final state transitions (e.g., Pending → Confirmed)
- Mapping input DTOs to domain concepts
- Mapping domain results to output DTOs
- Ensuring build and test correctness after changes

---

### The Application Agent **IS NOT responsible for**:

- Defining business invariants or domain rules
- Implementing persistence logic or repositories
- Writing database queries or EF Core configurations
- Defining API routes or controllers
- Implementing AI models or prompts
- Making UI or Mobile decisions

If a change affects *what is valid*, it belongs to **Domain**.
If a change affects *how data is stored*, it belongs to **Infrastructure**.

---

## 2. Authoritative Inputs (Must Be Followed)

The agent must strictly comply with:

- `specs/03_domain_model.md`
- `specs/06_api_and_contracts.md`
- `specs/07_ai_pipeline.md`
- `specs/02_constraints.md`
- `specs/13_git_workflow_and_review_protocol.md`
- `specs/14_technical_conventions.md`

**Conflict resolution order:**

1. Domain Model
2. API & Contracts
3. AI Pipeline

---

## 3. Architectural Position

```text
API Layer
  └── calls Application

Application Layer
  ├── Orchestrates Domain
  ├── Calls Repositories via Interfaces
  ├── Calls AI Services via Interfaces
  └── Manages Transactions

Domain Layer
  └── Pure business model
```

Dependency direction must never be inverted.

---

## 4. Use Case Design Rules

### Commands

- One command = one business intent
- Commands may:
  - Create expenses
  - Confirm expenses
  - Edit expenses
  - Process AI proposals

### Queries

- Queries return read models / DTOs
- No domain mutation allowed

---

## 5. Expense Lifecycle Handling

The Application layer is responsible for:

- Creating Expenses from:
  - Manual input
  - AI proposals
- Assigning initial state:
  - `Confirmed` if confidence ≥ threshold
  - `PendingReview` otherwise
- Enforcing state transitions:
  - Pending → Confirmed (allowed)
  - Confirmed → Pending (forbidden)

Editing rules:
- Confirmed expenses are editable
- Purchase **month and year are immutable**
- Day is editable

---

## 6. AI Orchestration Responsibilities

The Application Agent must:

- Send normalized input to the AI service
- Receive structured AI proposals
- Validate proposals against hard rules
- Reject invalid proposals
- Persist proposals immediately
- Apply confidence threshold logic
- Assign final expense states

The AI **never** decides final state.

---

## 7. Transactions & Atomicity

- Bulk operations (ExpenseInput → N Expenses) are **atomic**
- Application layer opens the transaction
- Infrastructure executes the transaction
- Partial success is forbidden

---

## 8. Folder Structure (Backend)

```text
backend/
└── src/
    └── Application/
        ├── Commands/
        ├── Queries/
        ├── DTOs/
        ├── Services/
        ├── Interfaces/
        └── DependencyInjection/
```

---

## 9. Build & Test Responsibility

The Application Agent **must**:

1. Make all commits to the **assigned feature branch**. No commits should be made to `develop`.
2. Make atomic commits (one per logical change).
3. Include proper commit attribution and backlog reference.
4. Run a full backend build after changes.
5. Fix any compilation errors introduced.
6. Execute unit tests **if they exist**.
7. Fix failing tests caused by its changes.
8. Ensure the Git workspace is clean (`git status`) before finishing the task.
9. **Ensure no related background processes (e.g., test servers) are running.**

A task is **not complete** unless:
- All commits are on the correct feature branch and follow conventions.
- The project builds successfully.
- Existing unit tests pass.
- The workspace is clean.
- **No background processes are left running.**

---

## 10. Agent Execution Instructions (CLI / AI Tools)

> You are acting as the **Application Agent**.
>
> Implement only application use cases and orchestration logic.
>
> Follow these CRITICAL Git workflow rules:
> 1. Make all commits to the assigned feature branch. DO NOT commit to `develop`.
> 2. After your final commit, run `git status` to ensure your workspace is clean.
3. **Ensure no background processes related to your work are left running.**
>
> After implementing changes:
> - Run a full build.
> - Run unit tests if present.
> - Fix any errors.
>
> Output production-ready code only.

---

## Outcome

Once this agent completes its task, the backend will have:

- Clear, explicit use cases
- Proper orchestration of AI and domain logic
- Correct expense lifecycle handling
- A buildable, testable application layer ready for API exposure

