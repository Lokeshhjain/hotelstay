# Reflection

If I had more time, I would improve the HotelStay case study in the following ways while still keeping the solution in-memory and offline.

## 1. Strengthen API Contracts

I would replace anonymous response objects and inline request records with dedicated DTO classes for:
- search requests
- search responses
- reservation requests
- reservation responses
- reservation lookup responses

That would make the API boundary clearer and more production-like.

## 2. Improve Reservation Flow Design

The current reservation flow works, but I would make it cleaner by storing the selected offer snapshot at the time of selection rather than re-searching with a temporary internal criteria.

That would:
- reduce hidden assumptions
- make reservations easier to reason about
- avoid coupling reservation creation to a search re-run

## 3. Centralize UI State Management

The Angular UI now uses a shared `HotelStateService` with `BehaviorSubject` observables to coordinate state across components.

With more time, I would continue to refine this by:
- strengthening the shared state APIs
- considering Angular signals for more declarative state handling
- potentially evolving toward a small component store pattern

That would make the UI easier to maintain and test.

## 4. Add More Integration Tests

I added unit tests for the core business rules, but I would also add API integration tests to verify:
- search endpoint behavior
- room type filtering
- reservation success and failure
- lookup behavior
- response status codes

That would improve confidence in the full application flow.

## 5. Improve Client Validation

The UI currently needs stronger form validation.

I would switch to reactive forms and add:
- required field validation
- date ordering validation
- destination-compatible document validation
- clearer inline error messages

## 6. Clean Up Validation Boundaries

I would make validation responsibilities more explicit:
- API layer: request shape and required parameters
- Application layer: orchestration and use-case validation
- Domain layer: business rules and policy decisions

That separation would make the code easier to evolve.

## 7. Add Mapper Classes

I would move provider normalization into dedicated mapper classes instead of keeping all mapping inside the service layer.

That would help if additional providers are added later.

## 8. Add Better Error Consistency

I would standardize error responses so every failure returns a consistent shape.

For example:
- error code
- user-friendly message
- validation details when relevant

## 9. Polish the In-Memory Design

Because the case study requires in-memory storage only, I would still improve the in-memory model by adding:
- reservation snapshots
- reference indexing
- reserved offer indexing
- clearer repository interfaces

That would keep the solution lightweight but more robust.

## 10. Improve Frontend Experience

With more time, I would make the UI feel more complete by adding:
- a proper confirmation panel
- selected-offer preview
- disabled states while requests are in flight
- empty/loading states
- accessibility improvements

## Final Takeaway

The solution is strong as a case study because it is:
- aligned with the spec
- offline
- deterministic
- layered
- testable

If I had more time, I would focus on making the boundaries cleaner, the UI state more robust, and the API contracts more explicit while still keeping the system fully in-memory.

## AI-Assisted Development

GitHub Copilot Agent was used as a structured development assistant rather than a source of final truth. The workflow relied on the repository's reusable prompt artifacts in `.github/prompts/` to guide each phase: analysis, design, modeling, implementation, testing, and review.

## Accepted AI Suggestions

- Use layered architecture and in-memory stub providers to keep the solution aligned with the offline, deterministic requirement.
- Add dedicated backend and frontend tests for search, reservation validation, and lookup flows.
- Update `src/HotelStay.Api/Program.cs` to support `WebApplicationFactory`-based integration testing.
- Improve prompt documentation in `prompts.md` so the repository contains a clear record of prompt artifact usage.
- Preserve provider normalization boundaries and avoid leaking provider-specific JSON details into the shared hotel offer model.

These suggestions were accepted because they directly matched the approved specification and the requirement analysis, and they were validated by confirming the resulting behavior against `spec.md` and the documented acceptance criteria.

## Rejected or Corrected AI Suggestions

- Some early test guidance produced overly strict frontend expectations for exact object equality and fixed dates. These were corrected to use dynamically generated future dates and more resilient validation of returned offer fields.
- Design ideas that implied adding authentication, payment processing, or external provider access were rejected because they fell outside the documented scope and constraints.
- UI state recommendations that would require a large refactor of the current component coordination were deferred to future work, while the current `HotelStateService` shared state approach keeps the implementation cohesive.

Those corrections were made to preserve the case study's current scope and to avoid introducing speculative functionality not supported by the requirement analysis.

## Validation Against the Specification

AI-generated changes were accepted only after cross-checking them against `spec.md` and `docs/requirement-analysis.md`. Examples include:
- Verifying that reservation confirmation output contains reference number, provider name, total price, and cancellation policy.
- Confirming that invalid document submission returns HTTP 422 and a meaningful validation message.
- Ensuring search results remain filtered by destination, dates, and optional room type.

This validation process helped keep each change aligned with the documented business rules and acceptance criteria.

## GitHub Copilot Agent Limitations

The agent provided useful architectural direction, but it required explicit grounding in the repository's existing files and prompt artifacts. It sometimes suggested broad improvements that needed to be constrained by the current offline-only scope.

The agent also did not replace the need for manual validation of the actual code changes, especially for newly added tests and API behavior.

## Reusable Prompt Files and Consistency

The reusable prompt files in `.github/prompts/` improved consistency across the SDLC by establishing a stable, phase-specific guide for each activity. They made it easier to keep terminology aligned, preserve the same quality expectations, and reduce drift between analysis, design, implementation, and testing.

## Lessons Learned and Future Improvements

- Keeping prompt documentation up to date is important; the new `prompts.md` entry now captures prompt intent and execution history.
- Reusable prompt artifacts are valuable for maintaining consistent guidance across multiple development phases.
- The case study would benefit from a stronger separation of client validation, application validation, and domain rules.
- A future iteration should convert the UI to reactive forms and continue refining the shared state service or a signal-based pattern.
- More API integration tests would increase confidence in the end-to-end flow while preserving the offline case study constraints.
