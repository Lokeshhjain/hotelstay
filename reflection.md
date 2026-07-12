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

## 3. Replace Window-Based UI Communication

The Angular UI currently uses custom browser events to share state between components.

With more time, I would replace that with:
- a shared service
- Angular signals
- or a small component store pattern

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
