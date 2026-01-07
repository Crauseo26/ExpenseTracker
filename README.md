# FinancIA

> Personal expense tracking with AI-powered automation

[![Status](https://img.shields.io/badge/status-pre--development-orange)]()
[![Phase](https://img.shields.io/badge/phase-workflow%20setup-blue)]()

---

## 📋 Overview

**FinancIA** is a personal finance management application designed to streamline expense tracking through AI-powered data entry automation. The system allows users to submit expenses via multiple input methods (manual entry, text, images) and uses AI to propose structured expense data, while maintaining strict backend authority over all domain decisions.

### Core Philosophy

- **AI proposes, Backend decides** - AI services provide suggestions with confidence scores, but the backend maintains final authority
- **User-owned and secure** - All data is scoped to individual users with no cross-user visibility
- **Expense-centric domain** - Focused on correctness, auditability, and future evolution
- **Agent-oriented development** - Built using coordinated AI agents following strict specifications

---

## 🏗️ Architecture

The system is architected as a **monorepo** with clear separation of concerns:

### Components

| Component | Technology | Responsibility |
|-----------|-----------|----------------|
| **Backend** | .NET 8+ | Domain logic, state management, persistence, orchestration, REST API |
| **AI Service** | Python | Input processing (OCR, NLP), expense proposals, confidence scoring |
| **Mobile App** | Android | Thin client for user interaction, displays backend state |

### Key Characteristics

- **Backend as Single Source of Truth** - All domain rules and state transitions enforced server-side
- **Asynchronous AI Processing** - Heavy workloads (OCR) handled via background jobs
- **Replaceable AI Layer** - No vendor lock-in, AI services are stateless and advisory only
- **RESTful API** - Clean HTTP interface for synchronous operations

---

## 📂 Repository Structure

```
/
├── specs/              # System specifications (single source of truth)
├── backend/            # .NET backend (API + Domain + Infrastructure)
├── ai-service/         # Python AI processing service
├── mobile/             # Android mobile application
├── agents/             # AI agent role definitions
├── backlog/            # MVP backlog and execution tracking
│   ├── tasks/          # Individual task details
│   └── 02_execution_plan.md  # Live development status
├── scripts/            # Orchestration scripts
├── PROJECT_CONTEXT.md  # Meta-level progress tracking
└── README.md           # This file
```

---

## 📖 Documentation

### Essential Reading

**Start Here:**
1. [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md) - Current project status, role definitions, and onboarding
2. [`specs/00_repository_layout.md`](specs/00_repository_layout.md) - Repository structure and rules
3. [`specs/01_system_responsibilities.md`](specs/01_system_responsibilities.md) - Component boundaries
4. [`specs/02_constraints.md`](specs/02_constraints.md) - Technical constraints and stack decisions

### Core Specifications

The `/specs` directory contains the complete system design. Files are numbered for sequential reading:

- **00-02**: Repository, responsibilities, and constraints
- **03-05**: Domain model, architecture, and persistence
- **06-09**: API contracts, AI pipeline, lifecycle rules, and workflows
- **10-14**: Agent system, guardrails, orchestration, Git workflow, and conventions

### Development Tracking

- [`backlog/01_mvp_backlog.md`](backlog/01_mvp_backlog.md) - Original feature backlog (static)
- [`backlog/02_execution_plan.md`](backlog/02_execution_plan.md) - **Live execution status** (updated by Lead Agent)
- [`backlog/tasks/`](backlog/tasks/) - Individual task details (created during execution)

---

## 🚀 Current Status

**Phase:** Pre-Development / Workflow Setup

**Progress:** ~40% of setup phases complete

### Completed ✅
- ✅ Complete architectural specifications
- ✅ Agent role definitions (Lead + 4 specialized backend agents)
- ✅ Git workflow and review protocol
- ✅ Execution plan system with task tracking
- ✅ Technical conventions framework

### In Progress 🔄
- 🔄 System validation with first task execution

### Next Steps 📝
- Execute first task (F01-T01: User aggregate)
- Validate agent workflow end-to-end
- Begin iterative MVP development

**Note:** No production code exists yet - this is intentional. Infrastructure and workflow must be validated before feature development begins.

---

## 🛠️ Development Approach

### Agent-Oriented Development

The project uses a multi-agent system for development:

- **Lead Agent** - Orchestrates work, manages execution plan, coordinates specialized agents
- **Domain Agent** - Implements domain model and business rules
- **Application Agent** - Implements use cases and orchestration logic
- **Infrastructure Agent** - Implements persistence, repositories, and database
- **API Agent** - Implements HTTP endpoints and contracts

### Workflow

1. Lead Agent reads execution plan and identifies next task
2. Lead Agent creates detailed task specification
3. Lead Agent delegates to appropriate specialized agent
4. Specialized agent implements changes with atomic commits
5. Lead Agent verifies build and pushes branch for review
6. Human reviews and merges
7. Lead Agent updates execution plan

### Key Principles

- **Specifications as Authority** - All agents follow `/specs` documents strictly
- **Atomic Tasks** - Small, reviewable units of work
- **One Commit Per Logical Change** - Clear git history
- **Branch Per Task** - Isolated changes with proper naming
- **Human in the Loop** - Review and approval at key checkpoints

---

## 🔧 Technical Stack

### Backend (.NET)
- Framework: .NET 8+
- Language: C# 12
- ORM: Entity Framework Core
- Database: PostgreSQL or SQL Server (relational)
- Architecture: Clean Architecture / Onion Architecture

### AI Service (Python)
- Purpose: OCR, NLP, text normalization, expense extraction
- API: Exposes structured proposals with confidence scores
- Stateless: No persistence, no domain authority

### Mobile (Android)
- Target: Android (MVP)
- Role: Thin client - no business logic
- Communication: REST API over HTTPS

---

## 📦 Building and Running

### Current State

⚠️ **No code exists yet** - the repository contains specifications only.

Build and run instructions will be added once the backend and AI services are implemented.

### Future Instructions (Placeholder)

```bash
# Backend
cd backend
dotnet restore
dotnet build
dotnet run --project src/Expenses.Api

# AI Service
cd ai-service
pip install -r requirements.txt
python src/main.py

# Mobile
# Instructions to be added
```

---

## 🤝 Contributing

This is currently a personal project built with AI assistance. The development process follows a strict agent-oriented workflow defined in the specifications.

### For AI Agents

If you are an AI agent working on this project:

1. Read [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md) first
2. Understand your role and responsibilities
3. Follow specifications in `/specs` strictly
4. Check [`backlog/02_execution_plan.md`](backlog/02_execution_plan.md) for current status
5. Never make architectural decisions - ask the human

### For Humans

If you're interested in contributing:

1. Review the complete specification suite in `/specs`
2. Understand the agent-oriented development approach
3. Reach out to discuss potential contributions

---

## 📄 License

[License to be determined]

---

## 🔗 Links

- **Documentation**: See `/specs` directory
- **Progress Tracking**: See `PROJECT_CONTEXT.md` and `backlog/02_execution_plan.md`
- **Agent Definitions**: See `/agents` directory

---

## 📞 Contact

[Contact information to be added]

---

**Last Updated:** 2025-01-06  
**Current Phase:** Workflow Setup & Validation  
**Next Milestone:** First task execution with Lead Agent
