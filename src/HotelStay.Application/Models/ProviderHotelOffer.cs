using HotelStay.Domain.Enums;

namespace HotelStay.Application.Models;

public sealed record ProviderHotelOffer(
    string Id,
    string ProviderName,
    string RoomTypeCode,
    decimal PerNightRate,
    CancellationPolicy CancellationPolicy,
    bool IsAvailable,
    int CancellationWindowHoursBeforeCheckIn);
