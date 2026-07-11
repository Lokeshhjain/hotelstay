using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;

namespace HotelStay.Domain.Entities;

public sealed class Reservation
{
    public ReservationReference ReservationReference { get; init; } = default!;
    public string TravellerName { get; init; } = string.Empty;
    public string Destination { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public RoomType RoomType { get; init; }
    public decimal TotalPrice { get; init; }
    public CancellationPolicy CancellationPolicy { get; init; }
    public DocumentType DocumentType { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public string SelectedOfferId { get; init; } = string.Empty;
    public string ValidationOutcome { get; init; } = "Accepted";
}
