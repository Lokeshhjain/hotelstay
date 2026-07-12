namespace HotelStay.Api.Contracts;

public sealed record SearchHotelsRequest(string? Destination, DateOnly? CheckIn, DateOnly? CheckOut, string? RoomType);
