# Project Context & Progress

## Purpose

This document serves as the **single source of truth** for understanding the current state of the FinancIA project from a **meta-development perspective**. It is designed to enable seamless handoff between AI agents working on project setup, workflow definition, and automation infrastructure.

**This document is NOT about the MVP features** (those are tracked in `backlog/02_execution_plan.md`). Instead, it tracks:
- The setup and configuration of the development process itself
- The agent-oriented workflow system
- Progress on building the automation infrastructure
- Context needed for any successor AI agent to continue the work

**Target Audience:**
- Successor AI agents taking over this orchestration role
- Human developer (as reference and validation point)

---

## Your Role (As Current/Successor Agent)

### Role Definition

You are the **Project Orchestrator & Technical Partner**.

Your responsibilities are:
- **NOT to implement MVP features** (that's the Lead Agent's job)
- **NOT to write production code** (that's specialized agents' job)
- **TO design, validate, and evolve** the project's workflow, specs, and agent system
- **TO act as** technical co-architect, methodology guardian, and reasoning partner
- **TO maintain** architectural integrity and ensure all decisions align with constraints
- **TO ensure** high-level documentation (`README.md`, `PROJECT_CONTEXT.md`) is synchronized with the actual project state after every PR merge and before the next task loop. **All such updates must be pushed to the remote repository immediately.**
- **TO review** `specs/12_master_orchestration_prompt.md` and `specs/10_agent_responsibilities.md` whenever an agent definition file (`agents/...`) is modified, removed, or added, ensuring consistency across the system.

You are essentially the "meta-developer" — you build the system that enables other agents to build the product.

---

### How to Continue Work (Onboarding for Successor Agent)

If you are a new AI agent taking over this role:

#### 1. Initial Context Gathering (15-20 minutes reading)

**Read these documents in order:**

1. **This document** (`PROJECT_CONTEXT.md`) - You're reading it now
2. `README.md` - High-level project overview
3. `specs/00_repository_layout.md` - Repository structure and rules
4. `specs/01_system_responsibilities.md` - Component boundaries
5. `specs/02_constraints.md` - Technical constraints and decisions
6. `specs/11_engineering_guardrails.md` - Non-negotiable rules
7. `specs/13_git_workflow_and_review_protocol.md` - Git workflow
8. `specs/14_technical_conventions.md` - Coding conventions
9. `backlog/02_execution_plan.md` - Current MVP development status

**Optional but recommended:**
- `specs/12_master_orchestration_prompt.md` - Lead Agent instructions
- `agents/lead_agent.md` - Lead Agent definition
- All files in `agents/backend/` - Specialized agent definitions

#### 2. Understand Current State

Check the **Development Roadmap** section below to see:
- What has been completed ✅
- What is in progress 🔄
- What is pending 📝

#### 3. Review Recent Changes

```bash
# Check recent commits
git log --oneline -20

# Check current branch
git branch

# Check for uncommitted changes
git status
```

#### 4. Identify Next Task

Look at the **Current Focus** and **Next Steps** sections below.

#### 5. Confirm with Human

Before making any changes:
- Summarize your understanding of the current state
- Confirm the next task with the human
- Ask for clarification on any ambiguities

---

## Project Overview

### Ultimate Goal

Build **FinancIA**: A personal expense tracking application with AI-powered data entry automation.

**Target:** MVP (Minimum Viable Product) for Android

**Key characteristics:**
- User-owned and secure by default
- AI proposes, backend decides (AI never has domain authority)
- Expense-centric domain model
- Built using agent-oriented development

---

### Current Overall Status

**Phase:** Phase 6 - Mobile App Foundation

**What exists:**
- ✅ Complete architectural specifications
- ✅ Backend Core (.NET 9): Domain, Identity, Persistence, API (100% Complete)
- ✅ AI Service (Python): FastAPI, Gemini, Deterministic Scoring (100% Complete)
- ✅ Integration: Backend consumes AI Service with secure, context-aware logic
- ✅ CI/CD: GitHub Actions pipeline for Backend
- ✅ Agent definitions for Backend & AI

**Next Major Milestone:**
Initialize the Android Project (Feature 11).

---

## Development Roadmap

### Phase 1: Foundation ✅ COMPLETED
**Outcome:** Complete specification suite ready for agent-driven development.
**Completed:** 2025-01-06

### Phase 2: Workflow Automation ✅ COMPLETED
**Outcome:** Complete workflow automation system ready for use.
**Completed:** 2025-01-06

### Phase 3: Validation & First Execution ✅ COMPLETED
**Outcome:** Validation of agent system with Feature F01.
**Completed:** 2026-01-07

### Phase 4: Iteration & Refinement ✅ COMPLETED
**Outcome:** Feature F01 fully implemented, process refined.
**Completed:** 2026-01-10

### Phase 5: Scale & Parallelize (Backend Core) ✅ COMPLETED
**Goal:** Execute multiple backend features.
**Tasks:**
- [x] F02 (Expense Lifecycle)
- [x] F03 (Accounts)
- [x] F04 (AI Interfaces)
- [x] F05 (Confirmation Rules)
- [x] F06 (Persistence)
- [x] F07 (API Endpoints)
- [x] F08 (Tests & CI)
- [x] F09 (AI Service Implementation)
- [x] F10 (Backend-AI Integration)

**Outcome:** Backend and AI Service are fully implemented, tested, and integrated.
**Completed:** 2026-01-23

---

### Phase 6: Mobile App Foundation 🔄 IN PROGRESS

**Goal:** Establish the Android client.

**Tasks:**
- [ ] Initialize Android Project (Kotlin/Compose)
- [ ] Implement Authentication Logic
- [ ] Implement Camera Capture

---

### Phase 7: Full MVP Development 📝 PENDING

**Goal:** Complete all UI features on Mobile.

---

## Conversation History / Changelog

*(Previous history omitted for brevity...)*

### Session 10: 2026-01-16 (API Completion & Account Endpoints)
**Major Accomplishments:**
1. **Executed F07-T03 (Account & Query Endpoints):** Lead Agent orchestrated the implementation of `AccountsController` and `AccountGroupsController`.
2. **Completed Feature F07:** All MVP API endpoints are now implemented.

### Session 11: 2026-01-18 (AI Service Foundation)
**Participants:** Human, Orchestrator, Lead Agent
**Major Accomplishments:**
1. **Defined AI Specs:** Created `specs/15_ai_service_api_contract.md`.
2. **Initialized AI Service (F09-T01):** Setup FastAPI, Pydantic, Uvicorn structure.
3. **Implemented LLM Logic (F09-T02):** Integrated Google Gemini Flash via `google-genai`.
4. **Context Awareness:** Updated system to accept `availableAccounts` for smart suggestions.

### Session 12: 2026-01-19 (AI Refinement & Security)
**Participants:** Human, Orchestrator, Lead Agent
**Major Accomplishments:**
1. **Implemented Deterministic Scoring (F09-T03):** Shifted from LLM confidence to rule-based scoring (penalties for missing data).
2. **Implemented Security (F09-T05):** Secured AI Service with `X-Service-Token`.
3. **Dynamic Configuration:** Made scoring rules configurable via API request.
4. **Completed Feature F09:** AI Service is production-ready.

### Session 13: 2026-01-21 to 2026-01-23 (Backend-AI Integration)
**Participants:** Human, Orchestrator, Lead Agent
**Major Accomplishments:**
1. **Updated Backend Client (F10-T01):** Aligned .NET DTOs with Python API Contract v1.0.
2. **Implemented Context Injection (F10-T02):** `ProcessExpenseInputCommandHandler` now fetches user accounts and injects them into the AI request.
3. **Implemented Matching Logic:** Backend successfully matches AI string suggestions ("Supermercado") to Account UUIDs.
4. **Validated Integration (F10-T03):** Implemented Mock Integration Tests verifying the full flow.
5. **Completed Feature F10:** The system is fully integrated at the service level.

---

## Current Focus

**Active Phase:** Phase 6 - Mobile App Foundation

**Current Task:** Prepare for F11-T01: Initialize Android Project

**What Needs to Happen:**
1. Define `agents/mobile/mobile_android_agent.md`.
2. Guide human to kick off Lead Agent for F11-T01.

---

## Status Summary (Quick Reference)

**Last Updated:** 2026-01-23

**Current Phase:** 6 - Mobile App Foundation

**Overall Progress:**
- Backend: 100% (F01-F08, F10)
- AI Service: 100% (F09)
- Mobile: 0%

**Blockers:** None

**Next Major Milestone:** Initialize Android App structure.

**Confidence Level:** Very High - Backend architecture is solid and fully tested.

---

**End of Document**
