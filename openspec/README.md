# OpenSpec - AE Infinity API

This directory contains OpenSpec configuration for the backend API.

## Structure

- `project.md` → Symlink to `../../ae-infinity-context/openspec/project.md` (shared)
- `AGENTS.md` → Symlink to `../../ae-infinity-context/openspec/AGENTS.md` (shared)
- `specs/` → Symlink to `../../ae-infinity-context/openspec/specs/` (read-only)
- `changes/` → Backend-specific implementation changes

## Usage

### List All Specs
```bash
openspec list --specs
```

### List Backend Changes
```bash
openspec list
```

### Create a Backend-Specific Change
```bash
# For implementation-only changes (no API contract changes)
mkdir -p openspec/changes/refactor-repository-pattern
cd openspec/changes/refactor-repository-pattern

# Create proposal, tasks, optional design
touch proposal.md tasks.md

# No spec deltas needed for implementation-only changes
```

### Validate Changes
```bash
openspec validate [change-id] --strict
```

### Archive After Deployment
```bash
# Use --skip-specs for backend-only changes
openspec archive [change-id] --skip-specs --yes
```

## When to Create Changes Here

**Use API repo changes for:**
- Backend refactoring (no API contract changes)
- Performance optimizations (backend only)
- Infrastructure improvements
- Testing improvements
- Code quality improvements

**Use context repo changes for:**
- API contract changes
- New endpoints or modifications
- Data model changes
- Anything affecting the frontend

See `../../ae-infinity-context/openspec/CROSS_REPO_SETUP.md` for details.
