using HotelStay.Api.Contracts;
using HotelStay.Api.Validation;
using HotelStay.Application.Contracts;
using HotelStay.Domain.Entities;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Api.Routes;

public static class HotelRoutes
{
    public static void MapHotelRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/hotels/search", async (string? destination, DateOnly? checkIn, DateOnly? checkOut, string? roomType, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
        {
            var request = new SearchHotelsRequest(destination, checkIn, checkOut, roomType);
            var errors = HotelRequestValidator.ValidateSearch(request);

            if (errors.Count > 0)
            {
                return Results.BadRequest(new ApiErrorResponse("VALIDATION_ERROR", "Search request validation failed.", errors));
            }

            var criteria = new SearchCriteria(destination!, checkIn!.Value, checkOut!.Value, roomType);
            try
            {
                var offers = await service.SearchHotelsAsync(criteria, cancellationToken);

                return Results.Ok(new SearchHotelsResponse(
                    offers.Select(offer => new HotelOfferDto(
                        offer.Id,
                        offer.Provider,
                        offer.RoomType.ToString(),
                        offer.PerNightRate,
                        offer.TotalStayPrice,
                        offer.CancellationPolicy.ToString(),
                        offer.CancellationWindowHoursBeforeCheckIn)).ToList()));
            }
            catch (HotelStay.Application.Exceptions.InvalidRequestException ex)
            {
                return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", ex.Message, new[] { ex.Message }));
            }
        });

        app.MapPost("/hotels/reserve", async (ReserveHotelRequest request, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
        {
            var errors = HotelRequestValidator.ValidateReservation(request);

            if (errors.Count > 0)
            {
                var code = errors.Any(x => x.Contains("invalid", StringComparison.OrdinalIgnoreCase)) ? "VALIDATION_ERROR" : "INVALID_REQUEST";
                return Results.BadRequest(new ApiErrorResponse(code, "Reservation request validation failed.", errors));
            }

            if (!Enum.TryParse<DocumentType>(request.DocumentType!, true, out var documentType))
            {
                return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Reservation request validation failed.", new[] { "Document type is invalid." }));
            }

            HotelOffer? offerSnapshot = null;
            if (request.OfferSnapshot is not null)
            {
                if (!Enum.TryParse<RoomType>(request.OfferSnapshot.RoomType, true, out var roomType) ||
                    !Enum.TryParse<CancellationPolicy>(request.OfferSnapshot.CancellationPolicy, true, out var cancellationPolicy))
                {
                    return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Offer snapshot contains invalid values.", new[] { "Offer snapshot contains invalid values." }));
                }

                offerSnapshot = new HotelOffer
                {
                    Id = request.OfferSnapshot.Id,
                    Provider = request.OfferSnapshot.Provider,
                    RoomType = roomType,
                    PerNightRate = request.OfferSnapshot.PerNightRate,
                    TotalStayPrice = request.OfferSnapshot.TotalStayPrice,
                    CancellationPolicy = cancellationPolicy,
                    CancellationWindowHoursBeforeCheckIn = request.OfferSnapshot.CancellationWindowHoursBeforeCheckIn,
                    IsAvailable = true
                };
            }

            try
            {
                var reservationRequest = new ReservationRequest(
                    request.TravellerName!,
                    request.Destination!,
                    documentType,
                    request.DocumentNumber!,
                    request.SelectedOfferId,
                    offerSnapshot);

                try
                {
                    var reservation = await service.ReserveHotelAsync(reservationRequest, cancellationToken);

                    return Results.Ok(new ReservationDto(
                        reservation.ReservationReference.Value,
                        reservation.Provider,
                        reservation.TotalPrice,
                        reservation.CancellationPolicy.ToString(),
                        reservation.OfferSnapshot is null ? null : new HotelOfferDto(
                            reservation.OfferSnapshot.Id,
                            reservation.OfferSnapshot.Provider,
                            reservation.OfferSnapshot.RoomType.ToString(),
                            reservation.OfferSnapshot.PerNightRate,
                            reservation.OfferSnapshot.TotalStayPrice,
                            reservation.OfferSnapshot.CancellationPolicy.ToString(),
                            reservation.OfferSnapshot.CancellationWindowHoursBeforeCheckIn)));
                }
                catch (HotelStay.Application.Exceptions.InvalidRequestException ex)
                {
                    return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", ex.Message, new[] { ex.Message }));
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("could not be found", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Selected offer not found or stale. Perform a search and select an offer before reserving.", new[] { ex.Message }));
                }
                catch (InvalidOperationException ex)
                {
                    return Results.UnprocessableEntity(new ApiErrorResponse("BUSINESS_RULE_VIOLATION", ex.Message, new[] { ex.Message }));
                }
            }
            catch (InvalidOperationException ex)
            {
                return Results.UnprocessableEntity(new ApiErrorResponse("BUSINESS_RULE_VIOLATION", ex.Message, new[] { ex.Message }));
            }
        });

        app.MapGet("/hotels/reservation/{reference}", async (string reference, IHotelAvailabilityService service, CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return Results.BadRequest(new ApiErrorResponse("INVALID_REQUEST", "Reservation reference is required.", new[] { "Reservation reference is required." }));
            }

            var reservation = await service.GetReservationAsync(reference, cancellationToken);

            return reservation is null
                ? Results.NotFound(new ApiErrorResponse("NOT_FOUND", "Reservation was not found.", new[] { "Reservation was not found." }))
                : Results.Ok(new ReservationLookupDto(
                    reservation.ReservationReference.Value,
                    reservation.TravellerName,
                    reservation.Provider,
                    reservation.RoomType.ToString(),
                    reservation.TotalPrice,
                    reservation.CancellationPolicy.ToString(),
                    reservation.ValidationOutcome));
        });
    }
}
