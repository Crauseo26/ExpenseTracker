# Engineering Guardrails

## Purpose
This document defines **non-negotiable engineering rules** that all implementations must follow.

Its goal is to:
- protect architectural integrity
- prevent common anti-patterns
- constrain AI agents within safe boundaries

These guardrails apply to **all code written by humans or AI agents**.

---

## General Principles

- Correctness > convenience
- Explicitness > cleverness
- Domain rules > technical shortcuts
- Specs > agent assumptions

If a guardrail conflicts with convenience, the guardrail wins.

---

## Architecture Guardrails

### Backend Authority

- The backend is the **only source of truth**
- All domain decisions happen server-side
- Frontend must never infer or compute domain state

---

### Layering Rules (Backend)

Allowed dependencies:

```
API → Application → Domain → Persistence
```

Forbidden:
- Domain depending on infrastructure
- Controllers containing business logic
- Persistence enforcing domain rules

---

## Domain Guardrails

- All domain rules must live in domain or application services
- State transitions must follow `08_expense-lifecycle.md`
- No entity may be persisted in an invalid state

---

## API Guardrails

- APIs must be explicit and versionable
- No implicit behavior based on client type
- Validation errors must be deterministic

Forbidden:
- Hidden side effects
- Silent corrections

---

## AI Integration Guardrails

- AI output is **always untrusted**
- AI services may only propose data
- Backend validates and decides

Forbidden:
- AI creating domain entities
- AI deciding expense state
- AI bypassing validation rules

---

## Persistence Guardrails

- Relational database only
- No business logic in SQL or triggers
- No denormalized reporting tables in MVP

---

## Mobile Guardrails

- Mobile app is a thin client
- No local authoritative state
- Local cache (if any) is disposable

---

## Error Handling Guardrails

- Fail fast on domain violations
- Partial writes are forbidden
- Async jobs must be idempotent

---

## Testing Guardrails

Minimum expectations:
- Unit tests for domain rules
- Integration tests for workflows
- State transition tests

---

## Forbidden Anti-Patterns

- Fat controllers
- God services
- Business logic in UI
- Business logic in AI prompts
- Implicit state changes

---

## Change Policy

- Guardrails may only be changed intentionally
- Any change requires spec update and approval

---

## Summary

These guardrails:

- protect long-term maintainability
- enable safe AI-driven development
- reduce architectural entropy

All agents must treat this document as a **hard constraint**.

