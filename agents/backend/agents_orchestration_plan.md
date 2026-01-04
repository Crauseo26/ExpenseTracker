# Agents Orchestration Plan

## Purpose
This document defines **how multiple AI agents are orchestrated** to build the system incrementally, safely, and in parallel when possible. It establishes execution order, dependencies, handoff rules, and quality gates.

The goal is to:
- Maximize parallelism where safe
- Prevent responsibility overlap
- Ensure production-ready output at every step

---

## Core Principle

> **Agents never compete; they collaborate in a defined order.**

Each agent operates within strict boundaries and produces artifacts consumed by subsequent agents.

---

## Agent Inventory

### Backend Agents

1. **Domain Agent**
2. **Application Agent**
3. **Infrastructure Agent**
4. **API Agent**
5. **AI Integration Agent** (future)

### Supporting Agents (optional / later)

- Test Agent
- Mobile Agent
- DevOps Agent

---

## Execution Order (Backend MVP)

```text
Phase 1 ─ Domain
   ↓
Phase 2 ─ Application
   ↓
Phase 3 ─ Infrastructure
   ↓
Phase 4 ─ API
   ↓
Phase 5 ─ AI Integration
```

This order is **mandatory** for first implementation.

---

## Phase Details

### Phase 1 — Domain Agent

**Inputs:**
- `specs/03_domain_model.md`

**Outputs:**
- Domain entities
- Value objects
- Domain invariants

**Quality Gate:**
- Compiles successfully
- No infrastructure or API dependencies

---

### Phase 2 — Application Agent

**Depends on:** Domain Agent

**Inputs:**
- `specs/03_domain_model.md`
- `specs/06_api_and_contracts.md`
- `specs/07_ai_pipeline.md`

**Outputs:**
- Commands
- Queries
- Use case orchestration

**Quality Gate:**
- Backend builds successfully
- Unit tests pass (if present)

---

### Phase 3 — Infrastructure Agent

**Depends on:** Application Agent

**Inputs:**
- `specs/05_persistence_model.md`
- Domain & Application abstractions

**Outputs:**
- DbContext
- Migrations
- Repository implementations

**Quality Gate:**
- Backend builds successfully
- Migrations compile and apply

---

### Phase 4 — API Agent

**Depends on:** Application + Infrastructure Agents

**Inputs:**
- `specs/06_api_and_contracts.md`

**Outputs:**
- Controllers
- DTOs
- Middleware

**Quality Gate:**
- Backend builds successfully
- Integration tests pass (if present)

---

### Phase 5 — AI Integration Agent (Later)

**Depends on:** Application + Infrastructure

**Inputs:**
- `specs/07_ai_pipeline.md`

**Outputs:**
- AI service client
- Normalization adapters

**Quality Gate:**
- AI pipeline contracts respected
- No domain or API violations

---

## Parallelism Rules

Parallel execution is allowed **only when outputs do not overlap**.

Allowed examples:
- API Agent (read-only endpoints) + Infrastructure Agent (repositories)
- Test Agent + API Agent

Forbidden examples:
- Domain Agent + Application Agent
- Application Agent + Infrastructure Agent

---

## Handoff Rules

- Agents communicate **only through code and specs**, never assumptions
- Each agent must rely on outputs from previous phases
- No agent may rewrite artifacts owned by another agent

---

## Validation & Recovery

If a phase fails:

1. Stop downstream agents
2. Fix the failing phase
3. Re-run affected phases

Never "patch around" a broken upstream phase.

---

## Human-in-the-Loop Checkpoints

Human review is required after:

- Domain Agent completion
- Application Agent completion
- Infrastructure Agent completion

API and later phases may iterate faster.

---

## Outcome

Following this orchestration plan ensures:

- Clean separation of concerns
- Predictable agent behavior
- Minimal rework
- A scalable foundation for parallel AI-assisted development

