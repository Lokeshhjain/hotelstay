namespace HotelStay.Api.Contracts;

public sealed record ApiErrorResponse(string Code, string Message, IReadOnlyCollection<string>? Details = null);
