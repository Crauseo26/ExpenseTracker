# Execution Plan

## Purpose
This document tracks the **current execution state** of the MVP development. It is the single source of truth for:
- What has been completed
- What is currently in progress
- What is ready for human review
- What is blocked
- What is pending in priority order

This document is **managed by the Lead Agent** and updated after each merge.

---

## Current Phase
**Phase 3 — Integration & Mobile**

Target: Connect Backend to AI Service and begin Mobile App development.

---

## Task Status Legend

- ✅ **Completed**: Merged to `develop`
- 🔄 **In Progress**: Agent currently working
- 📋 **Ready for Review**: Branch pushed, awaiting human review
- ⛔ **Blocked**: Cannot proceed due to dependency
- 📝 **Pending**: Not yet started, in priority order

---

## Status Dashboard

### Completed ✅
- [x] F01-T01: Define User aggregate (Domain Agent)
- [x] F01-T02: Migrate to ASP.NET Core Identity
- [x] F01-T02.1: Complete Swagger UI Integration for Authentication
- [x] F01-T03: Enforce user scoping in repositories (Infrastructure Agent)
- [x] F02-T01: Implement Expense aggregate lifecycle (Domain Agent)
- [x] F02-T02: Implement expense use cases (Application Agent)
- [x] F03-T01: Implement Account and AccountGroup aggregates (Domain Agent)
- [x] F03-T02: Implement account management use cases (Application Agent)
- [x] F04-T01: Implement ExpenseInput aggregate (Domain Agent)
- [x] F04-T02: Implement AI integration interfaces (Application Agent)
- [x] F04-T03: Implement AI service client (Infrastructure Agent)
- [x] F06-T01: Create DbContext and entity configurations (Infrastructure Agent)
- [x] F06-T02: Implement repositories (Infrastructure Agent)
- [x] F06-T03: Create initial migration (Infrastructure Agent)
- [x] F07-T01: Implement Expense CRUD endpoints (API Agent)
- [x] F07-T02: Implement ExpenseInput submission endpoint (API Agent)
- [x] F07-T03: Implement query endpoints with filters (API Agent)
- [x] F08-T01: Add unit tests for domain (Application Agent)
- [x] F08-T02: Add integration tests for API (Application Agent)
- [x] F08-T03: Configure CI pipeline (Manual / DevOps)
- [x] F09-T01: Setup Python project structure and FastAPI skeleton (AI Agent)
- [x] F09-T02: Implement LLM orchestration and system prompt (AI Agent)
- [x] F09-T03: Implement JSON extraction and confidence scoring (AI Agent)
- [x] F09-T05: Implement Service Security & Dynamic Scoring (AI Agent)

### In Progress 🔄
None

### Ready for Review 📋
- [x] F10-T01: Update Backend AI Client Contracts & Security (Infrastructure Agent)

### Blocked ⛔
None

---

## Pending Tasks (Priority Order) 📝

### Feature 10 — Backend-AI Integration

**Goal**: Update .NET Backend to consume the secured and configurable AI Service.

**Tasks:**
- [ ] F10-T01: Update Backend AI Client Contracts & Security (Infrastructure Agent)
- [ ] F10-T02: Inject User Accounts into AI Request (Application Agent)
- [ ] F10-T03: End-to-End Integration Test (Application Agent)

**Agent Assignments:**
- Backend-Infrastructure-Agent
- Backend-Application-Agent

**Dependencies:**
- F10 depends on F09 completion (API Contract v1.0)

---

### Feature 11 — Mobile App Foundation (Android)

**Goal**: Initialize the Android project and core structure.

**Tasks:**
- [ ] F11-T01: Initialize Android Project (Kotlin/Compose)
- [ ] F11-T02: Implement Auth & JWT Storage
- [ ] F11-T03: Implement Camera/Gallery Capture

**Agent Assignments:**
- Mobile-Agent (New)

---

## Execution Strategy

### Phase 1 Batch (Sequential Foundation)
1. F01-T01 → F01-T02 → F01-T03
   - Must complete before other features

### Phase 2 Batch (Parallel Domain Work)
2. F02-T01 (can start after F01-T01)
3. F03-T01 (can run parallel with F02-T02)

### Phase 3 Batch (Application Layer)
4. F02-T02 → F04-T01 → F04-T02 → F04-T03 → F05-T01 → F05-T02

### Phase 4 Batch (Persistence)
5. F06-T01 → F06-T02 → F06-T03

### Phase 5 Batch (API)
6. F07-T01 → (F07-T02 || F07-T03)

### Phase 6 Batch (Testing)
7. F08-T01 || F08-T02 → F08-T03 (manual)

### Phase 7 Batch (AI Service)
8. F09-T01 → F09-T02 → F09-T03 → F09-T05

### Phase 8 Batch (Integration)
9. F10-T01 → F10-T02 → F10-T03

---

## Notes & Decisions

### 2026-01-19 (Evening)
- F10-T01 completed: Backend AI Client updated to match v1.0 API contract
- Updated DTOs to align with Spec 15:
  - `AIProcessingRequest`: Changed to `rawText`, added `availableAccounts` list, updated metadata to ISO-8601 format
  - `AIProcessingResponse`: Added `processingTimeMs`, updated proposal structure with metadata object
  - `AIExpenseProposal`: Added metadata fields (merchant, rawExtraction, suggestedAccount), changed purchaseDate to string
- Implemented security: Added `X-Service-Token` header support using `AIService:ApiKey` configuration
- Updated endpoint from `/api/process` to `/process-text` to match specification
- All DTOs now use `JsonPropertyName` attributes for proper camelCase serialization
- Solution builds successfully with no errors
- Branch pushed: feature/F10-T01-backend-ai-client-update

### 2026-01-19 (Afternoon)
- F09-T05 completed: Security and dynamic scoring implemented
- Secured AI service with `X-Service-Token` header
- Implemented `ScoringConfig` and `PenaltyConfig` models
- Refactored `refinement.py` to use injected configuration
- Added comprehensive security tests
- **Feature F09 (AI Service) is now fully complete**

### 2026-01-19 (Morning)
- F09-T03 completed: Deterministic scoring logic implementation
- Removed confidence score from LLM prompt (LLM must not guess)
- Implemented python-side scoring with penalties:
  - Missing/Zero amount: -0.25
  - Unknown currency/type: -0.25
  - Inferred date: -0.05
  - Empty description: -0.20
  - Short description (<3 chars): -0.10
  - Generic description (blacklist): -0.05
- Added unit tests for all penalty scenarios
- Integrated scoring into Gemini provider flow

### 2026-01-18 (Night)
- F09-T02 completed: LLM Orchestration with Gemini
- Implemented `GeminiLLMProvider` using `google-genai` library
- Configured "JSON Mode" for reliable structured output
- Updated Pydantic models to include `availableAccounts`
- Implemented robust prompts with domain context injection
- Branch pushed: feature/F09-T02-llm-orchestration

### 2026-01-18 (Evening)
- F09-T01 completed: Python AI service project setup
- Created FastAPI application structure in `ai-service/` directory
- Implemented `/health` endpoint returning status and version (200 OK verified)
- Implemented placeholder `/process-text` endpoint with full request/response models
- Service verified to start successfully on port 5000
- Branch pushed: feature/F09-T01-ai-service-setup