# AI Prompts Used

This file captures the main prompts used while building the HotelStay case study, along with the implementation decisions that followed from them.

## 1. Case Study Understanding Prompt

Prompt intent:
- Read the HotelStay specification and summarize the product, architecture, and workflow.

Decision notes:
- I used this to align the implementation with the case study scope before writing code.
- It helped confirm the solution should stay offline and use deterministic stub data.
- It also clarified that the system should stay within a layered architecture instead of introducing unnecessary complexity.

## 2. Backend Test Implementation Prompt

Prompt intent:
- Create a new xUnit test project under `tests` and implement backend unit tests.

Decision notes:
- I added a dedicated `HotelStay.UnitTests` project rather than placing tests inside the API project.
- I referenced the Application, Domain, and Infrastructure projects so the tests could exercise real application behavior.
- I focused on core business rules:
  - search validation
  - document validation
  - offer normalization
  - reservation creation
  - reservation lookup
  - in-memory store behavior

## 3. UI Implementation Prompt

Prompt intent:
- Improve the UI look and feel using Bootstrap.

Decision notes:
- I added Bootstrap to the Angular app and updated the shell layout first.
- I kept the existing component structure instead of rewriting the UI from scratch.
- I used a consistent card-based presentation across search, results, reservation, and lookup sections.
- I kept the UI offline-friendly and avoided introducing extra state libraries because the case study scope did not require them.

## 4. Search Filter Fix Prompt

Prompt intent:
- Fix the search API optional filter so room type actually affects results.

Decision notes:
- I added explicit room type parsing and validation so invalid filter values no longer pass silently.
- I applied the filter in the application layer, which kept provider normalization separate from business selection logic.
- I added tests to lock in the behavior for valid, invalid, and filtered searches.

## 5. Review and Score Prompt

Prompt intent:
- Evaluate the completed HotelStay solution for code quality, architecture, AI usage, prompting skills, and production readiness.

Decision notes:
- The review highlighted that the solution is strong as a case study but not yet production-ready.
- The biggest recommended improvements were:
  - explicit API DTOs
  - stronger validation boundaries
  - cleaner UI state handling
  - integration tests
  - better reservation flow design

## Implementation Principles Followed

- Keep the solution offline and deterministic.
- Use in-memory storage only.
- Preserve the layered architecture from the specification.
- Keep provider-specific behavior isolated.
- Normalize provider results before returning them to the UI.
- Add tests whenever business logic changed.
- Prefer small targeted changes over speculative overengineering.

## Notes on AI Usage

- AI was used as a coding and analysis assistant, not as a substitute for design decisions.
- I used the prompts to guide implementation step by step, then verified the output through tests and build checks.
- The final solution was shaped by human review decisions about scope, architecture, and maintainability.
