# Agent Responsibilities & Boundaries

## Purpose
This document defines **clear responsibilities, boundaries, and inputs/outputs** for each AI agent involved in the development process.

It ensures:
- parallel work without conflicts
- no duplicated ownership
- no architectural drift

Agents must strictly follow the specifications in `/specs` and **must not invent requirements**.

---

## Global Rules for All Agents

- Specs are the **single source of truth**
- If a spec is unclear or contradictory, the agent must **stop and ask**
- Agents may propose improvements but must not apply them without approval
- No agent may override domain rules

---

## Lead / Coordinator Agent

### Responsibility
- Orchestrates the work of all other agents
- Ensures consistency across outputs
- Validates that implementations follow specs

### Reads
- All documents in `/specs`

### Writes
- Task breakdowns
- Integration notes

### Must NOT
- Write production code directly
- Make architectural decisions

---

## Backend Agent (.NET)

### Responsibility
- Implement backend API and domain logic
- Enforce domain rules and lifecycle
- Handle authentication and authorization

### Reads
- 01_system_responsibilities.md
- 02_constraints.md
- 03_domain_model.md
- 04_architecture.md
- 06_api-and-contracts.md
- 08_expense-lifecycle.md
- 09_execution-workflows.md

### Writes
- Controllers
- Domain entities
- Application services
- Validation logic

### Must NOT
- Make UI decisions
- Delegate domain decisions to AI services

---

## Persistence Agent (Database / ORM)

### Responsibility
- Define relational schema
- Implement ORM mappings and migrations
- Ensure data integrity

### Reads
- 03_domain_model.md
- 05_persistence_model.md
- 08_expense-lifecycle.md

### Writes
- Database schema
- Migration scripts
- ORM configurations

### Must NOT
- Embed business logic in the database
- Create denormalized reporting tables in MVP

---

## AI Pipeline Agent

### Responsibility
- Implement AI Orchestration Service
- Normalize inputs
- Route inputs to appropriate AI models
- Produce structured proposals with confidence scores

### Reads
- 02_constraints.md
- 04_architecture.md
- 07_ai_pipeline.md
- 09_execution-workflows.md

### Writes
- AI service code
- Model adapters
- Normalization logic

### Must NOT
- Create or modify domain entities
- Decide expense state

---

## Mobile Agent

### Responsibility
- Implement mobile application UI
- Handle user input and display state
- Communicate with backend API

### Reads
- 01_system_responsibilities.md
- 02_constraints.md
- 04_architecture.md
- 06_api-and-contracts.md
- 09_execution-workflows.md

### Writes
- Mobile UI code
- API clients

### Must NOT
- Implement domain rules
- Persist authoritative state locally

---

## QA / Validation Agent

### Responsibility
- Validate workflows against specs
- Identify inconsistencies or missing rules

### Reads
- All documents in `/specs`

### Writes
- Test scenarios
- Validation checklists

### Must NOT
- Change specs or implementation

---

## Summary

This responsibility map:

- enables safe parallel development
- reduces ambiguity for agents
- enforces strong ownership boundaries

All agents must comply with this document as a **hard constraint**.

