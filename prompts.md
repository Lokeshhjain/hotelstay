# AI Prompts Used

This file captures the main prompts and prompt artifacts used while building the HotelStay case study, along with the implementation decisions and review work that followed from them.

## AI Model

- Repository prompt artifacts are written for GitHub Copilot Agent Mode.
- Current assistant runtime in this workspace is Raptor mini.
- Usage intent: analysis, design, implementation, testing, and review assistance for the HotelStay case study.

## Prompt Files and Usage

The repository contains the following prompt artifacts under `.github/prompts/`:

- `analysis.prompt.md`
  - Used to translate requirements into a structured business analysis artifact.
- `design.prompt.md`
  - Used to define architecture boundaries, component responsibilities, API contracts, and workflow sequencing.
- `modeling.prompt.md`
  - Used to clarify domain concepts, request/response contracts, validation boundaries, and workflow abstractions.
- `implementation.prompt.md`
  - Used to implement the approved solution in the target stack, including backend, frontend, and integration points.
- `testing.prompt.md`
  - Used to add and refine backend and frontend tests, covering happy paths, failure cases, and validation scenarios.
- `review.prompt.md`
  - Used to review the solution for quality, architecture compliance, documentation completeness, and production readiness.

## Prompt Execution Summary

- `analysis.prompt.md` guided the requirement analysis and helped capture the business scope, functional requirements, and validation rules.
- `design.prompt.md` guided architecture and API contract design, including feature separation and provider normalization expectations.
- `modeling.prompt.md` helped define the core domain model concepts, data contracts, and workflow boundaries without embedding implementation detail.
- `implementation.prompt.md` guided actual code changes across the backend and Angular frontend while preserving the layered architecture.
- `testing.prompt.md` drove the creation and improvement of unit tests, integration tests, and component tests.
- `review.prompt.md` was used to validate the final solution, identify last-pass fixes, and update documentation such as this prompt log.

## Prompt Catalog

### `analysis.prompt.md`

Prompt intent:
- Translate a business requirement into a structured requirement analysis artifact that can be handed off to design.

### `design.prompt.md`

Prompt intent:
- Produce design and architecture guidance, including component responsibilities, API contracts, and interaction flows.

### `modeling.prompt.md`

Prompt intent:
- Define domain concepts, request/response models, validation rules, and workflow abstractions without introducing implementation detail.

### `implementation.prompt.md`

Prompt intent:
- Implement the approved solution in a production-ready manner for the target technology stack with a focus on maintainability and testability.

### `testing.prompt.md`

Prompt intent:
- Create comprehensive tests for backend and frontend behavior, including success paths, negative paths, and edge cases.

### `review.prompt.md`

Prompt intent:
- Evaluate the final solution for architecture compliance, code quality, security, and documentation readiness.

## Implementation Principles Followed

- Keep the solution offline and deterministic.
- Use in-memory storage only.
- Preserve the layered architecture implied by the case study.
- Keep provider-specific behavior isolated behind normalization boundaries.
- Normalize provider results before returning them to the UI.
- Add tests whenever business logic changed.
- Prefer small targeted changes over speculative overengineering.
- Keep the API entry point thin and organize endpoints by feature.
- Preserve explicit mapping layers and validation boundaries.

## Notes on AI Usage

- AI was used as an assistant for analysis, implementation, testing, and review.
- Prompt artifacts helped steer the workflow across requirement analysis, architecture design, implementation, and quality validation.
- The final solution was shaped by human review decisions about scope, architecture, and maintainability.
