namespace HotelStay.Domain.ValueObjects;

public sealed record DocumentValidationResult(bool IsValid, string Message);
