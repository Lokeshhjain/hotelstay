namespace HotelStay.Api.Contracts;

public sealed record ReservationLookupDto(
    string ReservationReference,
    string TravellerName,
    string Provider,
    string RoomType,
    decimal TotalPrice,
    string CancellationPolicy,
    string ValidationOutcome);
