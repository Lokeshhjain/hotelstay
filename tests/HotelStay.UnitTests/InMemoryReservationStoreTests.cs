using HotelStay.Domain.Entities;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure.Repositories;

namespace HotelStay.UnitTests;

public sealed class InMemoryReservationStoreTests
{
    [Fact]
    public async Task AddAsync_StoresReservationsAndReservedOfferIds()
    {
        var store = new InMemoryReservationStore();
        var reservation = new Reservation
        {
            ReservationReference = new ReservationReference("RSV-12345678"),
            TravellerName = "Alex",
            Destination = "Delhi",
            Provider = "PremierStays",
            RoomType = RoomType.Standard,
            TotalPrice = 360m,
            CancellationPolicy = CancellationPolicy.FreeCancellation,
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID-1",
            SelectedOfferId = "premier-1",
            ValidationOutcome = "Accepted"
        };

        await store.AddAsync(reservation);

        var fetched = await store.GetAsync("rsv-12345678");
        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        Assert.NotNull(fetched);
        Assert.Equal("Alex", fetched!.TravellerName);
        Assert.Contains("premier-1", reservedOfferIds);
    }
}
