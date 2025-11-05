#!/usr/bin/env bash
# Setup OpenSpec for ae-infinity-api repo
# Platform: Cross-platform (Linux, macOS, Windows with Git Bash)
#
# Environment Variables:
#   CONTEXT_REPO_PATH - Override path to ae-infinity-context repo
#                       (default: auto-detect from parent directory)

set -e

# Auto-detect context repo location
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PARENT_DIR="$(dirname "$SCRIPT_DIR")"
DEFAULT_CONTEXT_PATH="$PARENT_DIR/ae-infinity-context"
CONTEXT_REPO_PATH="${CONTEXT_REPO_PATH:-$DEFAULT_CONTEXT_PATH}"

# Verify context repo exists
if [ ! -d "$CONTEXT_REPO_PATH" ]; then
    echo "❌ Error: Context repository not found at: $CONTEXT_REPO_PATH"
    echo ""
    echo "Expected structure:"
    echo "  workspace/"
    echo "  ├── ae-infinity-context/"
    echo "  └── ae-infinity-api/ (current)"
    echo ""
    echo "You can override with: CONTEXT_REPO_PATH=/path/to/context ./setup-openspec.sh"
    exit 1
fi

echo "🔧 Setting up OpenSpec for ae-infinity-api..."
echo "   Context repo: $CONTEXT_REPO_PATH"

# Create directory structure
echo "📁 Creating openspec directory structure..."
mkdir -p openspec/changes
mkdir -p openspec/changes/archive

# Create symlinks to shared files
echo "🔗 Creating symlinks to context repo..."
cd openspec

# Remove existing files/links if they exist
rm -f project.md AGENTS.md specs

# Calculate relative path from openspec directory to context repo
# This ensures symlinks work correctly regardless of workspace structure
OPENSPEC_DIR="$SCRIPT_DIR/openspec"
REL_PATH=$(realpath --relative-to="$OPENSPEC_DIR" "$CONTEXT_REPO_PATH/openspec" 2>/dev/null || \
           python3 -c "import os.path; print(os.path.relpath('$CONTEXT_REPO_PATH/openspec', '$OPENSPEC_DIR'))" 2>/dev/null || \
           echo "../../ae-infinity-context/openspec")

# Create symlinks using detected path
ln -s "$REL_PATH/project.md" project.md
ln -s "$REL_PATH/AGENTS.md" AGENTS.md
ln -s "$REL_PATH/specs" specs

cd ..

# Update .gitignore
echo "📝 Updating .gitignore..."
if ! grep -q "# OpenSpec symlinks" .gitignore 2>/dev/null; then
    cat >> .gitignore << 'EOF'

# OpenSpec symlinks (point to context repo)
openspec/project.md
openspec/AGENTS.md
openspec/specs
EOF
fi

# Create README for openspec directory
cat > openspec/README.md << 'EOF'
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
# For API-only changes (no API contract changes)
mkdir -p openspec/changes/optimize-query-performance
cd openspec/changes/optimize-query-performance

# Create proposal, tasks, optional design
touch proposal.md tasks.md design.md

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
- Database query optimizations
- Internal refactoring (no API contract changes)
- Performance improvements
- Infrastructure changes
- Logging and monitoring
- Dependency updates

**Use context repo changes for:**
- API contract changes (new/modified endpoints)
- Authentication/authorization changes
- Data model changes affecting API responses
- New features requiring frontend integration

See `../../ae-infinity-context/openspec/CROSS_REPO_SETUP.md` for details.
EOF

# Configure git
echo "⚙️  Configuring git..."
# Calculate relative path from API repo to context repo for git config
REL_CONTEXT_PATH=$(realpath --relative-to="$SCRIPT_DIR" "$CONTEXT_REPO_PATH" 2>/dev/null || \
                   python3 -c "import os.path; print(os.path.relpath('$CONTEXT_REPO_PATH', '$SCRIPT_DIR'))" 2>/dev/null || \
                   echo "../ae-infinity-context")
git config --local openspec.context-repo "$REL_CONTEXT_PATH"

echo ""
echo "✅ OpenSpec setup complete!"
echo ""
echo "📋 Verify setup:"
echo "   ls -la openspec/"
echo ""
echo "🧪 Test commands:"
echo "   openspec list --specs    # Should show specs from context repo"
echo "   openspec list            # Should show API changes (empty for now)"
echo ""
echo "📖 Read more: openspec/README.md"
echo ""

