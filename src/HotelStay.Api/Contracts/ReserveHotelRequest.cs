namespace HotelStay.Api.Contracts;

public sealed record ReserveHotelRequest(string? TravellerName, string? Destination, string? DocumentType, string? DocumentNumber, string? SelectedOfferId, HotelOfferDto? OfferSnapshot = null);
