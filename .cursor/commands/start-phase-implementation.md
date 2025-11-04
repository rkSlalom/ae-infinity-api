# Start Phase Implementation

Please implement the next pending phase from the IMPLEMENTATION_PLAN.md following these steps:

## Pre-Implementation
1. Read `docs/IMPLEMENTATION_PLAN.md` and identify the next phase to implement
2. Read context files from `../ae-infinity-context/`:
   - PROJECT_SPEC.md
   - ARCHITECTURE.md
   - API_SPEC.md
   - DEVELOPMENT_GUIDE.md
3. Review `.cursorrules` to ensure compliance with architecture patterns

## Implementation Workflow
1. **Create TODO List** - Break down the phase into specific tasks based on deliverables
2. **Domain Layer First** - Create/update entities if needed
3. **Application Layer** - Implement Commands, Queries, Handlers, Validators, DTOs
4. **Infrastructure Layer** - Add entity configurations, update DbContext if needed
5. **API Layer** - Create/update controllers with endpoints
6. **Testing** - Create comprehensive test script and verify all endpoints

## Architecture Requirements
- ✅ Follow Clean Architecture (Domain → Application → Infrastructure → API)
- ✅ Use CQRS pattern (separate Commands and Queries)
- ✅ Add FluentValidation validators for all commands
- ✅ Use MediatR for all handlers
- ✅ Create DTOs for all responses
- ✅ Implement proper authorization checks
- ✅ Return correct HTTP status codes
- ✅ Add XML documentation for Swagger

## Deliverables
- All commands/queries implemented with handlers and validators
- All API endpoints created and documented
- Test script created (test-phase-{number}-{name}.sh)
- Test results documented (PHASE_{NUMBER}_TEST_RESULTS.md)
- IMPLEMENTATION_PLAN.md updated with completion status

## Completion Criteria
- ✅ All endpoints visible in Swagger
- ✅ All tests passing
- ✅ No build errors or warnings
- ✅ Architecture patterns followed
- ✅ Authorization working correctly
- ✅ Validation returning proper error messages

Please proceed with implementing the next phase systematically and thoroughly.

