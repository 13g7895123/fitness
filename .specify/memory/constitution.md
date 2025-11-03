<!--
Sync Impact Report - Constitution v1.1.0
========================================
Version Change: 1.0.0 → 1.1.0
Modified Principles:
  - Principle III (User Experience Consistency): Added Traditional Chinese (zh-TW) language requirement
Added Sections:
  - Documentation Standards (new section specifying Traditional Chinese requirement)
Removed Sections: N/A
Templates Requiring Updates:
  ✅ .specify/templates/plan-template.md - Updated Constitution Check with language requirement
  ✅ .specify/templates/spec-template.md - Aligned with zh-TW documentation standard
  ✅ .specify/templates/tasks-template.md - Updated with language validation tasks
Follow-up TODOs: None
Previous Report (v1.0.0):
  - Initial creation with 4 core principles
  - Established code quality, testing, UX, and performance standards
-->

# Fitness Application Constitution

## Core Principles

### I. Code Quality (NON-NEGOTIABLE)

All code MUST meet measurable quality standards before merge:

- **Type Safety**: Strong typing required; no implicit `any` types without explicit justification
- **Code Coverage**: Minimum 80% test coverage for all new code; critical paths require 95%
- **Complexity Limits**: Cyclomatic complexity MUST NOT exceed 10 per function; 15 per module
- **Documentation**: All public APIs MUST have JSDoc/TSDoc comments; complex algorithms require inline explanation
- **Linting**: Zero linting errors allowed; warnings must be addressed or explicitly suppressed with justification
- **Naming Conventions**: Consistent naming (camelCase for variables/functions, PascalCase for classes/components)

**Rationale**: Poor code quality leads to technical debt that compounds over time, making the fitness app unmaintainable and unreliable for users tracking critical health data.

### II. Testing Standards (NON-NEGOTIABLE)

Test-Driven Development (TDD) is mandatory for all features:

- **Red-Green-Refactor**: Tests written first → User approval → Tests fail → Implementation → Tests pass
- **Test Pyramid**: Unit tests (70%) > Integration tests (20%) > E2E tests (10%)
- **Contract Testing**: All API endpoints MUST have contract tests verifying request/response schemas
- **Integration Testing**: Required for: external API calls, database operations, authentication flows, workout/nutrition tracking logic
- **Performance Testing**: Critical user journeys (workout logging, data sync) MUST have performance regression tests
- **Accessibility Testing**: All UI components MUST pass WCAG 2.1 AA automated tests

**Rationale**: Fitness tracking requires data accuracy and reliability. Users depend on consistent behavior for health decisions. Bugs in workout tracking or nutrition calculations can have real health consequences.

### III. User Experience Consistency

User-facing features MUST maintain consistent design and interaction patterns:

- **Design System**: All UI components MUST use the centralized design system; no one-off styles
- **Accessibility**: WCAG 2.1 AA compliance mandatory; keyboard navigation, screen reader support, color contrast ratios ≥4.5:1
- **Responsive Design**: Support for mobile (320px+), tablet (768px+), desktop (1024px+) viewports
- **Loading States**: All async operations MUST show loading indicators; skeleton screens for data-heavy views
- **Error Handling**: User-friendly error messages; no raw stack traces; actionable recovery steps provided
- **Offline Support**: Core features (workout logging, viewing history) MUST work offline with sync on reconnection
- **Internationalization**: All user-facing text MUST be externalized for i18n; date/number formatting locale-aware
- **Language**: Primary language is Traditional Chinese (zh-TW); all UI text, error messages, and notifications in zh-TW

**Rationale**: Fitness apps are used daily in varied contexts (gym, outdoors, low connectivity). Consistent, accessible UX ensures users can reliably track workouts regardless of environment or ability. Traditional Chinese ensures the app serves its target user base effectively.

### IV. Performance Requirements

Application performance MUST meet defined thresholds for user satisfaction:

- **Page Load**: Initial page load <2 seconds on 3G; <1 second on broadband
- **Interaction Response**: Button clicks/taps respond within 100ms; form submissions within 200ms
- **Data Sync**: Background sync completes within 5 seconds for typical workout data; progress indicator for larger syncs
- **API Response Times**: p95 latency <500ms for read operations; <1000ms for write operations
- **Memory Usage**: Mobile app memory footprint <150MB; no memory leaks (heap growth <10% over 1-hour session)
- **Battery Impact**: Background operations MUST use efficient scheduling; no continuous polling (use push notifications)
- **Database Queries**: All queries optimized; no N+1 queries; appropriate indexes for common access patterns

**Rationale**: Users interact with fitness apps multiple times daily, often with time constraints. Slow performance disrupts workout flow and reduces engagement, ultimately harming health outcomes.

## Documentation Standards

### Language Requirements (NON-NEGOTIABLE)

All project documentation and user-facing content MUST be in Traditional Chinese (zh-TW):

- **Specifications**: Feature specs (`spec.md`) MUST be written in Traditional Chinese
- **Implementation Plans**: All plans (`plan.md`) MUST be written in Traditional Chinese
- **User Stories**: Acceptance scenarios and user journeys in Traditional Chinese
- **Task Descriptions**: Task lists (`tasks.md`) MUST be written in Traditional Chinese
- **Checklists**: Quality checklists MUST be written in Traditional Chinese
- **API Documentation**: Endpoint descriptions, parameter names (where user-facing), error messages in Traditional Chinese
- **README Files**: Project README, quickstart guides, and all `docs/` content in Traditional Chinese
- **Code Comments**: Public API comments and user-facing logic comments in Traditional Chinese
- **Commit Messages**: Git commit messages SHOULD be in Traditional Chinese for consistency
- **UI Text**: All interface labels, buttons, placeholders, tooltips, notifications in Traditional Chinese

### Exceptions

The following MAY remain in English:

- **Code Identifiers**: Variable names, function names, class names (follow language conventions)
- **Technical Terms**: Industry-standard terms without clear zh-TW equivalents (e.g., "API", "HTTP", "REST")
- **Configuration Files**: Environment variables, config keys (standard naming conventions)
- **Log Keys**: Structured logging field names for parsing compatibility
- **Third-Party Integration**: External service references, URLs, technical identifiers

**Rationale**: Traditional Chinese documentation ensures all stakeholders (product owners, developers, QA, users) can effectively collaborate without language barriers, reducing miscommunication and improving product quality for the target market.

## Quality Standards

### Code Review Requirements

All pull requests MUST pass these gates:

1. **Automated Checks**: CI pipeline green (tests, linting, type checking, coverage threshold)
2. **Constitution Compliance**: Reviewer verifies adherence to all four core principles
3. **Manual Testing**: Feature demonstrated working in dev environment
4. **Documentation Updates**: README, API docs, and changelog updated as needed
5. **Complexity Justification**: Any principle violations documented in Complexity Tracking section with rationale

### Metrics and Monitoring

Production deployment MUST include:

- **Error Tracking**: Sentry or equivalent for client and server errors; alerts for error rate spikes
- **Performance Monitoring**: Real User Monitoring (RUM) for Core Web Vitals; APM for backend services
- **Usage Analytics**: Track user journey completion rates; identify drop-off points
- **Health Metrics**: API latency, error rates, database query performance
- **Alert Thresholds**: Error rate >1%, p95 latency exceeds principle thresholds by 50%

## Development Workflow

### Feature Development Process

1. **Specification**: Use `/speckit.specify` to create technology-agnostic feature spec with user stories (in Traditional Chinese)
2. **Checklist Generation**: Use `/speckit.checklist` to create quality checklists (UX, testing, performance, security) (in Traditional Chinese)
3. **Planning**: Use `/speckit.plan` to generate implementation plan with constitution check (in Traditional Chinese)
4. **Task Breakdown**: Use `/speckit.tasks` to create user story-based task list (in Traditional Chinese)
5. **Checklist Validation**: Use `/speckit.implement` to verify all checklists complete before coding
6. **Implementation**: Complete tasks following TDD; commit per task or logical group
7. **Review**: PR review verifies constitution compliance including language requirements
8. **Deployment**: Staging validation before production release

### Branching and Versioning

- **Semantic Versioning**: MAJOR.MINOR.PATCH format
  - MAJOR: Breaking API changes, database migrations requiring downtime
  - MINOR: New features, backward-compatible API additions
  - PATCH: Bug fixes, performance improvements, documentation
- **Branch Naming**: `###-feature-name` format where ### is feature number
- **Protected Branches**: `main` requires PR review and passing CI; `develop` for integration

## Governance

### Amendment Process

Constitution changes require:

1. **Proposal**: Document proposed changes with rationale in PR
2. **Impact Analysis**: Identify affected templates, checklists, and existing features
3. **Team Review**: All core maintainers must approve
4. **Migration Plan**: If changes affect existing code, provide migration guide
5. **Version Bump**: Follow semantic versioning for constitution itself (current: 1.0.0)
6. **Template Updates**: Update all affected files in `.specify/templates/` before merge

### Compliance and Enforcement

- **All pull requests** MUST pass constitution check in plan.md
- **Language compliance** verified during PR review; all documentation and user-facing text in Traditional Chinese
- **Complexity violations** MUST be documented in Complexity Tracking table with justification
- **Principle deviations** require explicit team discussion and documented decision
- **Regular audits**: Quarterly review of codebase compliance; refactoring tickets for violations
- **Runtime guidance**: Use `.specify/templates/agent-file-template.md` for AI-assisted development context

### Review Cycle

- **Quarterly Review**: Assess principle effectiveness; propose amendments if needed
- **Post-Incident**: Review constitution relevance after production incidents
- **Major Features**: Validate principles still serve project needs after significant features

**Version**: 1.1.0 | **Ratified**: 2025-11-02 | **Last Amended**: 2025-11-02
