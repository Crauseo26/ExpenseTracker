# B — Constraints & Technical Direction

## Purpose
This document captures the **explicit constraints, preferences, and technical directions** for the MVP. These constraints are intentional and help guide architectural, tooling, and implementation decisions.

Anything not explicitly allowed or required here should be considered **out of scope for the MVP**.

---

## 1. Platform & Application Scope

### 1.1 Target Platform

- The MVP targets **Android** only.
- Initial usage is personal, with potential external testers in later stages.
- iOS and Web are **out of scope** for the MVP, but considered possible future evolutions.

---

### 1.2 Mobile Development Approach

- No strict requirement between native vs cross-platform.
- Priority is given to:
  - Development speed
  - Simplicity
  - Good documentation and DX
  - Potential reuse for future platforms
- No complex native API access is required for the MVP.

---

### 1.3 Mobile Development Experience

- Limited prior mobile development experience.
- No strong technology preference.
- Technology choice should minimize friction and learning overhead.

---

## 2. Backend & Core Stack

### 2.1 Backend Language & Ecosystem

- **.NET** is the primary backend platform and authority.
- Responsibilities of .NET backend:
  - API layer
  - Domain logic
  - State management
  - Persistence
  - Orchestration

- **Python** is used for AI-related processing only:
  - OCR
  - Text classification
  - Extraction
  - Scoring

---

### 2.2 Backend Style

- **REST API** for synchronous operations:
  - Manual inputs
  - Plain text processing
- **Asynchronous jobs** for:
  - Image processing
  - Heavy AI workloads
- Event-driven or streaming architectures are **explicitly excluded** from the MVP.

---

### 2.3 Technologies to Avoid

- Java-based stacks are explicitly avoided.
- Preferred ecosystems:
  - .NET
  - Python
  - (Optionally Node.js, but not primary)

---

## 3. Data Persistence

### 3.1 Database Type

- **Relational database** is required.
- The domain is highly structured:
  - Expenses
  - Accounts
  - Expense Groups
  - Inputs
  - States

---

### 3.2 Primary Priority

Order of priority for technical decisions:

1. Speed of development
2. Personal learning value
3. Fine-grained performance optimization

---

### 3.3 Domain-Driven Alignment

- The persistence model must reflect the domain clearly.
- Explicit modeling of:
  - Expense states
  - Difference between raw input, AI proposal, and confirmed data
- Full academic DDD is not required, but the model must be expressive and evolvable.

---

## 4. AI & External Services

### 4.1 Use of External AI Providers

- External AI services are allowed for the MVP.
- Initial dependency is acceptable.
- Vendor lock-in must be **avoidable**.

---

### 4.2 AI Replaceability

- AI services must be replaceable without changing domain logic.
- The .NET backend remains the final authority.
- AI services act strictly as proposal generators.

---

### 4.3 Cost-Sensitive Processing

- Image processing (OCR + interpretation) is considered expensive.
- Such processing:
  - Must be asynchronous
  - May have higher latency
  - May have reduced coverage

---

## 5. Security & Data Sensitivity

### 5.1 Data Sensitivity Level

- The system handles **personal financial data**.
- Even as a personal MVP, basic security practices are mandatory.

---

### 5.2 Security Expectations for MVP

- Simple authentication mechanism
- Basic session or token handling
- No advanced authorization models (roles, permissions, etc.)

---

### 5.3 Regulatory Considerations

- No explicit regulatory requirements are addressed in the MVP.
- Regulatory compliance is deferred until commercialization is considered.

---

## 6. DevOps & Environment

### 6.1 Deployment Environment

- Backend is designed to run in the **cloud** from the start.
- No complex infrastructure is required.
- Ease of deployment and maintenance is prioritized.

---

### 6.2 Observability & Operations

- Basic observability is required:
  - Clear, structured logs
- CI/CD:
  - Desirable
  - Must remain simple
- Advanced monitoring is not required for the MVP.

---

### 6.3 Team Composition

- Single human developer.
- Heavy use of AI agents for:
  - Design
  - Coding assistance
  - Review

---

## 7. Explicitly Deferred Decisions

### 7.1 Deferred Topics

- Final AI provider selection
- Multi-user architecture
- Multi-device synchronization
- Monetization strategy
- Direct bank integrations
- Advanced analytics

---

### 7.2 Expected Future Changes

- Addition of Web application
- Evolution of AI processing pipeline
- Adjustments to data model based on real usage
- Refinement of confidence thresholds and decision rules

---

## Summary

This document defines **non-negotiable constraints** for the MVP. All architectural and implementation decisions must comply with these constraints unless this document is explicitly updated.

