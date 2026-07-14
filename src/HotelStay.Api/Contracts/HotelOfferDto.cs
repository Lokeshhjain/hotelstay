namespace HotelStay.Api.Contracts;

public sealed record HotelOfferDto(
    string Id,
    string Provider,
    string RoomType,
    decimal PerNightRate,
    decimal TotalStayPrice,
    string CancellationPolicy,
    int CancellationWindowHoursBeforeCheckIn);
