# Execution Workflows (End-to-End)

## Purpose
This document defines the **end-to-end execution workflows** of the system.

It connects:
- Mobile App behavior
- Backend orchestration
- AI pipeline
- Persistence

The goal is to allow **independent agents** (mobile, backend, AI) to work in parallel without ambiguity.

---

## Global Principles

- Backend is the **single authority**
- Mobile clients are **stateless and passive**
- AI systems **propose**, never decide
- All workflows are **user-scoped** (`UserId`)

---

## Workflow 1 — Manual Expense Creation

### Trigger
User creates an expense manually via mobile form.

### Steps
1. Mobile validates form fields (required, format only)
2. Mobile sends request to backend
3. Backend authenticates user
4. Backend validates domain rules
5. Backend creates Expense with state `CONFIRMED`
6. Backend persists Expense
7. Backend returns created Expense to mobile

### Outcome
- Expense is immediately usable
- No AI involvement

---

## Workflow 2 — Text-Based AI Expense Processing (Synchronous)

### Trigger
User pastes or forwards text (email, notification, copied text).

### Steps
1. Mobile sends raw text to backend
2. Backend authenticates user
3. Backend creates ExpenseInput (TEXT)
4. Backend normalizes input
5. Backend calls AI Orchestration Service (sync)
6. AI service:
   - selects appropriate agent
   - processes input
   - returns proposals + confidence scores
7. Backend validates each proposal
8. Backend applies scoring rules:
   - score ≥ threshold → CONFIRMED
   - score < threshold → PENDING_REVIEW
9. Backend persists all Expenses transactionally
10. Backend marks ExpenseInput as PROCESSED
11. Backend returns created Expenses to mobile

### Outcome
- One ExpenseInput may generate 0..N Expenses
- Expenses are immediately visible

---

## Workflow 3 — Image-Based AI Expense Processing (Asynchronous)

### Trigger
User uploads or captures an image (receipt, invoice).

### Steps
1. Mobile uploads image to backend
2. Backend authenticates user
3. Backend creates ExpenseInput (IMAGE)
4. Backend enqueues async job
5. Backend returns `202 Accepted`
6. Async worker:
   - performs OCR
   - normalizes text
   - calls AI Orchestration Service
7. Backend receives proposals
8. Backend validates proposals
9. Backend persists Expenses transactionally
10. Backend updates ExpenseInput status

### Result Retrieval
- Mobile polls for status
- Or receives notification (future)

---

## Workflow 4 — Editing Expenses

### Editing PENDING_REVIEW

1. User edits expense in mobile app
2. Mobile sends update request
3. Backend validates changes
4. Backend updates Expense
5. Backend promotes state to `CONFIRMED`

---

### Editing CONFIRMED

1. User edits expense
2. Backend validates:
   - month/year unchanged
3. Backend persists changes
4. State remains `CONFIRMED`

---

## Workflow 5 — ExpenseInput Error Handling

### Trigger
AI processing fails or returns invalid data.

### Steps
1. Backend marks ExpenseInput as `ERROR`
2. No Expenses are confirmed
3. Backend notifies user

---

## Workflow 6 — Deleting Expenses

### Trigger
User deletes an expense.

### Steps
1. Mobile sends delete request
2. Backend performs soft delete
3. Expense excluded from queries

---

## Workflow 7 — Listing & Querying Expenses

### Trigger
User opens expense list or dashboard.

### Steps
1. Mobile requests expenses
2. Backend applies:
   - UserId scope
   - filters (date, group, currency)
3. Backend returns results

---

## Failure & Recovery Principles

- Partial persistence is forbidden
- Async jobs are idempotent
- Expense lifecycle rules always apply

---

## Summary

These workflows:

- define the operational backbone of the system
- ensure consistency across components
- allow agents to implement features independently

This document should be used as a **coordination reference** during development.

