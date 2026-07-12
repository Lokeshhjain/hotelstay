namespace HotelStay.Api.Contracts;

public sealed record SearchHotelsResponse(IReadOnlyList<HotelOfferDto> Results);
