---
name: /openspec-doctor
id: openspec-doctor
category: OpenSpec
description: Diagnose and repair OpenSpec setup across all repositories
---
<!-- OPENSPEC:START -->
**OpenSpec Doctor - Automated Setup Validation and Repair**

This command runs diagnostics on your OpenSpec cross-repository setup and can automatically fix common issues.

**What it checks:**
- ✅ Repository detection (API, UI, Context)
- ✅ Context repo structure (project.md, AGENTS.md, specs/, changes/)
- ✅ Symlinks in API and UI repos (correct targets and existence)
- ✅ .gitignore entries (exclude symlinks from version control)
- ✅ Slash commands (.cursor/commands/ in all repos)
- ✅ Directory structure integrity

**Steps:**
1. Run the OpenSpec Doctor script
2. Review diagnostics report
3. Ask user if they want to auto-fix issues
4. If yes, run with --fix flag
5. Show final health report

**Guardrails:**
- Run diagnostics first (without --fix) to show what would change
- Always ask before auto-fixing
- Provide clear explanation of each issue found
- Show summary with issue count and fix count

**Implementation:**

```bash
# 1. Navigate to context repo
cd @/openspec/doctor

# 2. Run diagnostics (no fixes)
./openspec-doctor.sh

# 3. If user approves, run with auto-fix
# Ask: "Would you like me to auto-fix these issues? (y/n)"
# If yes:
./openspec-doctor.sh --fix

# 4. Show final status
echo "OpenSpec health check complete!"
```

**Output Explanation:**
- Show each check result (✓ success, ⚠ warning, ✗ error)
- Explain what each issue means
- Provide context for why it matters
- Suggest manual fixes for issues that can't be auto-repaired

**Common Issues and Fixes:**
- **Missing symlinks**: Auto-creates with correct relative paths
- **Incorrect symlink targets**: Re-creates with correct paths
- **Missing directories**: Creates openspec/, changes/, specs/
- **Missing .gitignore entries**: Adds OpenSpec exclusions
- **Missing slash commands**: Copies from context repo

**After Running:**
- Verify all repos can access shared context
- Test a slash command in each repo
- Confirm symlinks resolve correctly

**Reference:** See `@/openspec/doctor/README.md` for detailed documentation.

<!-- OPENSPEC:END -->

