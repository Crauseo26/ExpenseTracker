# Expense Lifecycle & State Machine

## Purpose
This document defines the **explicit lifecycle**, allowed state transitions, and invariants for the `Expense` aggregate.

It exists to remove ambiguity for:
- backend implementation
- AI-assisted flows
- agent-driven development

The lifecycle rules defined here are **authoritative** and must be enforced by the backend.

---

## Expense States

An `Expense` can be in exactly one of the following states:

- **PENDING_REVIEW**
- **CONFIRMED**

No other states exist in the MVP.

---

## State Definitions

### PENDING_REVIEW

An Expense is in `PENDING_REVIEW` when:

- it was created via AI processing
- and the confidence score is **below** the global threshold (0.87)

Characteristics:
- Expense is persisted
- Expense is visible to the user
- Expense requires explicit user intervention

---

### CONFIRMED

An Expense is in `CONFIRMED` when:

- it was created manually by the user, or
- it was created by AI processing with confidence **greater than or equal** to the threshold, or
- it was edited manually while in `PENDING_REVIEW`

Characteristics:
- Expense is final
- Expense participates fully in queries and aggregations
- Expense cannot return to `PENDING_REVIEW`

---

## Allowed State Transitions

```
PENDING_REVIEW  ──▶  CONFIRMED
```

Rules:
- This transition is **irreversible**
- No other transitions are allowed

---

## Forbidden Transitions

The following transitions are **not allowed**:

- `CONFIRMED → PENDING_REVIEW`
- `CONFIRMED → any other state`

Attempting a forbidden transition must result in a domain error.

---

## Creation Rules

### Manual Creation

- State: `CONFIRMED`
- AI is not involved

---

### AI-based Creation

- State is determined per proposal:

| Condition | State |
|---------|------|
| confidence ≥ threshold | CONFIRMED |
| confidence < threshold | PENDING_REVIEW |

- Multiple Expenses may be created from a single ExpenseInput
- Creation is **transactional**

---

## Edit Rules

### Editing PENDING_REVIEW Expenses

Allowed:
- Any field may be edited

Effect:
- Expense is automatically promoted to `CONFIRMED`

---

### Editing CONFIRMED Expenses

Allowed:
- Account
- ExpenseGroup (indirectly via Account)
- Amount
- Currency
- Description
- ExpenseType
- Purchase day (day only)

Restrictions:
- Purchase **month and year are immutable**

Any edit on a CONFIRMED expense keeps it in the CONFIRMED state.

---

## Deletion Rules

- Expenses are **soft-deleted**
- Deletion does not affect related entities (Account, ExpenseGroup, ExpenseInput)

---

## Relationship to ExpenseInput

- An Expense may reference one ExpenseInput
- An ExpenseInput may generate:
  - zero Expenses
  - one Expense
  - many Expenses

Expense lifecycle is **independent** of ExpenseInput state after creation.

---

## Authority & Enforcement

- The backend is the **only authority** that may:
  - create Expenses
  - change Expense state
  - enforce lifecycle rules

- Mobile applications:
  - never decide state
  - only reflect backend state

- AI services:
  - propose data
  - never decide state

---

## Summary

The Expense lifecycle:

- is intentionally minimal
- enforces strong invariants
- treats AI as an assistant, not an authority
- ensures auditability and correctness

This document must be treated as a **hard constraint** by all agents and implementations.

