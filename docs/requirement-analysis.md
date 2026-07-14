# Requirement Analysis

| Field | Value |
|-------|-------|
| Project | SkyRoute – Hotel Availability |
| Version | 1.1 |
| Status | Draft |
| Document Type | Requirement Analysis |
| Development Approach | Specification Driven Development (SDD) |

---

# 1. Executive Summary

SkyRoute requires a Hotel Availability feature that enables travellers to search available hotel rooms from multiple providers, compare results using a unified format, and reserve a selected room after successful document validation.

The solution must aggregate results from two independent hotel providers, normalize their responses, provide a consistent user experience, and support future provider expansion without changing business functionality.

The application must run completely offline using deterministic stub providers.

---

# 2. Problem Statement

Different hotel providers expose availability information using different response formats and business rules.

SkyRoute requires a unified hotel search experience where travellers can search across multiple providers without being aware of provider-specific implementations.

The reservation process must also enforce destination-specific identity document validation.

The current review also highlights that hotel availability data must be fully destination-aware, provider-specific response variations must be preserved as business-relevant behavior, and reservation attempts must be tied to a valid selected offer rather than manual entry or incomplete state.

---

# 3. Business Objectives

- Provide a unified hotel availability search.
- Normalize provider-specific responses into a common format.
- Allow travellers to reserve hotel rooms.
- Validate traveller identity documents based on destination.
- Provide a consistent booking experience.
- Ensure the system can easily support additional hotel providers.
- Support offline execution without external dependencies.
- Ensure search results and reservations are consistent with the traveller’s selected destination and selected offer context.

---

# 4. Scope

## In Scope

- Hotel availability search
- Hotel reservation
- Reservation retrieval
- Two stub hotel providers
- Response normalization
- Room type filtering
- Price calculation
- Identity document validation
- Angular frontend
- REST API
- Offline execution
- Destination-scoped search results and offer binding
- Consistent handling of invalid or stale reservation requests

## Out of Scope

- Authentication
- Authorization
- Real hotel providers
- Database persistence
- Payment processing
- Email notifications
- SMS notifications
- Hotel management functionality
- External provider integrations beyond the offline stub environment

---

# 5. Stakeholders

| Stakeholder | Responsibility |
|-------------|---------------|
| Traveller | Searches and reserves hotel rooms |
| SkyRoute Platform | Provides hotel search and reservation functionality |
| Hotel Providers | Supply room availability information |
| Development Team | Implements the solution |
| QA Team | Validates business functionality |

---

# 6. User Roles

## Traveller

Can:

- Search hotels
- View room availability
- Reserve a room
- View reservation details

---

# 7. Functional Requirements

## FR-001 Search Hotel Availability

The system shall allow a traveller to search hotel availability using:

- Destination
- Check-in date
- Check-out date
- Optional room type

---

## FR-002 Query Multiple Providers

The system shall query the following providers:

- PremierStays
- BudgetNests

---

## FR-003 Normalize Provider Responses

The system shall convert provider-specific responses into a unified response model before returning results.

---

## FR-004 Filter Unavailable Rooms

The system shall exclude rooms marked as unavailable by BudgetNests.

---

## FR-005 Support Room Types

The system shall support the following room types:

- Standard
- Deluxe
- Suite

Provider-specific room types shall be mapped to a unified RoomType enumeration.

---

## FR-006 Display Pricing

The system shall display:

- Per-night rate
- Total stay price

---

## FR-007 Display Provider Information

Search results shall display:

- Provider name
- Room type
- Per-night rate
- Total stay price
- Cancellation policy

---

## FR-008 Sort Search Results

The traveller shall be able to sort search results by total price.

---

## FR-009 Reserve Room

The traveller shall be able to reserve an available room.

---

## FR-010 Validate Reservation Documents

Before confirming a reservation:

- International destinations require Passport.
- Domestic destinations accept National ID.

---

## FR-011 Destination Categories

The system shall define:

- Minimum two domestic cities
- Minimum three international cities

---

## FR-012 Client Validation

Identity document validation shall be performed on the client before reservation submission.

---

## FR-013 Server Validation

Identity document validation shall be performed again on the server.

---

## FR-014 Validation Failure

When an invalid document is submitted:

- Reservation shall not be created.
- HTTP 422 shall be returned.
- A meaningful validation message shall be returned.

---

## FR-015 Reservation Confirmation

Successful reservations shall produce a structured confirmation view that includes:

- Reservation reference number
- Provider name
- Total price
- Cancellation policy

---

## FR-016 Retrieve Reservation

The traveller shall be able to retrieve reservation details using the reservation reference.

---

## FR-017 Search Result Relevance

Search results shall reflect the traveller’s selected destination, check-in date, check-out date, and optional room type.

---

## FR-018 Destination-Scoped Availability Data

The system shall ensure that hotel availability results are scoped to the selected destination so that unrelated cities do not return the same availability set.

---

## FR-019 Provider-Specific Response Handling

The system shall preserve and validate provider-specific availability behavior so that differences in payload structure and availability semantics remain part of the provider integration experience rather than being flattened into a single generic stub.

---

## FR-020 Consistent Validation Behaviour

The system shall handle invalid search and reservation input through a consistent validation path so that travellers receive predictable and meaningful failure responses.

---

## FR-021 Offer Selection Integrity

The system shall bind reservation requests to a selected offer from the active search context or an equivalent captured offer snapshot rather than requiring the traveller to enter an offer identifier manually.

---

## FR-022 Clear Handling of Missing or Stale Offers

The system shall return a clear validation or business-rule failure when a reservation request references an offer that is missing, stale, or no longer valid for the current search context.

---

## FR-023 Provider Stub Fidelity

The system shall ensure stubbed provider data models real provider differences: PremierStays responses must use PascalCase JSON shapes and BudgetNests must use snake_case JSON shapes. Stub providers must expose destination-scoped data so results vary by searched city rather than returning the same global sample set.

---

## FR-024 API Integration Tests (WebApplicationFactory)

The system shall include API integration tests that use `WebApplicationFactory` (or equivalent test host) to exercise `/hotels/search`, `/hotels/reserve`, and `/hotels/reservation/{reference}` end-to-end, including validation error paths and reservation lookup.

---

## FR-025 Frontend Lookup Component Tests

The frontend shall include unit tests for the `LookupComponent` (or equivalent) to verify reservation lookup behaviour, error display for not-found references, and proper rendering of reservation data.

---

# 8. Business Rules

- BR-001 Search results must be relevant to the supplied destination, stay dates, and optional room type.
- BR-002 Rooms marked as unavailable by a provider must not be presented as available offers.
- BR-003 Reservations must only be created for offers that are valid in the current booking context.
- BR-004 A reservation must be associated with a confirmed offer snapshot so that booking confirmation remains stable even if catalog data changes later.
- BR-005 Document validation must be based on the destination category and must reject invalid document combinations.
- BR-006 The system must not allow a reservation to proceed when the selected offer cannot be resolved from the active booking context.

---

# 9. Validation Rules

- VR-001 Destination, check-in date, and check-out date are mandatory for hotel search.
- VR-002 Check-out date must be later than the check-in date.
- VR-003 The submitted document type must be valid for the destination category.
- VR-004 The selected offer must be identifiable and valid before a reservation is confirmed.
- VR-005 Reservation requests that reference missing or stale offers must be rejected with a clear validation or business-rule message.

---

# 10. API Requirements

### Search Hotels

GET /hotels/search

---

### Reserve Hotel

POST /hotels/reserve

---

### Reservation Lookup

GET /hotels/reservation/{reference}

The API must support request payloads that carry the selected offer context from the client without relying on manual offer entry or ambiguous state.

---

# 11. Assumptions

- The solution remains offline and uses deterministic stub provider data.
- The business scenario focuses on a single traveller booking flow rather than multi-user concurrent operations.
- The destination-based document rules are limited to domestic and international categories as defined in the approved scope.

---

# 12. Constraints

- No real hotel provider integrations are in scope.
- No persistent database is in scope for the current case study.
- No authentication, payment, or notification workflows are in scope.
- Reservation and search behaviour must remain deterministic and reviewable in an offline environment.

---

# 13. Risks

- Inconsistent destination scoping could lead to incorrect or misleading search results.
- Weak offer-binding rules could create ambiguous or failed reservation attempts.
- Provider-specific differences could be flattened incorrectly and reduce the usefulness of the integration model.
- Validation inconsistencies could cause confusion for travellers and testing teams.

 - Missing integration tests increase the risk of regressions in end-to-end flows.
 - Absent frontend lookup tests may allow UI lookup regressions to go unnoticed.

---

# 14. Dependencies

- Availability data from the stub providers.
- Destination category rules for domestic and international validation.
- Reservation and offer catalog state used during the booking flow.
- Frontend state that preserves the selected offer from search results into reservation submission.

---

# 15. Acceptance Criteria

- AC-001 A traveller searching for hotels in a specific destination receives only offers that are relevant to that destination.
- AC-002 A reservation can be created only when the selected offer is valid and present in the current booking context.
- AC-003 A reservation request that references a missing or stale offer is rejected with a clear validation or business-rule message.
- AC-004 A traveller does not need to manually type an offer identifier to complete a reservation.
- AC-005 Invalid document or reservation input is rejected consistently and with a meaningful message.

 - AC-006 Provider stubs model provider-specific JSON shapes (PascalCase and snake_case) and return destination-scoped offers for different cities.
 - AC-007 The API has integration tests that exercise search, reserve, and lookup flows via a test host (e.g. `WebApplicationFactory`) including error paths.
 - AC-008 The frontend contains unit tests for the `LookupComponent` verifying successful lookup and not-found handling.
 - AC-009 The reservation UX does not present a free-text "Selected offer ID" input; reservations must be created using an offer selected from search results or a captured offer snapshot.

---

# 16. Requirement Traceability Matrix

| Requirement ID | Coverage |
|---|---|
| FR-001 to FR-017 | Covered by the core search, reservation, validation, and lookup flows defined in this analysis. |
| FR-018 | Covered by destination-scoped search result behavior. |
| FR-019 | Covered by provider-specific response handling and normalization expectations. |
| FR-020 | Covered by consistent validation and error handling requirements. |
| FR-021 | Covered by offer selection integrity and booking flow requirements. |
| FR-022 | Covered by clear failure handling for missing or stale offers. |
| FR-023 | Covered by provider stub fidelity and destination-scoped stub data requirements. |
| FR-024 | Covered by API integration test requirements using a test host. |
| FR-025 | Covered by frontend unit testing requirements for lookup behaviour. |

---

# 17. Glossary

- Destination category: A business classification used to determine the appropriate document validation rule.
- Hotel offer: A normalized room option that is returned to the traveller for review and booking.
- Reservation reference: A unique identifier used to retrieve a confirmed reservation.
- Selected offer: The offer chosen by the traveller from the search results and carried into the reservation workflow.

---

# 18. Open Questions

- Should destination-scoped availability data be governed by provider-specific city lists or by a shared business catalog?
- Should the booking flow require a stronger offer-binding mechanism than the current search-result context?
- Should invalid or stale offer attempts be surfaced as validation errors, business-rule errors, or both?

 - Should stub providers intentionally simulate different JSON casings (PascalCase/snake_case) and shape quirks, or should normalization be tested only at the mapper layer?
 - Should an API-level 400/422 be returned for cold-start reservation attempts instead of an opaque "offer not found" business exception?
 - Should the `LookupComponent` be the single UI surface for reservation retrieval testing, or should a shared service be tested instead to reduce UI coupling?

---

# 9. Business Rules

## BR-001

PremierStays returns PascalCase JSON.

---

## BR-002

BudgetNests returns snake_case JSON.

---

## BR-003

PremierStays always returns available rooms.

---

## BR-004

BudgetNests may return unavailable rooms.

Unavailable rooms shall not be returned to the traveller.

---

## BR-005

PremierStays cancellation policies:

- FreeCancellation (48 hours)
- NonRefundable

---

## BR-006

BudgetNests cancellation policies:

- Flexible (24 hours)
- NonRefundable

---

## BR-007

Both providers expose per-night pricing.

---

## BR-008

Total stay price shall be calculated using the stay duration.

---

## BR-009

Provider-specific room types shall be normalized.

---

## BR-010

Document validation depends on destination type.

---

## BR-012

Domestic destination list shall contain at least 2 cities.

---

## BR-013

International destination list shall contain at least 3 cities.

---

## BR-014

BudgetNests rooms returned with `available: false` shall be filtered out and not shown to the traveller.

---

## BR-015

International destinations always return available rooms from all providers.

---


| Rule | Validation |
|------|------------|
| Destination | Mandatory |
| Check-in | Mandatory |
| Check-out | Mandatory |
| Check-out | Must be after Check-in |
| Room Type | Optional |
| Passport | Required for international destination |
| National ID | Accepted for domestic destination |

---

# 11. Non-Functional Requirements

## NFR-001

The application shall run completely offline.

---

## NFR-002

The solution shall use deterministic provider responses.

---

## NFR-003

The solution shall support adding additional providers with minimal impact.

---

## NFR-004

The application shall be maintainable.

---

## NFR-005

The application shall be testable.

---

## NFR-006

The application shall provide consistent API responses.

---

## NFR-007

The application shall provide meaningful validation messages.

---

# 12. Assumptions

- Reservation data may be stored in memory.
- Reference numbers are generated by the application.
- Provider responses are deterministic.
- Dates are supplied in a valid format.
- Time zone handling is outside the current scope.

---

# 13. Constraints

- No authentication.
- No authorization.
- No database persistence.
- No external APIs.
- No real hotel providers.
- Must execute completely offline.

---

# 14. Risks

- Provider response formats may change in future.
- Document validation rules may expand.
- Additional providers may introduce new room types.
- Business rules may evolve.

---

# 15. Dependencies

- Angular frontend
- Hotel provider stubs
- REST API
- Shared domain models

---

# 16. Acceptance Criteria

- AC-001: A traveller can search hotels using destination, check-in, check-out, and an optional room type.
- AC-002: Search results are returned from both providers and normalized into a unified response.
- AC-003: Rooms marked as unavailable by BudgetNests are excluded from the traveller experience.
- AC-004: Per-night and total-stay prices are displayed for each result.
- AC-005: Reservation succeeds when the required document is valid for the destination.
- AC-006: Reservation fails when the document is invalid for the destination and returns HTTP 422 with a meaningful message.
- AC-007: Reservation lookup returns the corresponding reservation details using the reservation reference.
- AC-008: The application runs fully offline using deterministic stub providers.
- AC-009: The business flow can accommodate a third provider without changing the core traveller experience.
- AC-010: A successful reservation produces a confirmation view that includes the reservation reference, provider, total price, and cancellation policy.
- AC-011: Search results are returned in a way that reflects the selected destination, check-in date, check-out date, and optional room type.

---

# 17. Requirement Traceability Matrix

| ID | Category | Source | Acceptance Criteria |
|----|----------|--------|---------------------|
| FR-001 | Functional Requirement | Business Requirement | AC-001 |
| FR-002 | Functional Requirement | Business Requirement | AC-002 |
| FR-003 | Functional Requirement | Business Requirement | AC-002 |
| FR-004 | Functional Requirement | Business Requirement | AC-003 |
| FR-005 | Functional Requirement | Business Requirement | AC-001, AC-002 |
| FR-006 | Functional Requirement | Business Requirement | AC-004 |
| FR-007 | Functional Requirement | Business Requirement | AC-002 |
| FR-008 | Functional Requirement | Business Requirement | AC-001 |
| FR-009 | Functional Requirement | Business Requirement | AC-005 |
| FR-010 | Functional Requirement | Business Requirement | AC-005, AC-006 |
| FR-011 | Functional Requirement | Business Requirement | AC-001 |
| FR-012 | Functional Requirement | Business Requirement | AC-005, AC-006 |
| FR-013 | Functional Requirement | Business Requirement | AC-005, AC-006 |
| FR-014 | Functional Requirement | Business Requirement | AC-006 |
| FR-015 | Functional Requirement | Business Requirement | AC-005, AC-010 |
| FR-016 | Functional Requirement | Business Requirement | AC-007 |
| FR-017 | Functional Requirement | Business Requirement | AC-011 |
| BR-001 | Business Rule | Business Requirement | AC-002 |
| BR-002 | Business Rule | Business Requirement | AC-002 |
| BR-003 | Business Rule | Business Requirement | AC-002 |
| BR-004 | Business Rule | Business Requirement | AC-003 |
| BR-005 | Business Rule | Business Requirement | AC-002, AC-004 |
| BR-006 | Business Rule | Business Requirement | AC-002, AC-004 |
| BR-007 | Business Rule | Business Requirement | AC-004 |
| BR-008 | Business Rule | Business Requirement | AC-004 |
| BR-009 | Business Rule | Business Requirement | AC-002 |
| BR-010 | Business Rule | Business Requirement | AC-005, AC-006 |
| BR-011 | Business Rule | Business Requirement | AC-010 |
| NFR-001 | Non-Functional Requirement | Business Requirement | AC-008 |
| NFR-002 | Non-Functional Requirement | Business Requirement | AC-008 |
| NFR-003 | Non-Functional Requirement | Business Requirement | AC-009 |
| NFR-004 | Non-Functional Requirement | Business Requirement | AC-008 |
| NFR-005 | Non-Functional Requirement | Business Requirement | AC-008 |
| NFR-006 | Non-Functional Requirement | Business Requirement | AC-002 |
| NFR-007 | Non-Functional Requirement | Business Requirement | AC-006 |

---

# 18. Glossary

| Term | Meaning |
|------|---------|
| Provider | Hotel data source |
| Room Type | Standard, Deluxe, Suite |
| Normalization | Converting provider-specific responses into a unified format |
| Reservation Reference | Unique booking identifier |
| Domestic Destination | City accepting National ID |
| International Destination | City requiring Passport |

---

# 19. Open Questions

- Should duplicate rooms from different providers be displayed separately?
- What reservation reference format should be used?
- Should reservation references expire?
- Should hotel search support pagination?