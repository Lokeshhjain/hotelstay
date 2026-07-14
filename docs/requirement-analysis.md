# Requirement Analysis

| Field | Value |
|-------|-------|
| Project | SkyRoute – Hotel Availability |
| Version | 1.0 |
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

---

# 3. Business Objectives

- Provide a unified hotel availability search.
- Normalize provider-specific responses into a common format.
- Allow travellers to reserve hotel rooms.
- Validate traveller identity documents based on destination.
- Provide a consistent booking experience.
- Ensure the system can easily support additional hotel providers.
- Support offline execution without external dependencies.

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

## Out of Scope

- Authentication
- Authorization
- Real hotel providers
- Database persistence
- Payment processing
- Email notifications
- SMS notifications
- Hotel management functionality

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

# 8. API Requirements

### Search Hotels

GET /hotels/search

---

### Reserve Hotel

POST /hotels/reserve

---

### Reservation Lookup

GET /hotels/reservation/{reference}

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