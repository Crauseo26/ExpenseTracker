# API and Contracts

## Purpose
This document defines the public API surface of the backend and the contracts between:

- Mobile App ↔ Backend API
- Backend API ↔ AI Orchestration Service

It also clarifies synchronous vs asynchronous flows and transactional guarantees.

---

## General Principles

- Backend is the **single source of truth**
- AI output is treated as **untrusted input**
- All domain rules are enforced server-side
- Mobile clients never decide domain state

---

## Authentication

- The MVP includes authentication and session handling.
- All endpoints are authenticated.
- Every request is scoped to a single `UserId`.

Authorization is simple and user-based (no roles or permissions in MVP).

---

## Expense Creation Flows

### 1. Manual Expense Creation (Sync)

**Endpoint**
```
POST /api/expenses
```

**Behavior**
- Validates domain rules
- Creates an Expense with status `CONFIRMED`
- Bypasses AI entirely

---

### 2. Text-based AI Processing (Sync)

**Endpoint**
```
POST /api/expense-inputs/text
```

**Behavior**
- Creates an `ExpenseInput`
- Sends normalized text to AI Orchestration Service
- Receives one or more Expense proposals
- Applies validation and scoring rules

**Decision Rules**
- `confidence >= threshold (0.87)` → Expense created as `CONFIRMED`
- `confidence < threshold` → Expense created as `PENDING_REVIEW`

---

### 3. Image-based AI Processing (Async)

**Endpoint**
```
POST /api/expense-inputs/image
```

**Behavior**
- Creates an `ExpenseInput`
- Enqueues an async processing job
- Returns `202 Accepted`

Result retrieval:
- Polling endpoint
- Or callback notification (future)

---

## Transactional Guarantees

### Bulk Expense Creation

When a single `ExpenseInput` produces multiple Expense proposals:

- Expense creation is **transactional**
- Either:
  - all valid Expenses are persisted, or
  - none are persisted

Partial persistence is **not allowed**.

This guarantees consistency between:
- ExpenseInput
- Generated Expenses
- Audit data

---

## Expense States

| Condition | Resulting State |
|--------|---------------|
| Manual creation | CONFIRMED |
| AI score ≥ threshold | CONFIRMED |
| AI score < threshold | PENDING_REVIEW |

State transitions:
- `PENDING_REVIEW → CONFIRMED` (allowed)
- `CONFIRMED → any` (not allowed)

---

## Editing Expenses

**Endpoint**
```
PUT /api/expenses/{expenseId}
```

Rules:
- Pending expenses may be edited freely
- Any edit on a pending expense confirms it
- Confirmed expenses:
  - Account cannot be changed
  - Month and year of purchase date are immutable
  - Day may be edited

---

## ExpenseInput Status

| Status | Meaning |
|------|--------|
| PROCESSED | AI processing completed |
| ERROR | AI processing failed |

On `ERROR`:
- No expenses are confirmed
- User is notified

---

## AI Orchestration Contract

### Request

```json
{
  "normalizedText": "string",
  "inputType": "TEXT | IMAGE | NOTIFICATION",
  "references": {
    "accounts": ["UUID"],
    "expenseGroups": ["UUID"]
  }
}
```

---

### Response

```json
{
  "proposals": [
    {
      "accountId": "UUID",
      "expenseGroupId": "UUID",
      "amount": 510.00,
      "currency": "UYU",
      "description": "Uber",
      "expenseType": "SPORADIC | REPETITIVE",
      "purchaseDate": "YYYY-MM-DD",
      "confidence": 0.91
    }
  ],
  "overallConfidence": 0.90
}
```

Rules:
- AI must not create new entities
- All IDs must reference existing data

---

## Error Handling

- Domain validation errors → `400 Bad Request`
- Authentication errors → `401 Unauthorized`
- AI processing failures → `202 Accepted` + `ExpenseInput` marked as `ERROR`

---

## Non-Goals

This API does **not** include:

- Budget endpoints
- Reporting endpoints
- Analytics
- Multi-user sharing
- Banking integrations

---

## Summary

The API design:

- Keeps domain authority centralized
- Supports AI-assisted automation safely
- Guarantees transactional consistency
- Is simple, explicit, and evolvable

