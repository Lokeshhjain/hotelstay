# Specification

## 1. Overview

This specification refines the approved requirement analysis for the SkyRoute Hotel Availability feature and is intended to guide implementation planning for the backend, frontend, and test layers. It remains strictly aligned to the approved business requirements and should be read together with the requirement analysis and the supporting sequence diagrams.

## 2. Design Goals

- Deliver a consistent traveller experience across multiple providers.
- Keep provider-specific behavior isolated from the core business flow.
- Support future provider expansion without changing the core user journey.
- Ensure the solution runs fully offline using deterministic local stubs.
- Provide clear validation behavior across the client, API, and application layers.
- Preserve the approved business rules and scope without introducing unsupported requirements.
- Ensure reservation confirmation details are returned in a structured, traveller-friendly form.
- Keep search results aligned to the traveller’s selected destination, stay dates, and optional room type.
- Keep the API composition root thin and organize endpoints by feature rather than embedding all workflow logic in the startup entry point.
- Preserve explicit normalization and mapping layers so provider-specific transformations remain testable and maintainable.
- Capture a stable reservation snapshot at booking time so confirmation and retrieval remain independent of subsequent catalog re-querying.

## 3. Solution Architecture

### 3.1 Architectural Style

The solution will follow a layered architecture with clear separation between:

- Client experience
- API boundary
- Application orchestration
- Domain rules
- Infrastructure adapters and stubs

This structure is intended to keep the business flow understandable, testable, and extensible while isolating provider-specific behavior.

### 3.2 Proposed Project Structure

#### Backend

- src/HotelStay.Api
  - Endpoints and HTTP request handling
  - Feature-based endpoint grouping and request validation
  - Response shaping and error translation
- src/HotelStay.Application
  - Search orchestration
  - Reservation orchestration
  - Validation coordination
  - Provider coordination
  - Offer normalization and reservation snapshot orchestration
- src/HotelStay.Domain
  - Core business concepts
  - Domain rules and invariants
  - Enums and value objects
- src/HotelStay.Infrastructure
  - Provider adapters for PremierStays and BudgetNests
  - Deterministic stub implementations
  - Dedicated provider mappers and normalization logic
  - Concurrency-safe in-memory reservation storage for the current scope

#### Frontend

- src/HotelStay.UI
  - Search screen
  - Results screen
  - Reservation screen
  - Confirmation screen
  - Shared UI state and service integration

#### Tests

- tests/Unit
  - Domain rule tests
  - Validation tests
  - Provider normalization tests
- tests/Integration
  - API workflow tests
  - End-to-end orchestration tests
- tests/E2E
  - User flow tests for search, reservation, and lookup

### 3.3 Component Responsibilities

| Layer | Responsibility |
| --- | --- |
| Angular Client | Collects traveller input, displays search results, captures reservation details, presents confirmation information, and coordinates UI state through shared services and reactive forms. |
| API Layer | Exposes search, reservation, and reservation lookup endpoints and translates HTTP concerns into application operations. |
| Application Layer | Coordinates search and reservation workflows, applies validation rules, and orchestrates provider queries. |
| Domain Layer | Defines core concepts, business rules, and validation semantics for search, pricing, room types, documents, and reservations. |
| Infrastructure Layer | Implements provider-specific adapters, deterministic stub providers, and in-memory reservation persistence for the current scope. |

## 4. Domain Model

The domain model will be centered on the business concepts required to support search, normalization, reservation, and document validation. The design should keep these concepts explicit and avoid embedding provider-specific behavior in business logic.

| Concept | Type | Responsibility |
| --- | --- | --- |
| SearchCriteria | Value Object | Represents the traveller input for destination, check-in, check-out, and optional room type. |
| HotelOffer | Entity | Represents a normalized room offer returned to the traveller. |
| ProviderResult | Value Object or Aggregate Boundary | Represents provider-specific data received from a provider before normalization. |
| RoomType | Enum | Defines the unified room types: Standard, Deluxe, Suite. |
| CancellationPolicy | Enum or Value Object | Represents the normalized cancellation policy as a business value describing the number of hours before check-in during which cancellation remains allowed. |
| DestinationCategory | Enum | Distinguishes domestic and international destinations for document validation. |
| DocumentType | Enum | Represents the accepted document options for reservation validation. |
| Reservation | Entity | Represents a confirmed reservation with traveller data, an offer snapshot, and validation state. |
| ReservationReference | Value Object | Encapsulates the unique reservation identifier returned to the traveller. |
| ReservationRequest | Value Object | Represents the traveller-provided reservation details submitted for validation and confirmation. |
| DocumentValidationResult | Value Object | Records whether a submitted document satisfies the destination-based validation rule. |

### 4.1 Domain Responsibilities

- The domain layer will define the rules for room-type normalization, pricing calculation, destination-based document validation, and reservation acceptance.
- The domain layer will not depend on transport, UI, or provider-specific implementation details.
- The domain layer will remain the authority for business rules such as filtering unavailable rooms and validating reservation documents.

## 5. Business Workflow

### 5.1 Search to Results

1. The traveller enters a destination, check-in date, check-out date, and optional room type.
2. The application sends a search request to the API layer.
3. The application layer queries both providers through the provider abstraction.
4. Each provider returns its result using the provider-specific response shape.
5. The application layer normalizes each provider result into the shared offer model.
6. BudgetNests results marked as unavailable are excluded from the traveller-facing list.
7. The normalized results are returned to the frontend for display.
8. The returned result set remains scoped to the supplied destination, check-in date, check-out date, and optional room type so the traveller sees results relevant to the current search context.

### 5.2 Reservation Flow

1. The traveller selects a result and submits a reservation request.
2. The client validates the reservation form before submission.
3. The API layer forwards the request to the application layer.
4. The application validates the destination and submitted document.
5. If the document is valid, the reservation is accepted and a reservation reference is generated.
6. The selected offer details are captured as a reservation snapshot and persisted as part of the reservation record.
7. The reservation is returned to the traveller in a confirmation view that includes the reservation reference, provider, total price, and cancellation policy.
8. The traveller can retrieve reservation details later using the reservation reference without needing to re-query the catalog.

## 6. API Design

The API design should remain aligned with the approved endpoints and support the required search, reservation, and lookup behaviours.

### 6.1 Search Hotels

| Item | Detail |
| --- | --- |
| Method | GET |
| Route | /hotels/search |
| Purpose | Query both providers, filter unavailable rooms, normalize results, and return a unified list that reflects the selected destination, stay dates, and optional room type. |
| Required Query Parameters | destination, checkIn, checkOut |
| Optional Query Parameter | roomType |
| Success Status | 200 |
| Validation Failure Status | 400 |
| Notes | The response must contain a normalized list of hotel offers suitable for UI rendering. |

#### Request Model

- destination
- checkIn
- checkOut
- roomType (optional)

#### Response Model

- results: normalized list of hotel offers
- provider name
- room type
- per-night rate
- total stay price
- cancellation policy
- relevance to the current search criteria

### 6.2 Reserve Hotel

| Item | Detail |
| --- | --- |
| Method | POST |
| Route | /hotels/reserve |
| Purpose | Validate the supplied document, create a reservation, and return a reservation reference. |
| Success Status | 200 or 201 |
| Validation Failure Status | 422 |
| Notes | The request shall be rejected when the supplied document does not satisfy the destination rule. |

#### Request Model

- travellerName
- destination
- documentType
- documentNumber
- selectedOfferId

#### Response Model

- reservationReference
- provider
- totalPrice
- cancellationPolicy
- reservationSnapshot

### 6.3 Retrieve Reservation

| Item | Detail |
| --- | --- |
| Method | GET |
| Route | /hotels/reservation/{reference} |
| Purpose | Return reservation details for an existing reference. |
| Success Status | 200 |
| Missing Resource Status | 404 |

#### Response Model

- reservationReference
- travellerName
- provider
- roomType
- totalPrice
- cancellationPolicy
- validationOutcome

### 6.4 Validation Behaviour

- Missing destination, check-in, or check-out values shall be rejected with 400.
- A check-out date that is not after check-in shall be rejected with 400.
- Invalid document type for the destination category shall be rejected with 422.
- Validation failures must return a clear, meaningful message.

## 7. Provider Integration Design

### 7.1 Provider Abstraction

The solution will define a provider abstraction that allows multiple hotel providers to be added through a consistent contract. The abstraction will expose the required capabilities for retrieving provider-specific availability data in a provider-neutral way.

### 7.2 Provider-Specific Behaviour

| Provider | Response Shape | Availability Behaviour | Cancellation Policy Behaviour |
| --- | --- | --- | --- |
| PremierStays | PascalCase JSON | Always returns available rooms | FreeCancellation up to 48 hours before check-in, or NonRefundable |
| BudgetNests | snake_case JSON | May return unavailable rooms; filter out rooms with `available: false` | Flexible up to 24 hours before check-in, or NonRefundable |

### 7.2a Destination Category Constraints

- Domestic destinations shall be represented by at least 2 known cities.
- International destinations shall be represented by at least 3 known cities.
- Each provider shall determine the destination category by matching the search criteria destination against known domestic and international city lists.
- When a destination matches a domestic city, PremierStays returns available rooms and BudgetNests may return unavailable rooms (which shall be filtered out).
- When a destination matches an international city, both providers return only available rooms.
- Providers filter their catalog based on destination category to ensure search results respect the traveller's selected destination.

### 7.3 Normalization and Mapping Strategy

- Provider-specific responses will be normalized into the shared offer model before they are returned to the traveller.
- Room types from both providers will be mapped into the unified RoomType enum.
- Per-night pricing will be converted into a consistent representation of total stay price based on the stay duration.
- Cancellation policies will be normalized into a common contract for display and downstream handling.
- The normalization layer will be responsible for translation and not for business decision-making beyond mapping the provider response into the shared model.
- Mapping and normalization should be handled by dedicated mapper classes or adapters rather than embedded directly within orchestration services.

### 7.4 Future Provider Extension

A new provider can be introduced by implementing the same provider contract and supplying its mapping rules. The search workflow, reservation workflow, and UI experience should continue to work without requiring changes to the core traveller journey.

## 8. Validation Strategy

Validation must be applied consistently at each boundary to preserve data quality and user confidence.

### 8.1 Client Validation

The Angular client will validate the reservation form before submission using reactive forms and explicit field-level validation. This includes:

- Required traveller name
- Required document type
- Required document number
- Destination-compatible document selection

The client must prevent submission when a document requirement is not satisfied and should surface a clear validation message.

### 8.2 API Validation

The API layer will validate incoming request parameters and request bodies. This includes:

- Required search parameters
- Valid date ordering
- Required reservation fields
- Destination-based document compatibility

### 8.3 Application and Domain Validation

The application layer will coordinate validation requests, while the domain layer will enforce the business rules for:

- Destination category determination
- Document requirement evaluation
- Reservation acceptance or rejection
- Unavailable room handling

### 8.4 Document Validation Rules (Destination-Specific)

Identity document validation is destination-specific and must be enforced consistently on both client and server:

#### Domestic Destinations
- **Accepted Document:** National ID only
- **Rejected Document:** Passport
- **Message on Rejection:** "Domestic destinations require a National ID."

#### International Destinations
- **Accepted Document:** Passport only
- **Rejected Document:** National ID
- **Message on Rejection:** "International destinations require a Passport."

#### Validation Implementation
1. **Client-Side (Angular):** The reservation form must:
   - Determine the destination category by comparing the destination against known domestic and international city lists
   - Dynamically update the document type dropdown to show only the valid choice for that destination
   - Display a contextual hint indicating which document is required
   - Prevent form submission if the wrong document is selected
   - Surface validation errors before sending the request

2. **Server-Side (.NET):** The reservation endpoint must:
   - Re-validate the submitted document against the destination
   - Return HTTP 422 (Unprocessable Entity) with a meaningful error message if validation fails
   - Never create a reservation with an invalid document

## 9. Error Handling Strategy

The API and UI layers should respond to validation and processing failures in a consistent and understandable way.

| Scenario | Expected Status | Expected Behaviour |
| --- | --- | --- |
| Missing required search parameters | 400 | Return a validation error with a clear message. |
| Check-out not after check-in | 400 | Return a validation error with a clear message. |
| Invalid document for destination | 422 | Reject the reservation and return a clear validation message. |
| Reservation reference not found | 404 | Return a not-found response with a clear message. |
| Unexpected processing failure | 500 or equivalent internal error response | Return a generic failure response without exposing internal detail. |

All error responses should remain consistent in structure and language so the client can display them clearly without ambiguity.

## 10. UI Design

The frontend experience should align with the approved business workflow and present a clear path from search to confirmation.

### 10.1 Search Screen

Purpose:
- Collect destination, check-in, check-out, and optional room type.

Key elements:
- Destination input
- Check-in date input
- Check-out date input
- Optional room type selector
- Search action

Expected behaviour:
- Missing required values should be prevented or clearly reported.
- The user should be able to review the returned normalized offers.
- The UI should expose clear loading, empty, and error states while search requests are in progress or when no matching results are available.
- Shared UI state should be coordinated through explicit state services or equivalent state containers rather than browser-window custom events.

### 10.2 Results Screen

Purpose:
- Present normalized available offers from both providers.

Key elements:
- Provider badge
- Room type
- Per-night rate
- Total stay price
- Cancellation policy
- Sort capability by total price

Expected behaviour:
- Unavailable BudgetNests rooms must not appear.
- Results should show a consistent, unified presentation regardless of provider origin.
- The UI should maintain a shared state for the selected offer so the reservation form can be populated consistently from the chosen result.

### 10.3 Reservation Screen

Purpose:
- Capture the traveller’s reservation details for the selected offer.

Key elements:
- Traveller name
- Document type
- Document number
- Destination context
- Submit reservation action

Expected behaviour:
- Client-side validation should prevent invalid submission.
- Server-side validation must re-check the document requirement before allowing reservation creation.
- The reservation screen should communicate submission progress and surface validation feedback clearly while preserving the current form values.

### 10.4 Confirmation Screen

Purpose:
- Display a successful reservation outcome.

Key elements:
- Reservation reference number
- Provider name
- Total price
- Cancellation policy
- Reservation summary

Expected behaviour:
- The traveller should be able to view the reservation reference and understand the result of the reservation attempt.
- The confirmation view should present the reservation reference, provider, total price, and cancellation policy in a consistent layout.

### 10.5 State Management and Feedback Handling

Purpose:
- Keep the frontend experience predictable as the traveller moves from search to confirmation.

Key considerations:
- Shared UI state should track the current search criteria, normalized results, selected offer, reservation form values, and confirmation outcome.
- The UI should surface loading, empty, success, and error states for search and reservation interactions.
- Error handling should remain consistent with the API response structure so the traveller receives understandable feedback without ambiguous state transitions.

## 11. Testing Strategy

The testing strategy should verify that the business behaviour and required workflows are correct without depending on external providers or persistence.

### 11.1 Unit Testing Scope

- Domain rule validation for destination and document handling
- Room-type normalization rules
- Pricing and total-stay calculation rules
- Provider mapping and normalization behaviour
- Reservation acceptance and rejection logic

### 11.2 Integration Testing Scope

- Search workflow from API through provider abstraction to normalized response
- Reservation workflow from API request through validation and reservation creation
- Reservation lookup workflow
- Error response behaviour for invalid input and validation failure
- End-to-end HTTP testing using WebApplicationFactory for the API layer

### 11.3 UI Testing Scope

- Search form validation and submission behaviour
- Results display behaviour and sorting
- Reservation form validation and confirmation flow
- Confirmation screen rendering for successful and failed reservations
- Loading, empty, and error-state rendering for search and reservation interactions
- UI state transitions across search, result selection, reservation submission, and confirmation views
- Angular component and service unit tests for form validation, state handling, and API integration behaviour

### 11.4 Test Data and Determinism

- Stub providers should be deterministic and representative of the approved business scenarios.
- Tests should verify both successful and failing business cases without relying on real external services.

## 12. Design References

The supporting workflow documentation should remain consistent with this specification:

- The business workflow and sequence of interactions are captured in sequence-diagram.md.
- The approved business requirements remain defined in requirement-analysis.md.
- Repository conventions and implementation standards are defined in copilot-instructions.md.

## 13. AI Verification and Validation Checklist

- [x] The design is aligned to the approved requirement analysis and preserves the approved business requirements.
- [x] The specification covers backend, frontend, and test structure.
- [x] The domain model captures the core entities, value objects, enums, and responsibilities needed for implementation.
- [x] The business workflow from search through reservation confirmation is documented.
- [x] API contracts include request models, response models, validation behaviour, and status codes.
- [x] Provider integration design includes normalization and provider mapping strategy.
- [x] Reservation processing, validation strategy, and error handling are documented.
- [x] UI design covers search, results, reservation, and confirmation experiences.
- [x] Testing scope and responsibilities are documented for unit, integration, and UI coverage.
- [x] Future extensibility for additional providers is addressed.
- [x] Supporting design documents are referenced where appropriate.
