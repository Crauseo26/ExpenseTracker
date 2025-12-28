# Domain Model

## Overview
This document defines the core domain concepts, entities, value objects, and relationships of the system.

The domain is intentionally **expense-centric** and optimized for correctness, auditability, and future evolution.

---

## User Ownership

All domain entities are owned by **exactly one User**.

- The MVP includes **authentication and session handling**.
- Every persisted entity includes a `UserId`.
- There is **no data shared across users**.
- The MVP does **not** include:
  - shared accounts
  - multiple users under a single profile
  - cross-user visibility

This constraint simplifies the domain while keeping it ready for future multi-user extensions.

---

## Aggregate Root

### Expense (Aggregate Root)

An **Expense** represents a single monetary outflow from the user's personal balance.

Key characteristics:

- An Expense is always a **Gasto** (never an income).
- An Expense is immutable once confirmed (with limited editable fields).
- An Expense always belongs to one Account.
- An Expense always belongs (indirectly) to one ExpenseGroup.

#### Core Attributes

- `ExpenseId` (UUID)
- `UserId` (UUID)
- `AccountId` (UUID)
- `Amount` (decimal, >= 0, max 2 decimals)
- `Currency` (UYU | USD)
- `Description` (free text)
- `ExpenseType` (SPORADIC | REPETITIVE)
- `PurchaseDate` (YYYY-MM-DD)
- `Status` (PENDING_REVIEW | CONFIRMED)
- `CreatedAt`
- `UpdatedAt`
- `DeletedAt` (soft delete)

---

## Expense Lifecycle Rules

- An Expense is created either:
  - manually by the user, or
  - automatically via AI processing

- State transitions:
  - `PENDING_REVIEW → CONFIRMED` (allowed)
  - `CONFIRMED → any other state` (not allowed)

- Any manual edit on a pending expense automatically confirms it.
- Confirmed expenses cannot be reprocessed by AI.

---

## Account (Entity)

An **Account** represents a logical grouping of similar expenses.

Examples:
- Supermarket
- Rent
- Internet
- Gifts

Characteristics:

- Accounts are created **manually by the user**.
- The system may provide predefined accounts for initial setup.
- An Account can be reassigned to a different ExpenseGroup.
- Changing an Account’s ExpenseGroup does **not** modify existing Expenses.

Attributes:

- `AccountId`
- `UserId`
- `Name`
- `ExpenseGroupId`
- `CreatedAt`
- `DeletedAt`

---

## ExpenseGroup (Entity)

An **ExpenseGroup** is used to aggregate Accounts into higher-level categories.

Examples:
- Home Services
- Food
- Transportation

Characteristics:

- ExpenseGroups are independent entities.
- They do not directly reference Expenses.
- Aggregation occurs through Accounts.

Attributes:

- `ExpenseGroupId`
- `UserId`
- `Name`
- `CreatedAt`
- `DeletedAt`

---

## ExpenseInput (Entity)

An **ExpenseInput** represents a single source of unstructured information.

Examples:
- pasted text
- forwarded email content
- mobile notification
- image OCR result

Characteristics:

- One ExpenseInput may generate:
  - 0 Expenses
  - 1 Expense
  - N Expenses

- ExpenseInputs are immutable.
- ExpenseInputs are used for audit and traceability.

Attributes:

- `ExpenseInputId`
- `UserId`
- `InputType` (TEXT | IMAGE | NOTIFICATION)
- `RawContent`
- `NormalizedContent`
- `Status` (PROCESSED | ERROR)
- `CreatedAt`

---

## Relationships Summary

```
User
 ├─ Expense
 ├─ Account
 │    └─ ExpenseGroup
 └─ ExpenseInput
      └─ Expense (0..N)
```

---

## Value Objects

### Money (Value Object)

Represents a monetary amount.

- `Amount`
- `Currency`

Rules:
- Amount must be >= 0
- Max 2 decimals

This is modeled as a Value Object to allow future extensions (e.g. currency conversion).

---

### RepetitionPattern (Value Object)

Used only when `ExpenseType = REPETITIVE`.

Represents periodicity:

- Monthly
- Bi-monthly
- Quarterly
- Custom interval

This logic does not apply to sporadic expenses.

---

## Explicit Non-Goals of the Domain

The domain does **not** include:

- Budgets
- Incomes
- Savings
- Investments
- Reports
- Multi-user profiles
- Shared accounts

---

## Summary

The domain model:

- Is user-owned and secure by default
- Protects invariants strictly
- Keeps AI as an external proposal mechanism
- Is expressive without being over-engineered
- Is ready for future evolution

