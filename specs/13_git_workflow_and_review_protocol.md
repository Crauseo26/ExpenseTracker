# Git Workflow & Review Protocol

## Purpose
This document defines the **mandatory Git workflow** for all AI agents working on this project. It ensures that:
- Work remains reviewable in small, atomic units
- Changes are traceable to specific agents and backlog items
- Human review is efficient and predictable
- The project maintains clean Git history

These rules are **non-negotiable** and apply to all agents, including the Lead Agent.

---

## Core Principle

> **Every task produces exactly one feature branch with atomic, reviewable commits.**

No agent may commit directly to `main` or `develop`.

---

## Branch Strategy

### Base Branches

- `main`: Production-ready code (protected)
- `develop`: Integration branch for completed features (protected)

### Feature Branches

All work must happen in feature branches.

**Naming Convention:**

```
feature/<backlog-id>-<short-description>

Examples:
- feature/F01-user-authentication
- feature/F02-expense-lifecycle
- feature/F03-T01-expense-aggregate
```

**Rules:**
- Branch name MUST include the backlog reference (Feature ID or Task ID)
- Branch name MUST use kebab-case
- Branch MUST be created from `develop`
- Branch name MUST be descriptive enough to understand scope

---

## Commit Structure

### Commit Granularity

Each agent MUST produce:
- **One commit per logical change**
- **Multiple commits per task if the task has multiple logical steps**

A "logical change" is defined as:
- A single, cohesive modification that can be understood independently
- A change that leaves the project in a buildable state
- A change that serves one clear purpose

Example for "Implement Expense Aggregate":
```
Commit 1: Add Expense entity with core properties
Commit 2: Add Money value object
Commit 3: Add ExpenseDate value object
Commit 4: Add state transition logic
```

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

Agent: <agent-name>
Backlog-Ref: <backlog-id>
```

**Type:**
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code refactoring
- `test`: Adding tests
- `docs`: Documentation changes
- `build`: Build system changes

**Example:**

```
feat(domain): Add Expense aggregate with lifecycle rules

Implements core Expense entity with:
- State management (PendingReview, Confirmed)
- Immutable month/year rule
- Soft delete support

Agent: Backend-Domain-Agent
Backlog-Ref: F02-T01
```

---

## Agent-Specific Commit Attribution

All commits MUST include agent attribution in the commit message body:

```
Agent: Backend-Domain-Agent
```

If using Git author field:
```bash
git config user.name "Backend-Domain-Agent"
git config user.email "agent-domain@financia.local"
```

---

## Workflow Steps (Lead Agent Responsibility)

### 1. Before Delegating a Task

The Lead Agent MUST:

1. Create feature branch from `develop`:
   ```bash
   git checkout develop
   git pull origin develop
   git checkout -b feature/<backlog-id>-<description>
   ```

2. Document the branch creation in task brief
3. Provide branch name to executing agent

---

### 2. During Task Execution

Each executing agent MUST:

1. Work only on the assigned feature branch.
2. Make atomic commits (one per logical change) with proper messages.
3. Ensure each commit leaves the project in a buildable state.
4. Run build and tests (if they exist) before committing.

**CRITICAL RULE:** All file changes related to a task (including code, documentation, and execution plan updates) MUST be committed to the task's designated feature branch. No commits should be made directly to `develop` during task execution.

---

### 3. After Task Completion

The Lead Agent MUST:

1. Verify all commits follow conventions
2. Verify project builds successfully
3. Verify tests pass (if applicable)
4. Push branch to remote:
   ```bash
   git push origin feature/<backlog-id>-<description>
   ```

5. Create Pull Request automatically (if tooling supports it)
6. If PR creation fails or is unsupported, notify human reviewer with branch name
7. Mark task as "Ready for Review" in backlog

---

## Build Verification Rules

Before pushing ANY branch, the Lead Agent MUST verify:

- ✅ Project builds without errors
- ✅ Existing tests pass (if tests exist)
- ✅ The working directory is clean (verified with `git status`)
- ⚠️ If no tests exist, push is allowed but should be noted
- ✅ No uncommitted changes remain
- ✅ Commit messages follow conventions

If build or existing tests fail, the task is **not complete**.

---

## Pull Request Creation (Optional)

If the Lead Agent has access to PR creation tooling:

**PR Title Format:**
```
[<backlog-id>] <feature-description>

Example:
[F02] Implement Expense Lifecycle
```

**PR Description Template:**
```
## Backlog Reference
<backlog-id>

## Changes
- <list of changes>

## Agent Attribution
- <list of agents involved>

## Verification
- [ ] Project builds successfully
- [ ] Existing tests pass
- [ ] Commits follow conventions
```

If PR creation is not possible, the Lead Agent should provide this information in a comment or notification.

---

## Human Review Checkpoints

After a feature branch is pushed, the human reviewer will:

1. Review commit history
2. Review code changes
3. Run builds and tests locally
4. Provide feedback or approval

**Definition of Done:**
- Code reviewed and approved
- Merged into `develop` via Pull Request
- Feature branch deleted (done by human reviewer after merge)

---

## Prohibited Actions

Agents MUST NOT:

- ❌ Commit directly to `main` or `develop`
- ❌ Commit any task-related changes (including documentation) outside of the designated feature branch
- ❌ Merge branches without human approval
- ❌ Delete feature branches (this is done by human after merge)
- ❌ Push unreviewed changes
- ❌ Create branches without backlog reference
- ❌ Squash commits without authorization
- ❌ Rewrite history of pushed branches

---

## Example Full Workflow

### Scenario: Implement Feature 02 (Expense Lifecycle)

**Lead Agent Actions:**

1. Read backlog item F02
2. Create branch:
   ```bash
   git checkout -b feature/F02-expense-lifecycle
   ```
3. Delegate Task F02-T01 to Domain Agent
4. Domain Agent commits changes (3 logical commits):
   ```
   feat(domain): Add Expense entity with core properties
   Agent: Backend-Domain-Agent
   Backlog-Ref: F02-T01
   
   feat(domain): Add Money value object
   Agent: Backend-Domain-Agent
   Backlog-Ref: F02-T01
   
   feat(domain): Add state transition logic to Expense
   Agent: Backend-Domain-Agent
   Backlog-Ref: F02-T01
   ```
5. Delegate Task F02-T02 to Application Agent
6. Application Agent commits changes (2 logical commits):
   ```
   feat(application): Add CreateExpense command
   Agent: Backend-Application-Agent
   Backlog-Ref: F02-T02
   
   feat(application): Add ConfirmExpense command
   Agent: Backend-Application-Agent
   Backlog-Ref: F02-T02
   ```
7. Verify build succeeds
8. Push branch:
   ```bash
   git push origin feature/F02-expense-lifecycle
   ```
9. Create PR (if possible) or notify human reviewer

**Human Reviewer Actions:**

1. Review commits in feature/F02-expense-lifecycle
2. Test locally
3. Approve and merge to develop
4. Delete feature branch from remote

---

## Conflict Resolution

If merge conflicts occur:

- Lead Agent MUST stop and request human intervention
- Agents MUST NOT attempt to resolve conflicts autonomously

---

## Summary

This workflow ensures:

- ✅ Small, reviewable units of work
- ✅ Clear traceability (agent + backlog item)
- ✅ Clean Git history
- ✅ Predictable review process
- ✅ No "big bang" changes
- ✅ One commit per logical change (optimal granularity)

All agents must treat this document as a **hard constraint**.
