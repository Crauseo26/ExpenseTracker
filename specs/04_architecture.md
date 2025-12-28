# D — System Architecture (High-Level Design)

## Purpose
This document describes the **high-level system architecture**, component interactions, and execution flows for the MVP. It translates the domain and constraints into a concrete, scalable design without committing to low-level implementation details.

---

## 1. Architectural Principles

- **Backend-centric authority**: the backend enforces all domain rules
- **Thin client**: mobile app has no business logic authority
- **AI as advisory**: AI proposes, backend decides
- **Asynchronous where expensive**: heavy AI tasks never block the UI
- **Replaceable AI layer**: no vendor lock-in

---

## 2. High-Level Components

### 2.1 Mobile Application (Android)

Responsibilities:
- Capture user input (manual, text, images)
- Submit inputs to backend
- Display processing status and results
- Allow review and confirmation of pending Expenses

Communication:
- REST API (HTTPS)
- Polling or callback-based status updates

---

### 2.2 Backend Core (.NET)

Responsibilities:
- API gateway and authentication
- Domain validation and state management
- Expense lifecycle control
- Orchestration of AI processing
- Persistence

Subcomponents:
- API Layer
- Application Services
- Domain Layer
- Persistence Layer
- Background Job Dispatcher

---

### 2.3 AI Orchestration Service (Python)

Responsibilities:
- Receive normalized input from backend
- Route input to the correct AI agent:
  - Text classification agent
  - OCR + interpretation agent
- Normalize outputs into a strict JSON contract
- Compute confidence scores (0–100)

Constraints:
- No domain authority
- No persistence

---

### 2.4 Relational Database

Responsibilities:
- Persist domain entities
- Persist AI processing audit data
- Support soft deletes and historical inspection

---

## 3. Execution Flows

### 3.1 Manual Expense Creation (No AI)

1. User submits form via Mobile App
2. Backend validates domain rules
3. Expense is created as Confirmed
4. Expense is persisted
5. Mobile receives updated state

---

### 3.2 Text-Based AI Processing Flow

1. User submits plain text input
2. Backend creates ExpenseInput
3. Backend sends normalized input to AI service (sync)
4. AI returns proposed Expenses + confidence scores
5. Backend validates proposals against domain rules
6. Expenses are persisted as Pending or Confirmed
7. Mobile displays proposed Expenses for review

---

### 3.3 Image-Based AI Processing Flow (Async)

1. User uploads image
2. Backend creates ExpenseInput
3. Backend enqueues background job
4. Worker sends image/OCR data to AI service
5. AI processes and returns proposals
6. Backend validates and persists results
7. Mobile polls or receives callback

---

## 4. Synchronous vs Asynchronous Decisions

| Operation | Mode |
|---------|------|
| Manual entry | Synchronous |
| Plain text AI | Synchronous |
| Image OCR | Asynchronous |
| Expense confirmation | Synchronous |

---

## 5. Error Handling & Failure Modes

- AI service unavailable:
  - ExpenseInput marked as Error
  - User notified
- AI output violates domain rules:
  - Backend rejects or corrects proposals
- Partial failures:
  - Valid proposals are persisted
  - Invalid ones are discarded

---

## 6. Scalability Considerations

- AI services scale independently
- Image processing workers can scale horizontally
- Backend remains stateless where possible
- Database is the primary bottleneck and single source of truth

---

## 7. Explicit Non-Goals

- Real-time streaming
- Offline-first synchronization
- Multi-user concurrency handling
- Direct bank integrations

---

## Summary

This architecture ensures:
- Clear authority boundaries
- Predictable AI behavior
- Scalability without overengineering
- Alignment with the defined domain model and constraints

