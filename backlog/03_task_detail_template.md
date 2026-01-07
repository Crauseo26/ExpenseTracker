# Task Detail Template

## Purpose
This template is used by the Lead Agent to create detailed task specifications when decomposing features into executable units of work.

Each task gets its own file in `backlog/tasks/` following this structure.

---

## Template Structure

```markdown
# Task: [TASK-ID] — [Task Title]

## Feature Reference
Feature [ID] — [Feature Name]

## Objective
[Clear description of what needs to be accomplished]

## Agent Assignment
[Agent-Name] (e.g., Backend-Domain-Agent)

## Branch
feature/[TASK-ID]-[short-description]

## Authoritative Inputs
- [List of spec documents the agent must follow]
- specs/XX_document.md
- specs/YY_document.md

## Expected Outputs
- [List of files/components to be created or modified]
- File1.cs: [description]
- File2.cs: [description]

## Acceptance Criteria
- [ ] [Criterion 1]
- [ ] [Criterion 2]
- [ ] Project builds successfully
- [ ] Commits follow Git workflow conventions
- [ ] [Any specific domain rules enforced]

## Dependencies
- [List of tasks that must be completed before this one]
- Depends on: [TASK-ID]
- Requires: [specific entity/component]

## Technical Constraints
- [Any specific technical limitations or requirements]
- Must use: [specific technology/pattern]
- Must not: [prohibited actions]

## Status
⏳ Pending | 🔄 In Progress | 📋 Ready for Review | ✅ Completed | ⛔ Blocked

## Execution Log

### [Date] - [Time]
**Action**: [What happened]
**Agent**: [Which agent]
**Outcome**: [Result]

### [Date] - [Time]
**Action**: Task created
**Agent**: Lead-Agent
**Outcome**: Task file generated, ready for execution

## Notes
[Lead Agent or Human can add notes here during execution]

---

## Review Checklist (For Human Reviewer)

After branch is pushed:
- [ ] Code follows conventions
- [ ] All acceptance criteria met
- [ ] Build succeeds
- [ ] Tests pass (if applicable)
- [ ] Commits are atomic and well-documented
- [ ] No architectural violations

## Post-Merge Actions

After human merges to `develop`:
- [ ] Update execution plan (Lead Agent)
- [ ] Archive this task file (Lead Agent)
- [ ] Trigger next dependent task (Lead Agent)
```

---

## Usage Instructions

### For Lead Agent

1. **When decomposing a feature:**
   - Read feature from `backlog/01_mvp_backlog.md`
   - Create task file: `backlog/tasks/[TASK-ID]-[description].md`
   - Fill in all sections completely
   - Ensure dependencies are clear

2. **During execution:**
   - Update status field
   - Add entries to Execution Log
   - Add notes as needed

3. **After completion:**
   - Update status to ✅
   - Move to completed section in execution plan
   - Archive task file (optional: move to `backlog/tasks/completed/`)

### For Human Reviewer

1. **When reviewing:**
   - Open task file to see original acceptance criteria
   - Use Review Checklist to validate work
   - Provide feedback in task file if needed

2. **After merge:**
   - Confirm merge completion to Lead Agent
   - Lead Agent will handle task file cleanup

---

## Example Task File

See `backlog/tasks/F01-T01-user-aggregate.md` (to be created by Lead Agent when starting MVP execution)

