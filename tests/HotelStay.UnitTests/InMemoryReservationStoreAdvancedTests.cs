using HotelStay.Domain.Entities;
using HotelStay.Domain.Enums;
using HotelStay.Domain.ValueObjects;
using HotelStay.Infrastructure.Repositories;

namespace HotelStay.UnitTests;

/// <summary>
/// Advanced and edge case tests for InMemoryReservationStore.
/// Covers multiple reservations, concurrent access scenarios, case-insensitive lookups,
/// and boundary conditions in reservation storage and retrieval.
/// </summary>
public sealed class InMemoryReservationStoreAdvancedTests
{
    [Fact]
    public async Task AddAsync_HandlesMultipleReservations()
    {
        var store = new InMemoryReservationStore();
        var reservations = new[]
        {
            CreateReservation("RSV-1", "Alex", "premier-1"),
            CreateReservation("RSV-2", "Bob", "premier-2"),
            CreateReservation("RSV-3", "Charlie", "budget-1")
        };

        foreach (var reservation in reservations)
        {
            await store.AddAsync(reservation);
        }

        // Verify all stored
        Assert.NotNull(await store.GetAsync("rsv-1"));
        Assert.NotNull(await store.GetAsync("rsv-2"));
        Assert.NotNull(await store.GetAsync("rsv-3"));
    }

    [Fact]
    public async Task GetAsync_ReturnsCaseInsensitiveMatch()
    {
        var store = new InMemoryReservationStore();
        var reservation = CreateReservation("RSV-ABC123", "Alex", "offer-1");

        await store.AddAsync(reservation);

        var fetchedWithLowercase = await store.GetAsync("rsv-abc123");
        var fetchedWithMixedCase = await store.GetAsync("RsV-AbC123");
        var fetchedWithOriginal = await store.GetAsync("RSV-ABC123");

        Assert.NotNull(fetchedWithLowercase);
        Assert.NotNull(fetchedWithMixedCase);
        Assert.NotNull(fetchedWithOriginal);
        Assert.Equal("Alex", fetchedWithLowercase!.TravellerName);
    }

    [Fact]
    public async Task GetAsync_ReturnsNullForNonExistentReference()
    {
        var store = new InMemoryReservationStore();

        var result = await store.GetAsync("RSV-NONEXISTENT");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_ReturnsEmptySetWhenNoReservations()
    {
        var store = new InMemoryReservationStore();

        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        Assert.Empty(reservedOfferIds);
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_ReturnsAllReservedOfferIds()
    {
        var store = new InMemoryReservationStore();
        var reservations = new[]
        {
            CreateReservation("RSV-1", "Alex", "offer-1"),
            CreateReservation("RSV-2", "Bob", "offer-2"),
            CreateReservation("RSV-3", "Charlie", "offer-3")
        };

        foreach (var reservation in reservations)
        {
            await store.AddAsync(reservation);
        }

        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        Assert.Equal(3, reservedOfferIds.Count);
        Assert.Contains("offer-1", reservedOfferIds);
        Assert.Contains("offer-2", reservedOfferIds);
        Assert.Contains("offer-3", reservedOfferIds);
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_HandlesDuplicateOfferIdsDuringDuplication()
    {
        var store = new InMemoryReservationStore();
        var reservation1 = CreateReservation("RSV-1", "Alex", "offer-1");
        var reservation2 = CreateReservation("RSV-2", "Bob", "offer-1");  // Same offer ID

        await store.AddAsync(reservation1);
        await store.AddAsync(reservation2);

        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        // Should still be a set (no duplicates)
        Assert.Single(reservedOfferIds.Where(id => id == "offer-1"));
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_IsCaseInsensitive()
    {
        var store = new InMemoryReservationStore();
        var reservations = new[]
        {
            CreateReservation("RSV-1", "Alex", "OFFER-1"),
            CreateReservation("RSV-2", "Bob", "offer-1"),
            CreateReservation("RSV-3", "Charlie", "Offer-1")
        };

        foreach (var reservation in reservations)
        {
            await store.AddAsync(reservation);
        }

        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        // Should treat all as the same (case-insensitive)
        Assert.Single(reservedOfferIds.Where(id => id.Equals("offer-1", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public async Task AddAsync_PersistsAllReservationFields()
    {
        var store = new InMemoryReservationStore();
        var reservation = new Reservation
        {
            ReservationReference = new ReservationReference("RSV-COMPLETE"),
            TravellerName = "Alex",
            Destination = "Delhi",
            Provider = "PremierStays",
            RoomType = RoomType.Deluxe,
            TotalPrice = 720m,
            CancellationPolicy = CancellationPolicy.Flexible,
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID-COMPLETE-123",
            SelectedOfferId = "offer-complete",
            ValidationOutcome = "Accepted",
            OfferSnapshot = new HotelOffer
            {
                Id = "offer-snapshot",
                Provider = "PremierStays",
                RoomType = RoomType.Deluxe,
                PerNightRate = 144m,
                TotalStayPrice = 720m,
                CancellationPolicy = CancellationPolicy.Flexible,
                CancellationWindowHoursBeforeCheckIn = 48
            }
        };

        await store.AddAsync(reservation);
        var fetched = await store.GetAsync("rsv-complete");

        Assert.NotNull(fetched);
        Assert.Equal("Alex", fetched!.TravellerName);
        Assert.Equal("Delhi", fetched.Destination);
        Assert.Equal("PremierStays", fetched.Provider);
        Assert.Equal(RoomType.Deluxe, fetched.RoomType);
        Assert.Equal(720m, fetched.TotalPrice);
        Assert.Equal(CancellationPolicy.Flexible, fetched.CancellationPolicy);
        Assert.Equal(DocumentType.NationalId, fetched.DocumentType);
        Assert.Equal("NID-COMPLETE-123", fetched.DocumentNumber);
        Assert.Equal("offer-complete", fetched.SelectedOfferId);
        Assert.Equal("Accepted", fetched.ValidationOutcome);
    }

    [Fact]
    public async Task AddAsync_OfferSnapshotIsPreserved()
    {
        var store = new InMemoryReservationStore();
        var offerSnapshot = new HotelOffer
        {
            Id = "snapshot-id",
            Provider = "BudgetNests",
            RoomType = RoomType.Suite,
            PerNightRate = 200m,
            TotalStayPrice = 600m,
            CancellationPolicy = CancellationPolicy.NonRefundable,
            CancellationWindowHoursBeforeCheckIn = 24
        };

        var reservation = CreateReservation("RSV-SNAPSHOT", "Alex", "offer-1", offerSnapshot: offerSnapshot);

        await store.AddAsync(reservation);
        var fetched = await store.GetAsync("rsv-snapshot");

        Assert.NotNull(fetched!.OfferSnapshot);
        Assert.Equal("snapshot-id", fetched.OfferSnapshot.Id);
        Assert.Equal("BudgetNests", fetched.OfferSnapshot.Provider);
        Assert.Equal(RoomType.Suite, fetched.OfferSnapshot.RoomType);
        Assert.Equal(200m, fetched.OfferSnapshot.PerNightRate);
        Assert.Equal(600m, fetched.OfferSnapshot.TotalStayPrice);
        Assert.Equal(CancellationPolicy.NonRefundable, fetched.OfferSnapshot.CancellationPolicy);
        Assert.Equal(24, fetched.OfferSnapshot.CancellationWindowHoursBeforeCheckIn);
    }

    [Fact]
    public async Task AddAsync_MultipleTravellersSameCityDifferentOffers()
    {
        var store = new InMemoryReservationStore();
        var reservations = new[]
        {
            CreateReservation("RSV-1", "Alex", "offer-1", "Delhi"),
            CreateReservation("RSV-2", "Bob", "offer-2", "Delhi"),
            CreateReservation("RSV-3", "Charlie", "offer-3", "Delhi")
        };

        foreach (var reservation in reservations)
        {
            await store.AddAsync(reservation);
        }

        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        Assert.Equal(3, reservedOfferIds.Count);
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_ReturnsImmutableCollection()
    {
        var store = new InMemoryReservationStore();
        var reservation = CreateReservation("RSV-1", "Alex", "offer-1");

        await store.AddAsync(reservation);
        var reservedOfferIds = await store.GetReservedOfferIdsAsync();

        // Verify it's a read-only collection
        Assert.IsAssignableFrom<IReadOnlyCollection<string>>(reservedOfferIds);
    }

    [Fact]
    public async Task GetAsync_IsThreadSafeWithConcurrentAdds()
    {
        var store = new InMemoryReservationStore();
        var tasks = Enumerable.Range(1, 10)
            .Select(i => store.AddAsync(CreateReservation($"RSV-{i}", $"Traveller{i}", $"offer-{i}")))
            .ToList();

        await Task.WhenAll(tasks);

        // All should be retrievable
        for (int i = 1; i <= 10; i++)
        {
            var fetched = await store.GetAsync($"rsv-{i}");
            Assert.NotNull(fetched);
        }
    }

    [Fact]
    public async Task GetReservedOfferIdsAsync_MultipleCallsReturnConsistentSet()
    {
        var store = new InMemoryReservationStore();
        var reservation = CreateReservation("RSV-1", "Alex", "offer-1");

        await store.AddAsync(reservation);

        var first = await store.GetReservedOfferIdsAsync();
        var second = await store.GetReservedOfferIdsAsync();

        Assert.Equal(first, second);
    }

    [Fact]
    public async Task AddAsync_WithVeryLongReservationReference()
    {
        var store = new InMemoryReservationStore();
        var longRef = "RSV-" + new string('A', 1000);
        var reservation = CreateReservation(longRef, "Alex", "offer-1");

        await store.AddAsync(reservation);
        var fetched = await store.GetAsync(longRef);

        Assert.NotNull(fetched);
    }

    [Fact]
    public async Task GetAsync_WithSpecialCharactersInReference()
    {
        var store = new InMemoryReservationStore();
        var reference = "RSV-ABC-123_XYZ";
        var reservation = CreateReservation(reference, "Alex", "offer-1");

        await store.AddAsync(reservation);
        var fetched = await store.GetAsync(reference);

        Assert.NotNull(fetched);
    }

    private static Reservation CreateReservation(string reference, string travellerName, string offerId, string destination = "Delhi", HotelOffer? offerSnapshot = null)
    {
        return new Reservation
        {
            ReservationReference = new ReservationReference(reference),
            TravellerName = travellerName,
            Destination = destination,
            Provider = "PremierStays",
            RoomType = RoomType.Standard,
            TotalPrice = 360m,
            CancellationPolicy = CancellationPolicy.FreeCancellation,
            DocumentType = DocumentType.NationalId,
            DocumentNumber = $"NID-{travellerName}",
            SelectedOfferId = offerId,
            OfferSnapshot = offerSnapshot,
            ValidationOutcome = "Accepted"
        };
    }
}
