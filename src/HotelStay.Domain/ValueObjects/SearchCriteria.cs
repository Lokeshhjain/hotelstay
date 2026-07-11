namespace HotelStay.Domain.ValueObjects;

public sealed record SearchCriteria(string Destination, DateOnly CheckIn, DateOnly CheckOut, string? RoomType)
{
    public int Nights => Math.Max(1, CheckOut.DayNumber - CheckIn.DayNumber);
}
