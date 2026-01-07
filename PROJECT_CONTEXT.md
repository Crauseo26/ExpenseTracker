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

**Phase:** Pre-Development / Workflow Setup

**What exists:**
- ✅ Complete architectural specifications (all domain, constraints, APIs defined)
- ✅ Agent role definitions (Lead + 4 specialized backend agents)
- ✅ Git workflow and review protocol
- ✅ Execution plan system with task tracking
- ✅ Technical conventions framework
- ❌ No production code yet (intentional - infrastructure first)

**Next Major Milestone:**
First execution of Lead Agent to implement F01-T01 (User aggregate)

---

## Development Roadmap

### Phase 1: Foundation ✅ COMPLETED

**Goal:** Establish project structure, specifications, and agent definitions

**Tasks:**
- [x] Define repository structure and layout rules
- [x] Document system responsibilities and boundaries
- [x] Define domain model and aggregate roots
- [x] Define persistence model
- [x] Define API contracts
- [x] Define AI pipeline architecture
- [x] Document expense lifecycle rules
- [x] Define execution workflows (end-to-end)
- [x] Create agent responsibility definitions
- [x] Document engineering guardrails

**Outcome:** Complete specification suite ready for agent-driven development

**Completed:** 2025-01-06

---

### Phase 2: Workflow Automation ✅ COMPLETED

**Goal:** Create Git workflow, task tracking, and conventions system

**Tasks:**
- [x] Define Git workflow and review protocol (specs/13)
- [x] Create execution plan structure (backlog/02)
- [x] Create task detail template (backlog/03)
- [x] Create technical conventions document (specs/14)
- [x] Update Master Orchestration Prompt with workflow rules
- [x] Update Lead Agent with execution plan responsibilities
- [x] Update Lead Agent with conventions management responsibilities
- [x] Update all specialized agents with convention references

**Outcome:** Complete workflow automation system ready for use

**Completed:** 2025-01-06

**Key Deliverables:**
- `specs/13_git_workflow_and_review_protocol.md`
- `backlog/02_execution_plan.md`
- `backlog/03_task_detail_template.md`
- `backlog/tasks/` directory
- `specs/14_technical_conventions.md`
- Updated agent definitions

---

### Phase 3: Validation & First Execution 🔄 IN PROGRESS

**Goal:** Validate the agent system works with a real task

**Tasks:**
- [x] Complete manual update to `agents/backend/backend_api_agent.md` (add conventions reference)
- [ ] Initialize Lead Agent conversation
- [ ] Execute first task (F01-T01: User aggregate via Domain Agent)
- [ ] Validate workflow:
  - [ ] Task detail file created correctly
  - [ ] Execution plan updated properly
  - [ ] Feature branch created with correct naming
  - [ ] Commits follow conventions (atomic, attributed, backlog-ref)
  - [ ] Build succeeds
  - [ ] Push completes successfully
- [ ] Human review of first output
- [ ] Merge to develop
- [ ] Validate Lead Agent updates execution plan post-merge

**Current Status:** Ready to start - Awaiting Lead Agent conversation

**Next Immediate Step:** Initialize Lead Agent conversation

---

### Phase 4: Iteration & Refinement 📝 PENDING

**Goal:** Adjust workflow based on real usage

**Tasks:**
- [ ] Review first execution results with human
- [ ] Identify pain points or inefficiencies
- [ ] Refine execution plan format if needed
- [ ] Adjust agent instructions if necessary
- [ ] Update technical conventions based on actual code
- [ ] Document lessons learned

**Prerequisites:** Phase 3 completion

---

### Phase 5: Scale & Parallelize 📝 PENDING

**Goal:** Execute multiple features in parallel

**Tasks:**
- [ ] Execute multiple sequential tasks (F01-T02, F01-T03)
- [ ] Validate dependency handling
- [ ] Test parallel execution (F02-T01 || F03-T01)
- [ ] Ensure no merge conflicts
- [ ] Validate consistency across parallel work

**Prerequisites:** Phase 4 completion

---

### Phase 6: Full MVP Development 📝 PENDING

**Goal:** Complete all backend features

**Tasks:**
- [ ] Execute all 8 features from backlog/01_mvp_backlog.md
- [ ] Maintain workflow discipline throughout
- [ ] Document all technical conventions as they emerge
- [ ] Ensure human review quality remains high

**Prerequisites:** Phase 5 completion

---

## Key Decisions Made

This section documents important decisions and their rationale. **Do not revisit these decisions without explicit human approval.**

### Decision 1: AI as Advisory, Backend as Authority

**Context:** How should AI services interact with the domain?

**Decision:** AI services propose data with confidence scores. Backend validates and decides final state.

**Rationale:**
- Preserves domain integrity
- Allows AI replacement without domain changes
- Makes system auditable and debuggable
- Prevents AI hallucination from corrupting data

**Impact:** Architecture, API contracts, AI pipeline design

**Reference:** `specs/01_system_responsibilities.md`

---

### Decision 2: One Commit Per Logical Change

**Context:** How granular should commits be?

**Decision:** Each agent must produce one commit per logical change, multiple commits per task if needed.

**Rationale:**
- Maximizes reviewability
- Clear git history
- Easy to bisect or revert
- Forces agents to think incrementally

**Alternative Considered:** One commit per task (rejected: too large, hard to review)

**Impact:** Git workflow, agent instructions

**Reference:** `specs/13_git_workflow_and_review_protocol.md`

---

### Decision 3: Lead Agent Never Writes Code

**Context:** Should the Lead Agent write code directly?

**Decision:** No. Lead Agent orchestrates only. All code is written by specialized agents.

**Rationale:**
- Clear separation of concerns
- Specialized agents stay focused
- Lead Agent can focus on coordination
- Prevents responsibility creep

**Impact:** Agent role definitions, workflow design

**Reference:** `agents/lead_agent.md`

---

### Decision 4: Execution Plan as Living Document

**Context:** How to track progress across sessions?

**Decision:** `backlog/02_execution_plan.md` is updated by Lead Agent after each merge. It is the single source of truth for MVP development status.

**Rationale:**
- Enables stateless Lead Agent (no memory needed)
- Human can see progress at any time
- Successor agents can pick up where predecessor left off
- Supports parallelization planning

**Impact:** Workflow design, Lead Agent responsibilities

**Reference:** `backlog/02_execution_plan.md`

---

### Decision 5: Technical Conventions as Emergent

**Context:** Should we define all conventions upfront?

**Decision:** No. Start with minimal conventions, document decisions as they emerge during development.

**Rationale:**
- Avoids premature optimization
- Conventions based on real code, not speculation
- Flexibility to adjust as we learn
- Reduces upfront overhead

**Alternative Considered:** Define everything upfront (rejected: too rigid, likely wrong)

**Impact:** `specs/14_technical_conventions.md` design

**Reference:** `specs/14_technical_conventions.md`

---

### Decision 6: Monorepo Strategy

**Context:** How to organize code repositories?

**Decision:** Single monorepo containing backend, AI service, mobile, specs, and agents.

**Rationale:**
- Single developer
- Strong coupling between components
- Easier context sharing for AI agents
- Reduced coordination overhead
- Shared specs directory

**Impact:** Repository layout, agent file access

**Reference:** `specs/00_repository_layout.md`

---

### Decision 7: Hybrid Convention Ownership

**Context:** Who documents technical conventions - Lead Agent or Human?

**Decision:** Hybrid approach: Lead Agent documents emergent decisions automatically, human validates during review.

**Rationale:**
- Captures decisions in real-time
- Reduces human overhead
- Maintains human oversight
- Balances automation and control

**Impact:** Lead Agent responsibilities, review process

**Reference:** `agents/lead_agent.md`, Section 8

---

## Current Focus

**Active Phase:** Phase 3 - Validation & First Execution

**Current Task:** Ready to initialize Lead Agent conversation

**What Needs to Happen:**
1. Initialize Lead Agent conversation
2. Request execution of F01-T01 (User aggregate)
3. Observe and validate entire workflow

---

## Next Steps for Successor Agent

If you are taking over, here's what to do:

### Immediate (Next Session)

1. **Verify manual update was completed**
   - Check `agents/backend/backend_api_agent.md`
   - Confirm it includes `specs/14_technical_conventions.md` reference

2. **Prepare for Lead Agent initialization**
   - Review `specs/12_master_orchestration_prompt.md`
   - Understand Lead Agent's expected behavior

3. **When human is ready to start:**
   - Guide human to start a NEW conversation with Lead Agent
   - Provide the Master Orchestration Prompt
   - Instruct Lead Agent to read `backlog/02_execution_plan.md`
   - Request execution of first task (F01-T01)

### After First Execution

4. **Participate in review**
   - Help human review the output
   - Validate workflow was followed correctly
   - Identify any issues or improvements

5. **Document lessons learned**
   - Update this document's "Key Decisions" if new insights emerge
   - Update `specs/14_technical_conventions.md` if patterns were established
   - Update agent definitions if behavior needs adjustment

### Ongoing

6. **Maintain meta-documentation**
   - Keep this document updated as phases complete
   - Update changelog after each session
   - Ensure successor agents have clear context

---

## Important File References

### Core Project Documentation

- `README.md` - High-level project overview and quick start guide
- `PROJECT_CONTEXT.md` - This document (meta-level progress tracking)

### Specifications (Single Source of Truth)

- `specs/00_repository_layout.md` - Repository structure rules
- `specs/01_system_responsibilities.md` - Component boundaries (Mobile, Backend, AI)
- `specs/02_constraints.md` - Technical constraints and stack decisions
- `specs/03_domain_model.md` - Domain entities, aggregates, value objects
- `specs/04_architecture.md` - High-level system architecture
- `specs/05_persistence_model.md` - Database schema and ORM rules
- `specs/06_api_and_contracts.md` - API endpoints and contracts
- `specs/07_ai_pipeline.md` - AI processing architecture
- `specs/08_expense_lifecycle.md` - Expense state machine (critical!)
- `specs/09_execution_workflows.md` - End-to-end workflows
- `specs/10_agent_responsibilities.md` - Original agent role definitions
- `specs/11_engineering_guardrails.md` - Non-negotiable engineering rules
- `specs/12_master_orchestration_prompt.md` - Lead Agent initialization prompt
- `specs/13_git_workflow_and_review_protocol.md` - Git workflow (mandatory)
- `specs/14_technical_conventions.md` - Emergent coding conventions

### Agent Definitions

- `agents/lead_agent.md` - Lead Agent (orchestrator) definition
- `agents/backend/backend_domain_agent.md` - Domain layer agent
- `agents/backend/backend_application_agent.md` - Application layer agent
- `agents/backend/backend_infrastructure_agent.md` - Infrastructure layer agent
- `agents/backend/backend_api_agent.md` - API layer agent
- `agents/backend/agents_orchestration_plan.md` - Multi-agent coordination rules

### Backlog & Tracking

- `backlog/01_mvp_backlog.md` - Original MVP feature backlog (static)
- `backlog/02_execution_plan.md` - **LIVE execution status** (updated by Lead Agent)
- `backlog/03_task_detail_template.md` - Template for task detail files
- `backlog/tasks/` - Directory for individual task detail files (created during execution)

### Code Structure (Currently Empty)

- `backend/` - .NET backend (will be populated by agents)
- `ai-service/` - Python AI service (future)
- `mobile/` - Mobile app (future)

---

## Conversation History / Changelog

### Session 1: 2025-01-06 (Foundation & Setup)

**Participants:**
- Human: Developer/Product Owner
- AI Agent: Claude (Anthropic) - Project Orchestrator role

**Major Accomplishments:**

1. **Established Project Context**
   - Reviewed all existing specifications
   - Confirmed understanding of agent-oriented development approach
   - Clarified role as meta-developer (not feature implementer)

2. **Designed Git Workflow System**
   - Created comprehensive Git workflow specification
   - Defined branch naming conventions
   - Established commit message format
   - Defined one-commit-per-logical-change rule
   - Created pull request workflow

3. **Built Task Tracking System**
   - Created execution plan document structure
   - Designed task detail template
   - Established status tracking (Completed, In Progress, Ready for Review, Blocked, Pending)
   - Defined Lead Agent responsibilities for plan maintenance

4. **Established Technical Conventions Framework**
   - Created living document for emergent conventions
   - Defined hybrid ownership model (Lead Agent documents, human validates)
   - Pre-populated common conventions (naming, structure, error handling, etc.)
   - Integrated with agent workflows

5. **Updated All Agent Definitions**
   - Added Git workflow responsibilities to Lead Agent
   - Added execution plan management to Lead Agent
   - Added technical conventions management to Lead Agent
   - Updated all specialized agents to reference conventions document
   - Note: Manual update required for API Agent due to file permissions

6. **Created Meta-Documentation**
   - Created this PROJECT_CONTEXT.md document
   - Established successor agent onboarding process
   - Documented key decisions with rationale
   - Defined clear next steps

**Key Decisions Made:**
- Commit granularity (one per logical change)
- Execution plan as living document
- Technical conventions as emergent
- Hybrid convention ownership
- Branch-per-task workflow

**Files Created:**
- `specs/13_git_workflow_and_review_protocol.md`
- `backlog/02_execution_plan.md`
- `backlog/03_task_detail_template.md`
- `specs/14_technical_conventions.md`
- `backlog/tasks/` directory
- `PROJECT_CONTEXT.md` (this document)

**Files Modified:**
- `specs/12_master_orchestration_prompt.md`
- `agents/lead_agent.md`
- `agents/backend/backend_domain_agent.md`
- `agents/backend/backend_application_agent.md`
- `agents/backend/backend_infrastructure_agent.md`

**Status at End of Session:**
- Phase 1: ✅ Complete
- Phase 2: ✅ Complete
- Phase 3: 🔄 Ready to start (manual update completed)

**Pending Actions:**
- Human to initialize Lead Agent in new conversation
- Execute first task (F01-T01) to validate system

---

### [Future Sessions Will Be Added Here]

Template for future entries:

```markdown
### Session N: YYYY-MM-DD (Session Title)

**Participants:**
- Human: [name/role]
- AI Agent: [name/system] - [role]

**Major Accomplishments:**
- [Bullet list]

**Key Decisions Made:**
- [Bullet list]

**Files Created/Modified:**
- [List]

**Status at End of Session:**
- [Phase updates]

**Pending Actions:**
- [List]
```

---

## Update Protocol

### When to Update This Document

Update `PROJECT_CONTEXT.md` when:

1. **A phase is completed**
   - Update roadmap section
   - Move phase from "In Progress" to "Completed"
   - Add completion date

2. **A major decision is made**
   - Add to "Key Decisions Made" section
   - Include decision, rationale, and impact

3. **New files are created that affect workflow**
   - Add to "Important File References" section
   - Include brief description

4. **A session ends**
   - Add entry to "Conversation History / Changelog"
   - Summarize accomplishments
   - List files modified

5. **Current focus changes**
   - Update "Current Focus" section
   - Update "Next Steps" section

### How to Update

1. **Read the entire document first** to understand current state

2. **Make your updates** in the appropriate sections

3. **Commit with proper message:**
   ```
   docs(meta): Update project context after [session/phase/milestone]
   
   - [Summary of changes]
   
   Agent: [Your-Name]
   ```

4. **Notify human of update** if significant changes were made

### What NOT to Update

- Do not change past decisions without explicit human approval
- Do not remove completed phases (keep history)
- Do not delete old changelog entries
- Do not change the structure without discussion

---

## Notes for Human

### How to Use This Document

**When starting a new session with an AI agent:**
1. Share this document
2. Ask agent to read and confirm understanding
3. Ask agent to identify current phase and next steps
4. Proceed with work

**During review:**
- Check if agent updated this document appropriately
- Validate decisions captured correctly
- Confirm roadmap reflects reality

**When onboarding a new agent:**
- Simply point them to this document
- They should have everything needed to continue

### Maintenance Expectations

**You (human) should:**
- Review updates made by agents
- Validate major decisions are captured
- Correct any misunderstandings
- Add human-perspective notes if needed

**You should NOT need to:**
- Manually track every change
- Duplicate information from other documents
- Maintain detailed session notes (agent does this)

---

## Status Summary (Quick Reference)

**Last Updated:** 2025-01-06

**Current Phase:** 3 - Validation & First Execution

**Overall Progress:** ~40% (2/5 phases complete)

**Blockers:** 1 minor (manual file update)

**Ready to Start MVP Development:** Almost (after Phase 3 validation)

**Next Major Milestone:** First successful task execution with Lead Agent

**Confidence Level:** High - system design is complete and coherent

---

**End of Document**

*This document is maintained by AI agents under human supervision.*
*Last session: 2025-01-06*
*Next review: After first Lead Agent execution*
