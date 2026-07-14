namespace HotelStay.Api.Contracts;

public sealed record ReservationDto(
    string ReservationReference,
    string Provider,
    decimal TotalPrice,
    string CancellationPolicy,
    HotelOfferDto? OfferSnapshot);
