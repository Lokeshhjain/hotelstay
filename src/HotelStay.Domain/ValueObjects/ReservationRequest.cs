using HotelStay.Domain.Enums;

namespace HotelStay.Domain.ValueObjects;

public sealed record ReservationRequest(string TravellerName, string Destination, DocumentType DocumentType, string DocumentNumber, string SelectedOfferId);
