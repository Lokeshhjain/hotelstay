namespace HotelStay.Domain.ValueObjects;

public sealed record ReservationReference(string Value)
{
    public override string ToString() => Value;
}
