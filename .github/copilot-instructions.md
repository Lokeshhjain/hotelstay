# GitHub Copilot Instructions for Hotel Stay Availability

This repository is the Hotel Stay Availability system, a production-ready hotel availability and reservation platform built as an EPAM case study that demonstrates modern software engineering practices and effective GitHub Copilot agent collaboration. These instructions are authoritative for all code generation, refactoring, documentation, and review assistance in this repository.

## 1. Repository Overview

- Project name: Hotel Stay Availability
- Purpose: Provide a reliable hotel availability and reservation experience with maintainable, secure, and testable software.
- Primary goals:
  - Deliver production-quality backend services
  - Provide a modern Angular client experience
  - Demonstrate clean architecture and disciplined engineering
  - Support Specification Driven Development and AI-assisted delivery
- Core quality attributes:
  - Reusability
  - Maintainability
  - Extensibility
  - Performance
  - Readability
  - Security
  - Testability

## 2. AI Development Philosophy

- Follow Specification Driven Development. Prefer changes that are grounded in documented requirements and domain intent.
- Think in terms of long-term maintainability, not short-term convenience.
- Favor explicit, understandable design over clever abstractions.
- Write or update tests before or alongside behavior changes whenever practical.
- Preserve consistency with the existing architecture and naming patterns.
- Prefer small, purposeful changes that are easy to review and extend.
- When requirements are ambiguous, make the safest assumption and document it clearly rather than guessing silently.

## 3. Project Folder Structure

Maintain a clear separation of concerns that reflects Clean Architecture.

- src/
  - Domain/ or HotelStay.Domain/
  - Application/ or HotelStay.Application/
  - Infrastructure/ or HotelStay.Infrastructure/
  - API/ or HotelStay.Api/
  - Client/ or HotelStay.Web/
- tests/
  - Unit/
  - Integration/
  - End-to-End/
- docs/
  - architecture/
  - api/
  - decisions/
- .github/
  - workflows/
  - prompts/
  - copilot-instructions.md

Keep new code in the appropriate layer and do not place infrastructure concerns into the domain layer.

## 4. C# Coding Standards

- Use .NET 8 features appropriately and consistently.
- Prefer modern, idiomatic C# and concise, readable implementations.
- Favor immutable data shapes where appropriate and practical.
- Use async/await for I/O-bound operations.
- Avoid unnecessary complexity, hidden side effects, or stateful singleton behavior.
- Keep methods focused and small.
- Prefer composition over inheritance.
- Use interfaces and abstractions only when they add clarity and testability.
- Avoid code duplication by extracting shared behavior into reusable abstractions.
- Prefer explicit over implicit behavior.
- Follow SOLID principles in all design decisions.

## 5. Naming Conventions

- Use PascalCase for classes, records, interfaces, enums, methods, and properties.
- Use camelCase for local variables, parameters, and private fields.
- Use meaningful and domain-driven names rather than technical placeholders.
- Use suffixes and prefixes consistently where they add clarity, such as Interface, Service, Handler, Validator, Repository, Request, Response, Command, Query, and DTO.
- Avoid abbreviations unless they are widely accepted in the domain.
- Prefer names that reveal intent and business meaning.

## 6. File Organization

- Keep each class, interface, or record in its own file unless there is a strong, documented reason to group them.
- Group related files by responsibility and layer.
- Keep files focused and avoid monolithic files that contain unrelated code.
- Organize feature-oriented code clearly so it is easy to navigate.
- Keep generated or vendor-managed files outside the main source of truth where appropriate.

## 7. Clean Architecture Guidelines

- Keep business rules in the domain layer.
- Keep use cases and orchestration in the application layer.
- Keep external concerns such as persistence, messaging, and framework integrations in the infrastructure layer.
- Respect dependency direction: inner layers must not depend on outer layers.
- Do not let the domain layer depend on ASP.NET Core, Angular, Entity Framework, or other infrastructure frameworks.
- Define abstractions in the application or domain layer and implement them in the infrastructure layer.
- Make the system easy to evolve by isolating implementation details behind contracts.

## 8. Minimal API Guidelines

- Keep API endpoints thin and focused on transport concerns.
- Use endpoint mapping and route grouping to keep the API organized.
- Use request and response models that are explicit and easy to validate.
- Return consistent and meaningful responses, including problem details for failures.
- Avoid embedding business logic directly in endpoint handlers.
- Prefer command and query handling patterns where appropriate.
- Keep API contracts clear, versionable, and documented.
- Handle authentication, authorization, and validation consistently at the edge.

## 9. Dependency Injection Guidelines

- Use dependency injection as the default mechanism for wiring services.
- Register services with clear lifetimes that match their behavior.
- Prefer constructor injection over service location or static access.
- Avoid creating new dependencies inside domain objects or business rules.
- Keep composition roots explicit and centralized.
- Ensure that abstractions and implementations are aligned and easy to substitute in tests.

## 10. Validation Standards

- Validate all incoming data at the boundary.
- Enforce domain invariants in the domain layer where appropriate.
- Reject invalid state early and clearly.
- Use expressive validation messages that help developers and API consumers understand the issue.
- Do not rely on silent defaults for invalid input.
- Ensure validation rules are test-covered and consistent across application layers.

## 11. Exception Handling

- Handle exceptions at the appropriate boundary rather than swallowing them silently.
- Preserve meaningful exception context and avoid exposing internal implementation details to clients unnecessarily.
- Prefer explicit failure handling over broad catch-all logic.
- Use domain-specific exceptions where they improve clarity.
- Ensure unexpected failures are logged and surfaced in a controlled way.
- Do not use exceptions for normal control flow.

## 12. Logging Standards

- Use structured logging throughout the application.
- Log business events, errors, and meaningful state transitions.
- Avoid logging secrets, tokens, passwords, or sensitive user data.
- Include correlation identifiers or request context when useful.
- Keep logs actionable and concise.
- Do not log excessively or use noisy debug statements in production paths.

## 13. Domain Layer Rules

- The domain layer contains business concepts, entities, value objects, domain rules, and invariants.
- Keep domain code free from infrastructure and transport concerns.
- Domain objects should express business intent clearly.
- Avoid leaking persistence or API-specific details into the domain model.
- Prefer rich domain behavior over anemic models.
- Make domain logic testable without external dependencies.

## 14. Application Layer Rules

- The application layer coordinates domain behavior and external dependencies.
- Keep use cases, commands, queries, and orchestration logic here.
- Define abstractions for infrastructure concerns when they are required by the application layer.
- Keep the application layer free from UI and HTTP concerns.
- Favor clear orchestration and small, cohesive use cases.

## 15. Infrastructure Layer Rules

- The infrastructure layer implements persistence, external integrations, messaging, and platform-specific concerns.
- Keep infrastructure implementations behind abstractions defined by the application or domain layers.
- Do not place business rules in infrastructure code.
- Make implementations replaceable and testable through contracts.
- Handle external system failures in a controlled and observable manner.

## 16. API Layer Rules

- Expose functionality through a clean, consistent API surface.
- Keep API handlers focused on request processing and response shaping.
- Use versioning, validation, and authentication patterns consistently.
- Ensure API contracts reflect domain behavior accurately.
- Do not bypass application-layer orchestration with direct data access from the API layer.
- Return clear and standardized error responses.

## 17. Project Business Rules

- Provider implementations must remain independent.
- Normalize provider responses before returning data.
- API responses must expose only application DTOs.
- Provider-specific models must remain in the Infrastructure layer.
- Validation must be centralized.
- Business logic must not exist inside Minimal API endpoints.

## 18. Angular Guidelines

- Keep Angular code modular, readable, and aligned with the application domain.
- Prefer well-structured components, services, and reusable shared modules.
- Maintain clear separation between presentation, state, and API integration concerns.
- Use reactive patterns and predictable data flow.
- Avoid unnecessary complexity in templates and component logic.
- Keep API consumption centralized and consistent.
- Ensure accessibility and responsive behavior are considered in UI changes.

## 19. Testing Standards

- Use xUnit for automated tests.
- Write tests that verify real behavior and observable outcomes.
- Favor deterministic, isolated tests with clear arrange-act-assert structure.
- Cover both successful and failure paths.
- Validate boundary conditions, invalid input handling, and domain invariants.
- Avoid brittle tests that depend on implementation details rather than behavior.
- Keep tests maintainable and focused on business intent.

## 20. Git Commit Convention

- Use clear, conventional commit messages.
- Preferred format: type(scope): subject
- Examples of acceptable types: feat, fix, refactor, docs, test, chore, perf, style, build, ci
- Keep commit subjects concise and descriptive.
- Include meaningful context when a change is non-trivial.

## 21. Documentation Standards

- Keep documentation accurate, current, and aligned with behavior.
- Update relevant documentation when requirements, contracts, or architecture change.
- Prefer concise, practical documentation over excessive prose.
- Use documentation to explain intent, constraints, and non-obvious decisions.
- Maintain README, specification, and architecture notes in a consistent and accessible manner.

## 22. AI Collaboration Guidelines

- Treat GitHub Copilot as a development partner, not as a substitute for product and architectural judgment.
- Generate changes that are consistent with repository standards and the stated business domain.
- Preserve existing conventions unless a deliberate refactoring is justified.
- When making larger changes, maintain a clear plan and avoid introducing unrelated churn.
- Prefer suggestions that improve maintainability and clarity over those that optimize for brevity alone.

## 23. Workspace Context Usage

- Use **#file** when working from a specific document such as `spec.md` or `docs/requirement-analysis.md`.
- Use **#codebase** to understand the existing solution before introducing new functionality.
- Use **#selection** when modifying existing code to minimize unrelated changes.
- Always prefer existing implementations over generating duplicate functionality.
- Review related files before creating new classes or interfaces.

## 24. Session Continuity

GitHub Copilot should maintain context throughout the Software Development Lifecycle.

Before generating code:

1. Read `docs/requirement-analysis.md`
2. Read `spec.md`
3. Follow `.github/copilot-instructions.md`
4. Preserve terminology.
5. Preserve business rules.
6. Reuse existing abstractions.
7. Avoid conflicting implementations.

## 25. AI Verification Checklist

Before completing any generation:

- Verify business requirements.
- Verify API contracts.
- Verify validation rules.
- Verify naming conventions.
- Verify dependency injection.
- Verify async patterns.
- Verify testability.
- Highlight assumptions instead of guessing.

## 26. Things GitHub Copilot Must Avoid

- Do not generate implementation code that bypasses architecture boundaries.
- Do not introduce hard-coded secrets, credentials, or insecure defaults.
- Do not add unused abstractions or unnecessary complexity.
- Do not create tightly coupled code that is difficult to test or extend.
- Do not ignore validation, error handling, or logging requirements.
- Do not break existing behavior without clear justification and test coverage.
- Do not invent requirements that are not supported by the specification or repository context.
- Do not degrade performance, readability, or security for the sake of convenience.

## 27. Definition of Done

A change is complete only when all of the following are true:

- The implementation satisfies the relevant requirement or acceptance criteria.
- The code is aligned with Clean Architecture and repository conventions.
- The relevant tests are added or updated and pass.
- Validation, logging, and exception handling are appropriate for the change.
- Documentation is updated when behavior or architecture changes.
- The change is readable, maintainable, secure, and testable.
- The change does not introduce unnecessary technical debt.
