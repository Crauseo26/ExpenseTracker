# Agent Responsibilities & Boundaries

## Purpose
This document serves as the **directory of specialized agents** available for the project. It defines the high-level responsibility of each agent and points to their detailed definition file.

The Lead Agent uses this list to delegate tasks.

---

## 1. Backend Agents (See `agents/backend/`)

### Backend-Domain-Agent
- **Role:** Domain Modeler (DDD)
- **Focus:** Entities, Value Objects, Domain Services, Business Rules.
- **Definition:** `agents/backend/backend_domain_agent.md`

### Backend-Application-Agent
- **Role:** Use Case Implementer (CQRS)
- **Focus:** Command/Query Handlers, DTOs, Orchestration logic.
- **Definition:** `agents/backend/backend_application_agent.md`

### Backend-Infrastructure-Agent
- **Role:** Infrastructure & Persistence
- **Focus:** EF Core, Repositories, External Service Clients (AI Client), Migrations.
- **Definition:** `agents/backend/backend_infrastructure_agent.md`

### Backend-API-Agent
- **Role:** API Expositor
- **Focus:** Controllers, Endpoints, Authentication (JWT), Swagger.
- **Definition:** `agents/backend/backend_api_agent.md`

---

## 2. AI Services Agents (See `agents/ai_service/`)

### AI-Python-Agent
- **Role:** AI Service Developer (Python/FastAPI)
- **Focus:** LLM Orchestration, Prompt Engineering, Deterministic Scoring, FastAPI implementation.
- **Definition:** `agents/ai_service/ai_python_agent.md`

---

## 3. Mobile Agents (See `agents/mobile/`)

### Mobile-Flutter-Agent
- **Role:** Mobile Developer (Flutter)
- **Focus:** UI/UX implementation, State Management (Riverpod), API Consumption (Dio), Local Storage.
- **Definition:** `agents/mobile/mobile_flutter_agent.md`

---

## 4. Cross-Cutting

### Lead Agent (Orchestrator)
- **Role:** Project Manager & Architect
- **Focus:** Coordination, Spec Enforcement, Git Workflow, Execution Plan updates.
- **Definition:** `agents/lead_agent.md`

---

## Delegation Rules

1.  **Backend Logic:** Delegate to the specific layer agent (Domain vs Application vs Infrastructure).
2.  **Database/Migrations:** Delegate to **Backend-Infrastructure-Agent**.
3.  **API Endpoints:** Delegate to **Backend-API-Agent**.
4.  **AI Logic (Python):** Delegate to **AI-Python-Agent**.
5.  **Mobile App:** Delegate to **Mobile-Flutter-Agent**.

If a task spans multiple layers, the Lead Agent must break it down into subtasks and assign them sequentially or in parallel.