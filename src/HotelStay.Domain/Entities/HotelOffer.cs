using HotelStay.Domain.Enums;

namespace HotelStay.Domain.Entities;

public sealed class HotelOffer
{
    public string Id { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public RoomType RoomType { get; init; }
    public decimal PerNightRate { get; init; }
    public decimal TotalStayPrice { get; init; }
    public CancellationPolicy CancellationPolicy { get; init; }
    public int CancellationWindowHoursBeforeCheckIn { get; init; }
    public bool IsAvailable { get; init; } = true;
}
