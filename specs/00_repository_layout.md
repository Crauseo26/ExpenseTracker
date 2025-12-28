# Repository Bootstrap & Layout

## Purpose
This document defines the initial repository structure and ownership rules. It must be in place **before** delegating any code generation to AI agents.

This layout is considered **authoritative** for the MVP and acts as a guardrail for all automated development activities.

---

## Repository Strategy

- **Monorepo**
- Single Git repository containing all components of the system
- Shared `/specs` directory used as the source of truth for all agents

Rationale:
- Single developer
- Strong coupling between backend, AI pipeline and domain
- Easier context sharing for AI agents
- Reduced coordination overhead

---

## High-Level Structure

```
/
├─ specs/                # All system specifications (read-only for agents)
├─ backend/              # .NET backend (API + Domain)
├─ ai-service/           # Python-based AI processing service
├─ mobile/               # Mobile application (skeleton only for MVP)
├─ agents/               # Agent role definitions (prompts)
├─ scripts/              # Orchestration scripts for agents
├─ README.md
└─ .gitignore
```

---

## Component Responsibilities

### `/specs`
- Contains all specification documents
- Serves as the **single source of truth**
- **Read-only for AI agents**
- Any modification must be performed manually by the human owner

---

### `/backend`
- Technology: **.NET**
- Purpose:
  - Domain model
  - Business rules
  - Expense lifecycle management
  - REST API
  - Persistence

Initial structure:

```
/backend
 ├─ src/
 │   ├─ Expenses.Api/
 │   ├─ Expenses.Domain/
 │   └─ Expenses.Infrastructure/
 └─ tests/
```

Rules:
- Project skeletons are created manually using `dotnet new`
- AI agents may generate code **inside existing projects only**
- AI agents must not create new `.csproj` files or solutions

---

### `/ai-service`
- Technology: **Python**
- Purpose:
  - OCR processing
  - NLP and text normalization
  - AI agent orchestration
  - Confidence scoring

Structure:

```
/ai-service
 ├─ src/
 └─ tests/
```

Rules:
- Virtual environment and dependency management are defined manually
- AI agents may generate processing logic and internal modules
- AI agents must not alter environment or dependency definitions

---

### `/mobile`
- Mobile application
- MVP scope:
  - Directory exists
  - No functional implementation yet

```
/mobile
 └─ app/
```

Rules:
- AI agents are **not allowed** to generate mobile code during the MVP phase

---

### `/agents`
- Contains AI agent role definitions
- Each file represents a single agent profile

Example:
```
/agents
 ├─ backend.agent.md
 ├─ ai-pipeline.agent.md
 └─ reviewer.agent.md
```

These documents define:
- Agent responsibilities
- Allowed scope of changes
- Expected inputs and outputs

---

### `/scripts`
- Contains orchestration scripts
- Responsibilities:
  - Load specs
  - Select appropriate agents
  - Execute AI tools / CLIs
  - Capture outputs and logs
  - Enforce guardrails

---

## Ownership & Guardrails

- Repository structure is defined and owned by the human developer
- AI agents:
  - ❌ must not create new top-level directories
  - ❌ must not modify `/specs`
  - ❌ must not change repository layout
  - ✅ may generate or modify code within explicitly allowed directories

---

## Evolution Rules

- Structural changes are expected to be rare and intentional
- Any structural change must:
  1. Update this document
  2. Be applied manually
  3. Be communicated to all agents before execution

---

## Status

- This document is **active**
- Mandatory prerequisite before any AI-driven code generation

