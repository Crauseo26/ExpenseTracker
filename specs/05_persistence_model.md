# E — Persistence Model

## Purpose
This document defines the **relational persistence model** for the MVP. It translates the domain model into tables, relationships, and constraints while preserving domain invariants.

The database is the **single source of truth**. All writes are validated by the backend domain layer.

---

## 1. General Principles

- Relational database
- Explicit foreign keys
- Soft delete for domain entities
- Auditability for AI processing
- No direct persistence from AI services

---

## 2. Core Tables

### 2.1 Expenses

Represents a single financial outflow.

**Table:** `expenses`

| Column | Type | Notes |
|------|------|------|
| id | UUID | Primary key |
| account_id | UUID | FK → accounts.id |
| expense_input_id | UUID (nullable) | FK → expense_inputs.id |
| amount | DECIMAL(12,2) | Must be > 0 |
| currency | CHAR(3) | UYU, USD |
| expense_type | VARCHAR | Sporadic / Repetitive |
| description | TEXT | Free text |
| purchase_date | DATE | Day editable, month/year immutable after confirmation |
| state | VARCHAR | Pending / Confirmed |
| created_at | TIMESTAMP | System generated |
| updated_at | TIMESTAMP | System generated |
| deleted_at | TIMESTAMP (nullable) | Soft delete |

**Constraints:**
- `amount > 0`
- `state IN ('Pending','Confirmed')`
- `currency IN ('UYU','USD')`

---

### 2.2 Accounts

Logical grouping for expenses.

**Table:** `accounts`

| Column | Type | Notes |
|------|------|------|
| id | UUID | Primary key |
| expense_group_id | UUID | FK → expense_groups.id |
| name | VARCHAR | User-defined |
| created_at | TIMESTAMP |  |
| updated_at | TIMESTAMP |  |
| deleted_at | TIMESTAMP (nullable) | Soft delete |

**Rules:**
- Accounts are user-created only
- Account cannot be changed for confirmed expenses

---

### 2.3 Expense Groups

High-level categorization.

**Table:** `expense_groups`

| Column | Type | Notes |
|------|------|------|
| id | UUID | Primary key |
| name | VARCHAR | Unique per user |
| created_at | TIMESTAMP |  |
| updated_at | TIMESTAMP |  |
| deleted_at | TIMESTAMP (nullable) | Soft delete |

---

## 3. AI & Input Audit Tables

### 3.1 Expense Inputs

Raw inputs sent for AI processing.

**Table:** `expense_inputs`

| Column | Type | Notes |
|------|------|------|
| id | UUID | Primary key |
| raw_text | TEXT | Normalized input |
| source_type | VARCHAR | Text / Email / Notification / Image |
| status | VARCHAR | Processed / Error |
| created_at | TIMESTAMP |  |

---

### 3.2 AI Processing Results (Optional)

Stores historical AI processing metadata.

**Table:** `ai_processing_logs`

| Column | Type | Notes |
|------|------|------|
| id | UUID | Primary key |
| expense_input_id | UUID | FK → expense_inputs.id |
| model_name | VARCHAR | For traceability |
| confidence_score | INTEGER | 0–100 |
| raw_output | JSONB | AI response snapshot |
| created_at | TIMESTAMP |  |

---

## 4. Relationships

- One `expense_inputs` → 0..N `expenses`
- One `accounts` → N `expenses`
- One `expense_groups` → N `accounts`
- `expenses.expense_input_id` is nullable

---

## 5. Indexing Strategy

Recommended indexes:

- `expenses (purchase_date)`
- `expenses (state)`
- `expenses (account_id)`
- `accounts (expense_group_id)`
- `expense_inputs (created_at)`

---

## 6. Data Mutability Rules

| Entity | Editable | Notes |
|------|--------|------|
| Expense | Partial | Day editable after confirmation |
| Account | Yes | Except for confirmed expenses |
| Expense Group | Yes | No cascade to expenses |
| Expense Input | No | Immutable |

---

## 7. Deletion Rules

- Soft delete only for domain entities
- Deleted records are excluded from queries by default
- Audit tables are never deleted

---

## Summary

This persistence model:
- Preserves domain invariants
- Supports AI auditability
- Enables efficient querying
- Avoids premature optimization

All schema changes must be validated against the Domain Model (C).

