namespace HotelStay.Infrastructure.Mappers;

public sealed record PremierStaysOfferResponse(
    string Id,
    string ProviderName,
    string RoomTypeCode,
    decimal PerNightRate,
    string CancellationPolicy,
    bool IsAvailable,
    int CancellationWindowHoursBeforeCheckIn,
    string Destination);
