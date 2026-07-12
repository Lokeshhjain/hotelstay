# HotelStay

HotelStay is an offline hotel availability and reservation case study built with:

- ASP.NET Core Web API
- Angular
- xUnit
- in-memory storage
- deterministic stub providers

The solution demonstrates:

- hotel search across multiple providers
- normalized hotel offers
- reservation creation with destination-based document validation
- reservation lookup by reference
- a Bootstrap-based Angular UI

## Solution Structure

- `src/HotelStay.Api` - REST API endpoints
- `src/HotelStay.Application` - orchestration and validation services
- `src/HotelStay.Domain` - business rules, entities, and value objects
- `src/HotelStay.Infrastructure` - stub providers and in-memory repositories
- `HotelStay.UI` - Angular frontend
- `tests/HotelStay.UnitTests` - backend unit tests

## Prerequisites

- .NET 8 SDK
- Node.js 18+ or 20+
- npm

## Setup

### 1. Restore backend dependencies

From the repository root:

```bash
dotnet restore src/HotelStay.slnx
```

### 2. Restore frontend dependencies

```bash
cd HotelStay.UI
npm install
```

## Run the Application

### Backend API

From the repository root:

```bash
dotnet run --project src/HotelStay.Api
```

The API exposes:

- `GET /hotels/search`
- `POST /hotels/reserve`
- `GET /hotels/reservation/{reference}`

### Angular UI

From the `HotelStay.UI` folder:

```bash
npm start
```

The UI runs on:

- `http://localhost:4200`

It is configured to proxy hotel API requests to the local backend.

## Run Tests

### Backend unit tests

From the repository root:

```bash
dotnet test tests/HotelStay.UnitTests/HotelStay.UnitTests.csproj
```

## Manual Test Flow

1. Start the API.
2. Start the Angular UI.
3. Search using destination, check-in, check-out, and optional room type.
4. Select an offer from the results.
5. Submit reservation details.
6. Use the returned reservation reference to look up the reservation.

## Assumptions

- The solution is intentionally offline and does not call external hotel providers.
- Reservation data is stored in memory only.
- Provider responses are deterministic and come from local stub data.
- No authentication or authorization is included in the case study scope.
- No payment processing, email, or SMS notifications are included.
- No database persistence is used.
- Search and reservation flows are designed for a single-user case study scenario.
- Dates are expected to be entered in valid ISO-compatible formats by the UI and API.
- Hotel search filters are limited to destination, check-in, check-out, and optional room type.

## Notes

- The UI uses Bootstrap for styling.
- The backend uses layered architecture to keep domain rules isolated from infrastructure details.
- The case study is suitable as a functional offline demo and interview submission, but not a production deployment without additional hardening.
