#!/bin/bash
# Setup OpenSpec for ae-infinity-api repo

set -e

echo "🔧 Setting up OpenSpec for ae-infinity-api..."

# Create directory structure
echo "📁 Creating openspec directory structure..."
mkdir -p openspec/changes
mkdir -p openspec/changes/archive

# Create symlinks to shared files
echo "🔗 Creating symlinks to context repo..."
cd openspec

# Remove existing files/links if they exist
rm -f project.md AGENTS.md specs

# Create symlinks
ln -s ../../ae-infinity-context/openspec/project.md project.md
ln -s ../../ae-infinity-context/openspec/AGENTS.md AGENTS.md
ln -s ../../ae-infinity-context/openspec/specs specs

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
EOF

# Configure git
echo "⚙️  Configuring git..."
git config --local openspec.context-repo "../ae-infinity-context"

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

