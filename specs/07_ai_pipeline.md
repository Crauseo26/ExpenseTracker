# AI Pipeline Architecture

## Purpose
This document defines the architecture, responsibilities, and constraints of the AI Processing Pipeline used to transform unstructured user inputs into structured **Expense proposals**.

The AI pipeline is a **proposal system**, not a decision-making authority. All final decisions are made by the backend domain logic.

---

## Design Principles

1. **AI proposes, backend decides**
2. **No domain authority inside AI**
3. **Deterministic domain rules override AI output**
4. **Replaceable AI providers**
5. **Observability and auditability by design**
6. **Fail-safe over silent corruption**

---

## High-Level Flow

```
Raw Input (Text / Image / Notification)
        ↓
Input Normalization
        ↓
AI Orchestration Service
        ↓
Specialized AI Agent (OCR / Text / Classification)
        ↓
Structured JSON Proposal + Confidence Score
        ↓
Backend Validation & Decision Engine
```

---

## Input Types

The pipeline supports the following input types:

- Plain text (manual copy/paste)
- Forwarded text (email / notification content)
- Images (receipts, screenshots)

All inputs are converted into **normalized plain text** before semantic processing.

---

## Input Normalization Stage

Responsibilities:

- Clean formatting
- Remove noise (headers, footers, signatures)
- Normalize numbers, dates, and currency symbols
- Detect potential multi-expense inputs

Output:

```json
{
  "normalizedText": "string",
  "inputType": "TEXT | IMAGE | NOTIFICATION",
  "metadata": {
    "source": "string",
    "receivedAt": "ISO-8601"
  }
}
```

---

## AI Orchestration Service

The AI Orchestration Service is an **external service** (Python-based) owned by the project.

Responsibilities:

- Route inputs to the appropriate AI agent
- Enforce output schema
- Calculate confidence scoring
- Prevent hallucinated entity creation
- Normalize output to a strict JSON contract

This service is **stateless** and **replaceable**.

---

## Specialized AI Agents

### Text Parsing Agent

Used for:
- Plain text
- Notifications
- Emails

Capabilities:
- Expense extraction
- Amount detection
- Currency detection
- Account matching (by reference)

---

### OCR + Vision Agent

Used for:
- Receipt images
- Screenshots

Capabilities:
- OCR extraction
- Merchant detection
- Line-item grouping

Constraints:
- Executed asynchronously
- Cost-sensitive

---

## Output Contract (AI Proposal)

The AI must return a **list of proposed expenses**.

```json
{
  "proposals": [
    {
      "accountId": "UUID",
      "expenseGroupId": "UUID",
      "amount": 1090.00,
      "currency": "UYU",
      "description": "McDonalds",
      "expenseType": "SPORADIC | REPETITIVE",
      "purchaseDate": "YYYY-MM-DD",
      "confidence": 0.92
    }
  ],
  "overallConfidence": 0.90
}
```

Rules:
- AI cannot create new Accounts or ExpenseGroups
- All references must already exist
- Amounts must be non-negative

---

## Confidence Scoring

- Range: **0–100 internally**, normalized to **0.0–1.0** for decision logic
- Global MVP threshold: **0.87**

Decision rules:

- `confidence >= threshold → Expense marked **CONFIRMED**
- `confidence < threshold` → Expense marked **PENDING_REVIEW****

---

## Backend Validation & Decision Layer

The backend is responsible for:

- Validating domain rules
- Applying date correction rules
- Assigning final expense state
- Rejecting invalid proposals
- Persisting accepted proposals

AI output is treated as **untrusted input**.

---

## Multi-Expense Inputs

A single `ExpenseInput` may generate:

- 0 Expenses (invalid / error)
- 1 Expense
- N Expenses

Example:

```
Uber UYU 510 Yesterday
McDonalds UYU 1090 Yesterday
Coffee UYU 310 Today
```

---

## Failure Handling

### AI Failure

- Input marked as `ERROR`
- User notified
- No expenses confirmed

### Partial Success

- Valid proposals persisted
- Invalid ones rejected with reason

---

## Audit & Traceability

For each AI processing:

- Raw normalized input is stored
- AI output JSON is stored
- Confidence scores are stored
- Timestamp is stored
- **AI agent metadata is stored (provider, model name, model version)**

This enables:
- Debugging
- Model comparison
- Threshold tuning
- AI output JSON is stored
- Confidence scores are stored
- Timestamp and agent version are stored

This enables:
- Debugging
- Model comparison
- Threshold tuning

---

## Explicit Non-Responsibilities of AI

The AI pipeline **must not**:

- Decide expense final state
- Apply business rules
- Modify confirmed expenses
- Perform currency conversion
- Persist data directly

---

## Evolution Considerations

Future extensions may include:

- Provider-specific confidence calibration
- Per-input-type thresholds
- Human-in-the-loop correction feedback
- Incremental retraining datasets

---

## Summary

The AI Pipeline is a **controlled proposal system** designed to:

- Maximize automation
- Minimize domain risk
- Preserve backend authority
- Enable future scalability and replacement

