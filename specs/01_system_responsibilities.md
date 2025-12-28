# A — System Responsibilities & Boundaries

## Purpose
This document defines **clear responsibility boundaries** between the Mobile App, Backend System, and AI Services. It explicitly states **who can decide what**, and just as importantly, **what each component must not decide**.

These boundaries are mandatory and apply to the MVP and all future evolutions of the system.

---

## 1. Mobile Application Responsibilities

### The Mobile App **IS responsible for**:

- Capturing user input from multiple sources:
  - Manual form entry
  - Plain text (copy/paste from emails or notifications)
  - Image upload or camera capture
- Performing **basic client-side validations**:
  - Required fields
  - Correct numeric formats
  - Simple UX validations
- Displaying the **latest authoritative state** of Expenses as returned by the backend
- Allowing the user to:
  - Review AI-proposed Expenses
  - Correct AI-proposed data
  - Manually create Expenses without using AI
  - Confirm Expenses pending review

### The Mobile App **IS NOT responsible for**:

- Domain rule validation
- Determining the final state of an Expense
- Interpreting AI confidence scores
- Applying business logic or state transitions
- Persisting authoritative data
- Making decisions based on partial or cached information

> The Mobile App acts as a **thin client**. It reflects backend decisions but never replaces them.

---

## 2. Backend System Responsibilities (.NET)

### The Backend **IS responsible for**:

- Acting as the **single source of truth**
- Validating all domain rules
- Creating, updating, and soft-deleting domain entities
- Managing Expense state transitions:
  - Pending → Confirmed
- Orchestrating interactions with AI services
- Accepting, rejecting, or correcting AI outputs
- Deciding the final state of each Expense
- Persisting all data in the relational database
- Exposing:
  - Synchronous REST API endpoints
  - Asynchronous job endpoints
  - Polling or callback mechanisms for AI processing results

### The Backend **IS NOT responsible for**:

- Performing AI inference directly
- Delegating domain authority to AI services
- Making UI or presentation decisions

> The Backend is the **final decision-maker** and enforces all invariants of the system.

---

## 3. AI Services Responsibilities

### AI Services **ARE responsible for**:

- Processing normalized input provided by the backend
- Extracting structured information from:
  - Plain text
  - OCR results from images
- Proposing one or more Expense candidates per input
- Returning results in a **strictly defined JSON contract**
- Providing a **confidence score (0–100)** for each proposed Expense

### AI Services **ARE NOT responsible for**:

- Creating or mutating domain entities directly
- Persisting data
- Deciding Expense states
- Enforcing domain rules
- Creating new Accounts or Expense Groups
- Applying date rules or temporal corrections

> AI services are **advisory systems**, not authoritative ones.

---

## 4. Authority and Decision Rules

- All authoritative decisions belong to the Backend
- AI output is always treated as a **proposal**
- Confidence thresholds are evaluated exclusively by the Backend
- User corrections override AI proposals
- Once an Expense is confirmed:
  - It cannot be reverted to Pending
  - It cannot be reprocessed by AI

---

## 5. Error Handling Responsibilities

- If AI processing fails:
  - The ExpenseInput is marked as errored
  - The user is notified
- If AI output violates hard domain rules:
  - The backend rejects or corrects the output
  - The system remains consistent

---

## Summary

| Component | Authority Level |
|---------|----------------|
| Mobile App | Presentation & Input |
| Backend | Full Domain Authority |
| AI Services | Proposal & Classification Only |

This separation is **intentional and non-negotiable**, ensuring correctness, auditability, and long-term evolvability of the system.
